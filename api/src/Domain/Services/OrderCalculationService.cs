using Foundatio.Skeleton.Domain.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Foundatio.Skeleton.Domain.Services {
    public class OrderCalculationService {

        private readonly IOrderRepository _orderRepository;

        public OrderCalculationService(IOrderRepository orderRepository) {
            this._orderRepository = orderRepository;
        }

        public async Task CalculateProductWeigh() {
            var nowDate = DateTime.UtcNow.Date;
            var products = await _orderRepository.OrderProductReport(nowDate.AddDays(-2), nowDate.AddDays(2));

            var productWeighs = products.Select(x => {
                return new {
                    ProductId = x.ProductId,
                    WeightTotal = x.WeightTotal,
                    Order = x.WeightTotal * 1 + x.SaleTotal
                };
            });
        }
    }
}
