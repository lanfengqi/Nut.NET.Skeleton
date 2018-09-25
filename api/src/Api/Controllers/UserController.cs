using AutoMapper;
using Foundatio.Logging;
using Foundatio.Messaging;
using Foundatio.Skeleton.Api.Extensions;
using Foundatio.Skeleton.Api.Models;
using Foundatio.Skeleton.Api.Security;
using Foundatio.Skeleton.Core.Utility;
using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Domain.Repositories;
using Foundatio.Skeleton.Domain.Services;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace Foundatio.Skeleton.Api.Controllers {
    [RoutePrefix(API_PREFIX + "/users")]
    [Authorize(Roles = AuthorizationRoles.User)]
    public class UserController : RepositoryApiController<IUserRepository, User, ViewUser, User, UpdateUser> {
        private readonly IPublicFileStorage _publicFileStorage;
        private readonly IMessagePublisher _messagePublisher;
        private readonly IRoleRepository _roleRepository;
        private readonly ITemplatedSmsService _templatedSmsService;

        public UserController(
            ILoggerFactory loggerFactory,
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            ITemplatedSmsService templatedSmsService,
            IPublicFileStorage publicFileStorage,
            IMapper mapper,
            IMessagePublisher messagePublisher) : base(loggerFactory, userRepository, mapper) {
            _publicFileStorage = publicFileStorage;
            _messagePublisher = messagePublisher;
            _roleRepository = roleRepository;
            _templatedSmsService = templatedSmsService;
        }

        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ViewCurrentUser))]
        [HttpGet]
        [Route("me")]
        public async Task<IHttpActionResult> GetCurrentUseAsync() {
            if (base.currentUser == null)
                return base.NotFound();

            var currentUser = await GetModelAsync(base.currentUser.Id);
            if (currentUser == null)
                return NotFound();

            var viewUser = new ViewCurrentUser(currentUser, currentUser.Roles.Select(x => x.SystemName).ToArray(), currentUser.OrganizationId);

            return Ok(viewUser);
        }

        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ViewUser))]
        [HttpGet]
        [Route("{id:objectid}", Name = "GetUserById")]
        public override Task<IHttpActionResult> GetByIdAsync(string id) {
            return base.GetByIdAsync(id);
        }

        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(UpdateUser))]
        [HttpPatch]
        [Route("{id:objectid}")]
        public override Task<IHttpActionResult> PatchAsync(string id, UpdateUser changes, long? version = null) {
            return base.PatchAsync(id, changes, version);
        }

        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ViewUser))]
        [HttpPut]
        [Route("{id:objectid}")]
        public override Task<IHttpActionResult> PutAsync(string id, ViewUser user, long? version = null) {
            return base.PutAsync(id, user, version);
        }

        [HttpDelete]
        [Route("{id:objectid}")]
        [OverrideAuthorization]
        [Authorize(Roles = AuthorizationRoles.Admin)]
        public override Task<IHttpActionResult> DeleteAsync(string id) {
            return base.DeleteAsync(id);
        }

        [HttpDelete]
        [Route("{id:objectid}/remove")]
        [OverrideAuthorization]
        [RequireOrganization]
        [Authorize(Roles = AuthorizationRoles.Admin)]
        public async Task<IHttpActionResult> RemoveAsync(string id) {
            var user = await GetModelAsync(id);
            if (user == null)
                return NotFound();

            var organizationId = GetSelectedOrganizationId();

            if (user.OrganizationId == organizationId) {
                await _repository.RemoveAsync(user.Id);
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpPost]
        [Route("{id:objectid}/phone/{phone:minlength(1)}")]
        [Authorize(Roles = AuthorizationRoles.User)]
        [RequireOrganization]
        public async Task<IHttpActionResult> UpdatePhoneAsync(string id, string phone) {
            var user = await GetModelAsync(id);
            if (user == null)
                return NotFound();

            if (String.Equals(currentUser.Phone, phone, StringComparison.OrdinalIgnoreCase))
                return Ok(new { IsVerified = user.IsPhoneVerified });

            phone = phone.ToLower();
            if (!await IsPhonesAvailableInternalAsync(phone))
                return BadRequest("A user with this phone already exists.");

            user.Phone = phone;

            await UpdateModelAsync(user);

            return Ok(new { IsVerified = user.IsPhoneVerified });
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("verify-phone")]
        public async Task<IHttpActionResult> Verify(string token) {
            var user = await _repository.GetByVerifyPhoneTokenAsync(token);
            if (user == null)
                return NotFound();

            if (!user.HasValidPhoneTokenExpiration())
                return BadRequest("Verify Email Address Token has expired.");

            user.MarkPhoneVerified();

            await _repository.SaveAsync(user);

            return Ok();
        }

        [HttpGet]
        [Route("{id:objectid}/resend-verification-phone")]
        public async Task<IHttpActionResult> ResendVerificationPhone(string id) {
            var user = await GetModelAsync(id);
            if (user == null)
                return NotFound();

            if (!user.IsPhoneVerified) {
                user.CreateVerifyPhoneToken();
                await _repository.SaveAsync(user);

                _templatedSmsService.SendPhoneVerifyNotification(user);
            }

            return Ok();
        }

        [HttpPost]
        [Route("{id:objectid}/admin-role")]
        [OverrideAuthorization]
        [RequireOrganization]
        [Authorize(Roles = AuthorizationRoles.Admin)]
        public async Task<IHttpActionResult> AddAdminRoleAsync(string id) {
            var user = await GetModelAsync(id);
            if (user == null)
                return NotFound();

            var organizationId = GetSelectedOrganizationId();
            if (await user.AddedAdminMembershipRoles(_roleRepository, organizationId)) {
                await _repository.SaveAsync(user);

            }

            return Ok();
        }

        [HttpDelete]
        [Route("{id:objectid}/admin-role")]
        [OverrideAuthorization]
        [RequireOrganization]
        [Authorize(Roles = AuthorizationRoles.Admin)]
        public async Task<IHttpActionResult> DeleteAdminRoleAsync(string id) {
            var user = await GetModelAsync(id);
            if (user == null)
                return NotFound();

            var organizationId = GetSelectedOrganizationId();

            if (organizationId == user.OrganizationId) {
                var adminRole = await _roleRepository.GetBySystemNameAsync(AuthorizationRoles.Admin);
                user.Roles.Remove(adminRole);
                await _repository.SaveAsync(user);
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpPost]
        [Route("{id:objectid}/global-admin-role")]
        [OverrideAuthorization]
        [Authorize(Roles = AuthorizationRoles.GlobalAdmin)]
        public async Task<IHttpActionResult> AddGlobalAdminRoleAsync(string id) {
            var user = await GetModelAsync(id);
            if (user == null)
                return NotFound();

            if (await user.AddedGlobalAdminRole(_roleRepository)) {
                await _repository.SaveAsync(user);
            }

            return Ok();
        }

        [HttpDelete]
        [Route("{id:objectid}/global-admin-role")]
        [OverrideAuthorization]
        [Authorize(Roles = AuthorizationRoles.GlobalAdmin)]
        public async Task<IHttpActionResult> DeleteGlobalAdminRoleAsync(string id) {
            var user = await GetModelAsync(id);
            if (user == null)
                return NotFound();

            if (user.Roles.Any(x => x.SystemName == AuthorizationRoles.GlobalAdmin)) {
                var globalAdminRole = await _roleRepository.GetBySystemNameAsync(AuthorizationRoles.GlobalAdmin);
                user.Roles.Remove(globalAdminRole);
                await _repository.SaveAsync(user);
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        private async Task<bool> IsPhonesAvailableInternalAsync(string phone) {
            if (String.IsNullOrWhiteSpace(phone))
                return false;

            if (currentUser != null && String.Equals(currentUser.Phone, phone, StringComparison.OrdinalIgnoreCase))
                return true;

            return await _repository.GetByPhoneAsync(phone) == null;
        }

        protected override async Task<User> GetModelAsync(string id, bool useCache = false) {
            if (Request.IsAdmin() || String.Equals(currentUser.Id, id))
                return await base.GetModelAsync(id, useCache: useCache);

            return null;
        }

        protected override Task<IReadOnlyCollection<User>> GetModelsAsync(string[] ids, bool useCache = false) {
            if (Request.IsAdmin())
                return base.GetModelsAsync(ids, useCache: useCache);

            return base.GetModelsAsync(ids.Where(id => String.Equals(currentUser.Id, id)).ToArray());
        }
    }
}
