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

        public UserController(
            ILoggerFactory loggerFactory,
            IUserRepository userRepository,
            IPublicFileStorage publicFileStorage,
            IMapper mapper,
            IMessagePublisher messagePublisher) : base(loggerFactory, userRepository, mapper) {
            _publicFileStorage = publicFileStorage;
            _messagePublisher = messagePublisher;
        }

        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ViewCurrentUser))]
        [HttpGet]
        [Route("me")]
        public async Task<IHttpActionResult> GetCurrentUser() {

            //if (CurrentUser == null)
            //    return NotFound();

            var currentUser = await _repository.GetByIdAsync("3552");
            if (currentUser == null)
                return NotFound();

            return Ok(new {
                Name = "Admin"
            });
        }

        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ViewUser))]
        [HttpGet]
        [Route("{id:objectid}", Name = "GetUserById")]
        public override Task<IHttpActionResult> GetById(string id) {
            return base.GetById(id);
        }

        //[SwaggerResponse(HttpStatusCode.OK, Type = typeof(ICollection<ViewUser>))]
        //[HttpGet]
        //[Route]
        //[RequireOrganization]
        //public override Task<IHttpActionResult> Get(string f = null, string q = null, string sort = null, string offset = null, string mode = null, int page = 1, int limit = 10, string facet = null) {
        //    return base.Get(f, q, sort, offset, mode, page, limit, facet);
        //}

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
            var user = await GetModel(id, false);
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
            var user = await GetModel(id, false);
            if (user == null)
                return NotFound();

            if (String.Equals(CurrentUser.EmailAddress, email, StringComparison.OrdinalIgnoreCase))
                return Ok(new { IsVerified = user.IsEmailAddressVerified });

            email = email.ToLower();
            if (!await IsEmailAddressAvailableInternal(email))
                return BadRequest("A user with this email address already exists.");

            user.EmailAddress = email;

            await UpdateModelAsync(user);

            //if (!user.IsEmailAddressVerified)
            //    await ResendVerificationEmail(id);

            return Ok(new { IsVerified = user.IsEmailAddressVerified });
        }

        //[HttpGet]
        //[AllowAnonymous]
        //[Route("verify-email-address")]
        //public async Task<IHttpActionResult> Verify(string token) {
        //    var user = await _repository.GetByVerifyEmailAddressTokenAsync(token);
        //    if (user == null)
        //        return NotFound();

        //    if (!user.HasValidEmailAddressTokenExpiration())
        //        return BadRequest("Verify Email Address Token has expired.");

        //    user.MarkEmailAddressVerified();

        //    await _repository.SaveAsync(user);

        //    var admins = user.GetMembershipsWithAdminRole();
        //    if (admins != null && admins.Any())
        //        foreach (var membership in user.Memberships)
        //            await _organizationService.TryMarkOrganizationAsVerifiedAsync(membership.OrganizationId);

        //    ExceptionlessClient.Default.CreateFeatureUsage("Verify Email Address").AddObject(user).Submit();

        //    if (user.Password == null) {
        //        // TODO(derek): when we get last org in there, use that
        //        var t = await _tokenRepository.GetOrCreateUserToken(user.Id, null);
        //        return Ok(new TokenResponseModel { Token = t.Id });
        //    }

        //    return Ok();
        //}


        [HttpPost]
        [Route("{id:objectid}/admin-role")]
        [OverrideAuthorization]
        [RequireOrganization]
        [Authorize(Roles = AuthorizationRoles.Admin)]
        public async Task<IHttpActionResult> AddAdminRole(string id) {
            var user = await GetModel(id, false);
            if (user == null)
                return NotFound();

            var organizationId = GetSelectedOrganizationId();
            if (user.AddedAdminMembershipRoles(organizationId)) {


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
            var user = await GetModel(id, false);
            if (user == null)
                return NotFound();

            var organizationId = GetSelectedOrganizationId();

            if(organizationId == user.OrganizationId) {
                user.Roles.Remove(AuthorizationRoles.Admin);
                await _repository.AddAsync(user);
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpPost]
        [Route("{id:objectid}/global-admin-role")]
        [OverrideAuthorization]
        [Authorize(Roles = AuthorizationRoles.GlobalAdmin)]
        public async Task<IHttpActionResult> AddGlobalAdminRole(string id) {
            var user = await GetModel(id, false);
            if (user == null)
                return NotFound();

            if (user.AddedGlobalAdminRole()) {
                await _repository.AddAsync(user);
            }

            return Ok();
        }

        [HttpDelete]
        [Route("{id:objectid}/global-admin-role")]
        [OverrideAuthorization]
        [Authorize(Roles = AuthorizationRoles.GlobalAdmin)]
        public async Task<IHttpActionResult> DeleteGlobalAdminRole(string id) {
            var user = await GetModel(id, false);
            if (user == null)
                return NotFound();

            if (user.Roles.Contains(AuthorizationRoles.GlobalAdmin)) {
                user.Roles.Remove(AuthorizationRoles.GlobalAdmin);
                await _repository.AddAsync(user);
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

        protected override async Task<User> GetModel(string id, bool useCache = true) {
            if (Request.IsAdmin() || String.Equals(CurrentUser.Id, id))
                return await base.GetModel(id, useCache);

            return null;
        }

        protected override Task<IReadOnlyCollection<User>> GetModels(string[] ids, bool useCache = true) {
            if (Request.IsAdmin())
                return base.GetModels(ids, useCache);

            return base.GetModels(ids.Where(id => String.Equals(CurrentUser.Id, id)).ToArray(), useCache);
        }
    }
}
