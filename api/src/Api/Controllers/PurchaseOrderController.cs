using AutoMapper;
using Foundatio.Logging;
using Foundatio.Skeleton.Api.Models;
using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Domain.Repositories;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace Foundatio.Skeleton.Api.Controllers {
    [RoutePrefix(API_PREFIX + "/purchaseorders")]
    public class PurchaseOrderController : RepositoryApiController<IPurchaseOrderRepository, PurchaseOrder, ViewPurchaseOrder, NewPurchaseOrder, ViewPurchaseOrder> {

        public PurchaseOrderController(IPurchaseOrderRepository purchaseOrderRepository,
             ILoggerFactory loggerFactory,
              IMapper mapper
            ) : base(loggerFactory, purchaseOrderRepository, mapper) {
        
        }

        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ViewPurchaseOrder))]
        [HttpGet]
        [Route("{id:objectid}", Name = "GetPurchaseOrderById")]
        public override async Task<IHttpActionResult> GetByIdAsync(string id) {
            var purchaseOrder = await GetModelAsync(id);
            if (purchaseOrder == null)
                return NotFound();

            var viewOrder = await Map<ViewPurchaseOrder>(purchaseOrder);
            return Ok(viewOrder);
        }

        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(NewPurchaseOrder))]
        [HttpPost]
        [Route]
        public override Task<IHttpActionResult> PostAsync(NewPurchaseOrder value) {
            return base.PostAsync(value);
        }

        [HttpPost]
        [Route("{id:objectid}/cancel")]
        public async Task<IHttpActionResult> CancelPurchaseOrderAsync(string id) {
            var purchaseOrder = await GetModelAsync(id);
            if (purchaseOrder == null)
                return NotFound();
            if (purchaseOrder.Stat == PurchaseOrderStatus.Successful) {
                purchaseOrder.Stat = PurchaseOrderStatus.Canceled;
                await _repository.SaveAsync(purchaseOrder);
            }
            return Ok();
        }

        protected override Task<PurchaseOrder> AddModelAsync(PurchaseOrder value) {
            value.Id = Guid.NewGuid().ToString("N");
            value.CreatedUtc = value.UpdatedUtc = DateTime.UtcNow;
            value.Stat = PurchaseOrderStatus.Successful;

            value.PurchaseOrderItems.ToList().ForEach(x => {
                x.Id = Guid.NewGuid().ToString("N");
                x.PurchaseOrderId = value.Id;
                x.CreatedUtc = x.UpdatedUtc = DateTime.UtcNow;
            });

            return base.AddModelAsync(value);
        }
    }
}
