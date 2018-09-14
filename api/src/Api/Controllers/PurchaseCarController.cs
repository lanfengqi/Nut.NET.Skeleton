using AutoMapper;
using Foundatio.Logging;
using Foundatio.Skeleton.Api.Extensions;
using Foundatio.Skeleton.Api.Models;
using Foundatio.Skeleton.Core.Extensions;
using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Domain.Repositories;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace Foundatio.Skeleton.Api.Controllers {

    [RoutePrefix(API_PREFIX + "/purchasecars")]
    [Authorize(Roles = AuthorizationRoles.User)]
    public class PurchaseCarController : RepositoryApiController<IPurchaseCarRepository, PurchaseCar, ViewPurchaseCar, NewPurchaseCar, ViewPurchaseCar> {

        public PurchaseCarController(
            ILoggerFactory loggerFactory,
            IPurchaseCarRepository purchaseCarRepository,
            IMapper mapper) : base(loggerFactory, purchaseCarRepository, mapper) {

        }


        [HttpGet]
        [Route("cars")]
        public async Task<IHttpActionResult> GetForUserAsync() {
            if (currentUser == null)
                return NotFound();

            var cars = await _repository.GetByUserIdAsync(currentUser.Id);

            return Ok(cars);
        }

        [HttpGet]
        [Route("{id:objectid}", Name = "GetPurchaseCarById")]
        public override async Task<IHttpActionResult> GetByIdAsync(string id) {
            var car = await GetModelAsync(id);
            if (car == null)
                return NotFound();

            var viewPurchaseCar = await Map<ViewPurchaseCar>(car);
            return Ok(viewPurchaseCar);
        }

        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(NewPurchaseCar))]
        [HttpPost]
        [Route]
        public override Task<IHttpActionResult> PostAsync(NewPurchaseCar value) {
            return base.PostAsync(value);
        }

        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ViewPurchaseCar))]
        [HttpPut]
        [Route("{id:objectid}")]
        public override Task<IHttpActionResult> PutAsync(string id, ViewPurchaseCar value, long? version = null) {
            return base.PutAsync(id, value, version);
        }

        [HttpDelete]
        [Route("{id:objectid}")]
        public override Task<IHttpActionResult> DeleteAsync(string id) {
            return base.DeleteAsync(id);
        }

        protected override Task<Domain.Models.PurchaseCar> AddModelAsync(Domain.Models.PurchaseCar value) {
            value.Id = Guid.NewGuid().ToString("N");
            value.CreatedUtc = value.UpdatedUtc = DateTime.UtcNow;
            value.CarTotal = value.EnteredPrice * (value.EnteredWeight - value.EnteredGrossWeight);
            value.UserId = Request.GetUser().Id;
            value.OrganizationId = Request.GetSelectedOrganizationId();

            return base.AddModelAsync(value);
        }

    }
}
