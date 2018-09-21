using Foundatio.Skeleton.Domain.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Foundatio.Skeleton.Domain.Services {
    public class OrderReportService : IOrderReportService {
        private readonly IOrderRepository _orderRepository;

        public OrderReportService(IOrderRepository orderRepository) {
            _orderRepository = orderRepository;
        }


        public async Task<OrderByUserReportLine> GetOrderUserReportLine(string userId, DateTime? startDateUtc = null, DateTime? endDateUtc = null) {

            var orders = await _orderRepository.FindAsync(x => x.UserId == userId
            && (x.CreatedUtc >= startDateUtc.Value && startDateUtc.HasValue)
            && (x.CreatedUtc <= endDateUtc.Value && endDateUtc.HasValue));

            var result = new OrderByUserReportLine();

            if (orders != null && orders.Any()) {
                result.AvgEnteredPrice = orders.Select(x => x.OrderItems.Average(i => i.EnteredPrice)).Average();
                result.TotalEnteredWeight = orders.Select(x => x.OrderItems.Sum(i => i.EnteredWeight)).Sum();
                result.TotalOrderCost = orders.Select(x => x.OrderCosts.Sum(i => i.EnteredMoney)).Sum();
                result.OrderItemQuantity = orders.Select(x => x.OrderItems.Sum(i => i.Quantity)).Sum();
                result.OrderNum = orders.Count();
                result.TotalOrderPaymentMoney = orders.Sum(x => x.OrderPaymentMoney);
                result.TotalOrder = orders.Sum(x => x.OrderTotal);
            }
            return result;
        }
    }
}
