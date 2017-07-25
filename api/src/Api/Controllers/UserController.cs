using AutoMapper;
using Foundatio.Logging;
using Foundatio.Messaging;
using Foundatio.Skeleton.Api.Extensions;
using Foundatio.Skeleton.Api.Models;
using Foundatio.Skeleton.Api.Security;
using Foundatio.Skeleton.Api.Utility;
using Foundatio.Skeleton.Core.Collections;
using Foundatio.Skeleton.Core.JsonPatch;
using Foundatio.Skeleton.Core.Utility;
using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Domain.Repositories;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace Foundatio.Skeleton.Api.Controllers {
    [RoutePrefix(API_PREFIX + "/users")]
    //[Authorize(Roles = AuthorizationRoles.User)]
    public class UserController : RepositoryApiController<IUserRepository, User, ViewUser, User, UpdateUser> {
        private readonly IPublicFileStorage _publicFileStorage;
        private readonly IMessagePublisher _messagePublisher;
        private readonly IRoleRepository _roleRepository;

        public UserController(
            ILoggerFactory loggerFactory,
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IPublicFileStorage publicFileStorage,
            IMapper mapper,
            IMessagePublisher messagePublisher) : base(loggerFactory, userRepository, mapper) {
            _publicFileStorage = publicFileStorage;
            _messagePublisher = messagePublisher;
            _roleRepository = roleRepository;
        }

        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ViewCurrentUser))]
        [HttpGet]
        [Route("me")]
        public async Task<IHttpActionResult> GetCurrentUser() {
            var currentUser = await GetModel(CurrentUser.Id);
            if (currentUser == null)
                return NotFound();

            var viewUser = new ViewCurrentUser(currentUser, currentUser.Roles.Select(x => x.SystemName).ToArray(), currentUser.OrganizationId);

            return Ok(viewUser);
        }

        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ViewUser))]
        [HttpGet]
        [Route("{id:objectid}", Name = "GetUserById")]
        public override Task<IHttpActionResult> GetById(string id) {
            return base.GetById(id);
        }

        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ViewUser))]
        [HttpPatch]
        [Route("{id:objectid}")]
        public override Task<IHttpActionResult> PatchAsync(string id, PatchDocument changes, long? version = null) {
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
            var user = await GetModel(id);
            if (user == null)
                return NotFound();

            var organizationId = GetSelectedOrganizationId();

            if (user.OrganizationId == organizationId) {
                await _repository.RemoveAsync(user.Id);
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpPost]
        [Route("{id:objectid}/email-address/{email:minlength(1)}")]
        [Authorize(Roles = AuthorizationRoles.User)]
        [RequireOrganization]
        public async Task<IHttpActionResult> UpdateEmailAddress(string id, string email) {
            var user = await GetModel(id);
            if (user == null)
                return NotFound();

            if (String.Equals(CurrentUser.EmailAddress, email, StringComparison.OrdinalIgnoreCase))
                return Ok(new { IsVerified = user.IsEmailAddressVerified });

            email = email.ToLower();
            if (!await IsEmailAddressAvailableInternal(email))
                return BadRequest("A user with this email address already exists.");

            user.EmailAddress = email;

            await UpdateModelAsync(user);

            return Ok(new { IsVerified = user.IsEmailAddressVerified });
        }

        [HttpPost]
        [Route("{id:objectid}/admin-role")]
        [OverrideAuthorization]
        [RequireOrganization]
        [Authorize(Roles = AuthorizationRoles.Admin)]
        public async Task<IHttpActionResult> AddAdminRole(string id) {
            var user = await GetModel(id);
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
        public async Task<IHttpActionResult> DeleteAdminRole(string id) {
            var user = await GetModel(id);
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
        public async Task<IHttpActionResult> AddGlobalAdminRole(string id) {
            var user = await GetModel(id);
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
        public async Task<IHttpActionResult> DeleteGlobalAdminRole(string id) {
            var user = await GetModel(id);
            if (user == null)
                return NotFound();

            if (user.Roles.Any(x => x.SystemName == AuthorizationRoles.GlobalAdmin)) {
                var globalAdminRole = await _roleRepository.GetBySystemNameAsync(AuthorizationRoles.GlobalAdmin);
                user.Roles.Remove(globalAdminRole);
                await _repository.SaveAsync(user);
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        private async Task<bool> IsEmailAddressAvailableInternal(string email) {
            if (String.IsNullOrWhiteSpace(email))
                return false;

            if (CurrentUser != null && String.Equals(CurrentUser.EmailAddress, email, StringComparison.OrdinalIgnoreCase))
                return true;

            return await _repository.GetByEmailAddressAsync(email) == null;
        }

        protected override async Task<User> GetModel(string id) {
            if (Request.IsAdmin() || String.Equals(CurrentUser.Id, id))
                return await base.GetModel(id);

            return null;
        }

        protected override Task<IReadOnlyCollection<User>> GetModels(string[] ids) {
            if (Request.IsAdmin())
                return base.GetModels(ids);

            return base.GetModels(ids.Where(id => String.Equals(CurrentUser.Id, id)).ToArray());
        }
    }
}
