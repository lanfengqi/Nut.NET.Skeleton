using AutoMapper;
using Foundatio.Logging;
using Foundatio.Skeleton.Api.Extensions;
using Foundatio.Skeleton.Api.Models;
using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Domain.Repositories;
using Foundatio.Skeleton.Repositories.Model;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace Foundatio.Skeleton.Api.Controllers {

    [RoutePrefix(API_PREFIX + "/farmers")]
    [Authorize(Roles = AuthorizationRoles.User)]
    public class FarmerController : RepositoryApiController<IFarmerRepository, Farmer, ViewFarmer, NewFarmer, NewFarmer> {

        public FarmerController(
            ILoggerFactory loggerFactory,
            IFarmerRepository farmerRepository,
            IMapper mapper) : base(loggerFactory, farmerRepository, mapper) {

        }

        [HttpGet]
        [Route("admin")]
        public async Task<IHttpActionResult> GetForAdminsAsync(string phone = "", string farmerName = "", int page = 1, int limit = 10) {

            page = GetPage(page);
            limit = GetLimit(limit);

            var organizations = await _repository.FindAsync(x => (x.Phone == phone || string.IsNullOrEmpty(phone)) &&
            (x.FarmerName.Contains(farmerName) || string.IsNullOrEmpty(farmerName)) && !x.Deleted, new PagingOptions { Page = page, Limit = limit });

            return OkWithResourceLinks(organizations, organizations.TotalPages > page, page, organizations.TotalCount);
        }

        [HttpGet]
        [Route("{id:objectid}", Name = "GetFarmerById")]
        public override Task<IHttpActionResult> GetByIdAsync(string id) {
            return base.GetByIdAsync(id);
        }

        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(NewFarmer))]
        [HttpPost]
        [Route]
        public override Task<IHttpActionResult> PostAsync(NewFarmer value) {
            return base.PostAsync(value);
        }

        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(NewFarmer))]
        [HttpPatch]
        [Route("{id:objectid}")]
        public override Task<IHttpActionResult> PatchAsync(string id, NewFarmer value, long? version = null) {
            return base.PatchAsync(id, value, version);
        }

        [HttpDelete]
        [Route("{id:objectid}")]
        public override Task<IHttpActionResult> DeleteAsync(string id) {
            return base.DeleteAsync(id);
        }

        protected override Task<Domain.Models.Farmer> AddModelAsync(Domain.Models.Farmer value) {
            value.Id = Guid.NewGuid().ToString("N");
            value.CreatedUtc = value.UpdatedUtc = DateTime.UtcNow;
            value.OrganizationId = Request.GetSelectedOrganizationId();
            value.Deleted = false;

            return base.AddModelAsync(value);
        }

    }
}
