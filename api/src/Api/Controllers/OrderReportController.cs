using Foundatio.Skeleton.Api.Models;
using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Domain.Services;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace Foundatio.Skeleton.Api.Controllers {

    [RoutePrefix(API_PREFIX + "/orderReports")]
    [Authorize(Roles = AuthorizationRoles.User)]
    public class OrderReportController : AppApiController {
        private readonly IOrderReportService _orderReportService;

        public OrderReportController(IOrderReportService orderReportService) {

            _orderReportService = orderReportService;
        }

        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(OrderByUserReportLineModel))]
        [HttpPost]
        [Route("UserReport")]
        public async Task<IHttpActionResult> OrderUserReportLine(DateTime? reportDate = null) {
            if (!reportDate.HasValue)
                reportDate = DateTime.Parse(DateTime.UtcNow.ToShortDateString());

            if (base.currentUser == null)
                return base.NotFound();

            try {

                var result = await _orderReportService.GetOrderUserReportLine(currentUser.Id, reportDate, reportDate.Value.AddDays(1));

                
                return Ok(new OrderByUserReportLineModel {
                    AvgEnteredPrice = result.AvgEnteredPrice,
                    OrderItemQuantity = result.OrderItemQuantity,
                    OrderNum = result.OrderNum,
                    TotalEnteredWeight = result.TotalEnteredWeight,
                    TotalOrder = result.TotalOrder,
                    TotalOrderCost = result.TotalOrderCost,
                    TotalOrderPaymentMoney = result.TotalOrderPaymentMoney
                });

            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }
    }
}
