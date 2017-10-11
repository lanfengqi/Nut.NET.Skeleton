using Foundatio.Skeleton.Domain.Services;
using Foundatio.Skeleton.Domain.Repositories;
using System.Threading.Tasks;
using System.Web.Http;
using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Api.Models;
using Foundatio.Logging;
using AutoMapper;
using Swashbuckle.Swagger.Annotations;
using System.Net;
using System;
using System.Linq;

namespace Foundatio.Skeleton.Api.Controllers {
    [RoutePrefix(API_PREFIX + "/orders")]
    public class OrderController : RepositoryApiController<IOrderRepository, Order, ViewOrder, NewOrder, ViewOrder> {

        private readonly OrderCalculationService _orderCalculationService;

        public OrderController(OrderCalculationService orderCalculationService,
            IOrderRepository orderRepository,
             ILoggerFactory loggerFactory,
              IMapper mapper
            ) : base(loggerFactory, orderRepository, mapper) {
            _orderCalculationService = orderCalculationService;
        }

        [HttpGet]
        [Route("calculateproductweigh")]
        public async Task<IHttpActionResult> CalculateProductWeigh() {
            await _orderCalculationService.CalculateProductWeigh();

            return Ok();
        }

        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ViewOrder))]
        [HttpGet]
        [Route("{id:objectid}", Name = "GetOrderById")]
        public override async Task<IHttpActionResult> GetByIdAsync(string id) {
            var order = await GetModelAsync(id);
            if (order == null)
                return NotFound();

            var viewOrder = await Map<ViewOrder>(order);
            return Ok(viewOrder);
        }

        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(NewOrder))]
        [HttpPost]
        [Route]
        public override Task<IHttpActionResult> PostAsync(NewOrder value) {
            return base.PostAsync(value);
        }

        protected override Task<Order> AddModelAsync(Order value) {
            value.Id = Guid.NewGuid().ToString("N");
            value.CreatedUtc = value.UpdatedUtc = DateTime.UtcNow;
            value.Stat = (int)OrderStatusType.Successful;

            value.OrderItems.ToList().ForEach(x => {
                x.Id = Guid.NewGuid().ToString("N");
                x.OrderId = value.Id;
                x.CreatedUtc = x.UpdatedUtc = DateTime.UtcNow;
            });

            return base.AddModelAsync(value);
        }
    }
}
