using AutoMapper;
using Foundatio.Logging;
using Foundatio.Messaging;
using Foundatio.Skeleton.Api.Models;
using Foundatio.Skeleton.Api.Security;
using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Domain.Repositories;
using Foundatio.Skeleton.Repositories.Model;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace Foundatio.Skeleton.Api.Controllers {
    [RoutePrefix(API_PREFIX + "/organizations")]
    [Authorize(Roles = AuthorizationRoles.User)]
    public class OrganizationController : RepositoryApiController<IOrganizationRepository, Organization, ViewOrganization, NewOrganization, NewOrganization> {
        private readonly IUserRepository _userRepository;
        private readonly IMessagePublisher _messagePublisher;

        public OrganizationController(IOrganizationRepository organizationRepository,
            ILoggerFactory loggerFactory,
            IUserRepository userRepository,
            IMessagePublisher messagePublisher,
            IMapper mapper)
            : base(loggerFactory, organizationRepository, mapper) {
            _userRepository = userRepository;
            _messagePublisher = messagePublisher;
        }

        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ICollection<ViewOrganization>))]
        [HttpGet]
        [Route]
        [RequireOrganization]
        public async Task<IHttpActionResult> GetAsync() {

            var organizationId = currentUser.OrganizationId;
            var organization = await _repository.GetByIdAsync(organizationId);
            return Ok(organization);
        }

        [HttpGet]
        [Route("admin")]
        [Authorize(Roles = AuthorizationRoles.GlobalAdmin)]
        public async Task<IHttpActionResult> GetForAdminsAsync(int page = 1, int limit = 10) {

            page = GetPage(page);
            limit = GetLimit(limit);

            var organizations = await _repository.FindAsync(x => x.Version >= 1, new PagingOptions { Page = page, Limit = limit });

            return OkWithResourceLinks(organizations, organizations.TotalPages > page, page, organizations.TotalCount);
        }

        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ViewOrganization))]
        [HttpGet]
        [Route("{id:objectid}", Name = "GetOrganizationById")]
        public override async Task<IHttpActionResult> GetByIdAsync(string id) {
            var organization = await GetModelAsync(id, true);
            if (organization == null)
                return NotFound();

            var viewOrganization = await Map<ViewOrganization>(organization);
            return Ok(viewOrganization);
        }

        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ViewOrganization))]
        [HttpPost]
        [Route]
        public override Task<IHttpActionResult> PostAsync(NewOrganization value) {
            return base.PostAsync(value);
        }

        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(NewOrganization))]
        [HttpPatch]
        [Route("{id:objectid}")]
        public override Task<IHttpActionResult> PatchAsync(string id, NewOrganization changes, long? version = null) {
            return base.PatchAsync(id, changes, version);
        }

        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ViewOrganization))]
        [HttpPut]
        [Route("{id:objectid}")]
        public override Task<IHttpActionResult> PutAsync(string id, ViewOrganization organization, long? version = null) {
            return base.PutAsync(id, organization, version);
        }

        [HttpDelete]
        [Route("{id:objectid}")]
        public override Task<IHttpActionResult> DeleteAsync(string id) {
            return base.DeleteAsync(id);
        }

        [HttpPost]
        [Route("delete")]
        public async Task<IHttpActionResult> DeleteAsync([FromBody]EntitySelection selection) {
            if (selection == null || ((selection.Ids == null || selection.Ids.Length == 0) && selection.Filter == null))
                return StatusCode(HttpStatusCode.BadRequest);

            if (selection.Ids != null && selection.Ids.Length > 0)
                return await base.DeleteAsync(selection.Ids);

            if (selection.Filter != null) {
                //  queue work item here
            }

            return StatusCode(HttpStatusCode.BadRequest);
        }

        protected override Task<Domain.Models.Organization> AddModelAsync(Domain.Models.Organization value) {
            value.Id = Guid.NewGuid().ToString("N");
            value.CreatedUtc = value.UpdatedUtc = DateTime.UtcNow;
            value.IsVerified = false;
            value.Version = 1;

            return base.AddModelAsync(value);
        }
    }
}
