using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Foundatio.Skeleton.Domain.Repositories {
    public class OrderRepository : EFRepositoryBase<Order>, IOrderRepository {
        public OrderRepository(IEFRepositoryContext efRepositoryContext)
            : base(efRepositoryContext, null) {
        }

        public async Task<IReadOnlyCollection<OrderProductReportLine>> OrderProductReport(DateTime? startDate = null, DateTime? endDate = null) {

            if (!startDate.HasValue || !endDate.HasValue)
                throw new ArgumentNullException("start date or end date is null");

            var query = _context.Set<Order>().Where(x => x.PickupTime >= startDate.Value && x.PickupTime <= endDate.Value);

            var items = query.SelectMany(x => x.OrderItems);

            var results = await (from oq in items
                                 group oq by oq.ProductId into g
                                 select new OrderProductReportLine {
                                     ProductId = g.Key,
                                     QuantityTotal = g.Sum(x => x.Quantity),
                                     WeightTotal = g.Sum(x => x.Weight),
                                     SaleTotal = g.Sum(x => x.SaleTotal)
                                 }).ToListAsync();
            return results;
        }
    }
}
