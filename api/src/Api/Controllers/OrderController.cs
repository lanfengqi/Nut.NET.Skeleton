using AutoMapper;
using Foundatio.Logging;
using Foundatio.Skeleton.Api.Models;
using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Domain.Repositories;
using Foundatio.Skeleton.Domain.Services;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace Foundatio.Skeleton.Api.Controllers {

    [RoutePrefix(API_PREFIX + "/orders")]
    [Authorize(Roles = AuthorizationRoles.User)]
    public class OrderController : RepositoryApiController<IOrderRepository, Order, ViewOrder, ViewOrder, ViewOrder> {
        private readonly IOrderProcessingService _orderProcessingService;

        public OrderController(
            ILoggerFactory loggerFactory,
            IOrderRepository orderRepository,
            IMapper mapper,
            IOrderProcessingService orderProcessingService)
            : base(loggerFactory, orderRepository, mapper) {

            _orderProcessingService = orderProcessingService;
        }

        [HttpGet]
        [Route("{id:objectid}", Name = "GetOrderById")]
        public override async Task<IHttpActionResult> GetByIdAsync(string id) {
            var order = await GetModelAsync(id);
            if (order == null)
                return NotFound();

            var viewPurchaseCar = await Map<ViewOrder>(order);
            return Ok(viewPurchaseCar);
        }

        [HttpGet]
        [Route("GetOrderByCustomOrderNumber")]
        public async Task<IHttpActionResult> GetByCustomOrderNumberAsync(string customOrderNumber) {
            var order = await _repository.GetByCustomOrderNumberAsync(customOrderNumber);
            if (order == null)
                return NotFound();

            var viewPurchaseCar = await Map<ViewOrder>(order);
            return Ok(viewPurchaseCar);
        }


        [HttpDelete]
        [Route("{id:objectid}")]
        public override async Task<IHttpActionResult> DeleteAsync(string id) {
            var order = await GetModelAsync(id);
            if (order == null)
                return NotFound();

            await _orderProcessingService.DeleteOrder(order);

            return Ok();
        }

        [HttpPost]
        [Route("CancelOrder")]
        public async Task<IHttpActionResult> CancelOrderAsync(string id) {
            var order = await GetModelAsync(id);
            if (order == null)
                return NotFound();
            try {
                await _orderProcessingService.CancelOrder(order);
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }

            return Ok();
        }
    }
}
