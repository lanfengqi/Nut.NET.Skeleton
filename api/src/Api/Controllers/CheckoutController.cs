using Foundatio.Metrics;
using Foundatio.Skeleton.Api.Extensions;
using Foundatio.Skeleton.Api.Models;
using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Domain.Services;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace Foundatio.Skeleton.Api.Controllers {
    [RoutePrefix(API_PREFIX + "/Checkout")]
    [Authorize(Roles = AuthorizationRoles.User)]
    public class CheckoutController : AppApiController {
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IMetricsClient _metricsClient;

        public CheckoutController(IOrderProcessingService orderProcessingService,
            IMetricsClient metricsClient) {
            _orderProcessingService = orderProcessingService;
            _metricsClient = metricsClient;
        }

        /// <summary>
        /// Login with a local username and password.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>An auth token.</returns>
        [SwaggerResponse(HttpStatusCode.OK)]
        [HttpPost]
        [Route("placeorder")]
        public async Task<IHttpActionResult> PlaceOrderAsync(ProcessOrderModel model) {
            if (model == null)
                return BadRequest("Process Order Request is required.");

            try {
                var request = new ProcessOrderRequest() {
                    FarmerId = model.FarmerId,
                    Notes = model.Notes,
                    OtherCost = model.OtherCost,
                    PaymentCardNumber = model.PaymentCardNumber,
                    PaymentMethodSystemName = model.PaymentMethodSystemName,

                    UserId = Request.GetUser().Id,
                    OrganizationId = Request.GetSelectedOrganizationId(),
                };

                var result = await _orderProcessingService.PlaceOrder(request);

                if (!result.Success)
                    return BadRequest(string.Join(",", result.Errors));

                await _metricsClient.CounterAsync("Place Order");

                return Ok(result.PlacedOrder);
            } catch (Exception) {
                return Unauthorized();
            }
        }

    }
}
