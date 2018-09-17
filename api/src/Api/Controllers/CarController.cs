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

    [RoutePrefix(API_PREFIX + "/cars")]
    [Authorize(Roles = AuthorizationRoles.User)]
    public class CarController : RepositoryApiController<ICarRepository, Car, ViewCar, NewCar, UpdateCar> {

        public CarController(
            ILoggerFactory loggerFactory,
            ICarRepository carRepository,
            IMapper mapper) : base(loggerFactory, carRepository, mapper) {

        }

        [HttpGet]
        [Route("admin")]
        [Authorize(Roles = AuthorizationRoles.GlobalAdmin)]
        public async Task<IHttpActionResult> GetForAdminsAsync(string carNum = "", int page = 1, int limit = 10) {

            page = GetPage(page);
            limit = GetLimit(limit);

            var organizations = await _repository.FindAsync(x => x.CarNum.Contains(carNum) && x.IsActive, new PagingOptions { Page = page, Limit = limit });

            return OkWithResourceLinks(organizations, organizations.TotalPages > page, page, organizations.TotalCount);
        }

        [HttpGet]
        [Route("GetCarByCarNum")]
        public async Task<IHttpActionResult> GetByCarNumAsync(string carNum) {
            var model = await _repository.GetByCarNumAsync(carNum);
            if (model == null)
                return NotFound();

            var car = await Map<ViewCar>(model);
            return Ok(model);
        }

        [HttpGet]
        [Route("{id:objectid}", Name = "GetCarById")]
        public override async Task<IHttpActionResult> GetByIdAsync(string id) {
            var model = await GetModelAsync(id);
            if (model == null)
                return NotFound();

            var car = await Map<ViewCar>(model);
            return Ok(model);
        }

        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(NewCar))]
        [HttpPost]
        [Route]
        public override Task<IHttpActionResult> PostAsync(NewCar value) {
            return base.PostAsync(value);
        }

        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(UpdateCar))]
        [HttpPatch]
        [Route("{id:objectid}")]
        public override Task<IHttpActionResult> PatchAsync(string id, UpdateCar value, long? version = null) {
            return base.PatchAsync(id, value, version);
        }

        [HttpDelete]
        [Route("{id:objectid}")]
        public override Task<IHttpActionResult> DeleteAsync(string id) {
            return base.DeleteAsync(id);
        }

        protected override Task<Domain.Models.Car> AddModelAsync(Domain.Models.Car value) {
            value.Id = Guid.NewGuid().ToString("N");
            value.CreatedUtc = value.UpdatedUtc = DateTime.UtcNow;
            value.OrganizationId = Request.GetSelectedOrganizationId();
            value.IsActive = true;

            return base.AddModelAsync(value);
        }

    }
}
