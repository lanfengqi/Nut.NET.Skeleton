using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Foundatio.Skeleton.Domain.Repositories {
    public interface IOrderRepository : IEFRepository<Order> {

        Task<IReadOnlyCollection<OrderProductReportLine>> OrderProductReport(DateTime? startDate = null, DateTime? endDate = null);
    }
}
