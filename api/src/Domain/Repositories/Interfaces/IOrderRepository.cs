using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Repositories;
using System;
using System.Threading.Tasks;

namespace Foundatio.Skeleton.Domain.Repositories {
    public interface IOrderRepository : IEFRepository<Order> {

        Task<PagedList<Order>> SearchOrders(string userId = "", DateTime? startDateUtc = null, DateTime? endDateUtc = null, int page = 1, int limit = 10);

        Task<Order> GetByCustomOrderNumberAsync(string customOrderNumber);
    }
}
