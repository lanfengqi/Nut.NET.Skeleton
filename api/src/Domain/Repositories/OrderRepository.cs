using FluentValidation;
using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Repositories;
using Foundatio.Skeleton.Repositories.Model;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Foundatio.Skeleton.Domain.Repositories {
    public class OrderRepository : EFRepositoryBase<Order>, IOrderRepository {

        public OrderRepository(IEFRepositoryContext eFRepositoryContext, IValidator<Order> validators)
            : base(eFRepositoryContext, validators) {

        }

        public async Task<PagedList<Order>> SearchOrders(string userId = "", DateTime? startDateUtc = null, DateTime? endDateUtc = null
              , int page = 1, int limit = 10) {

            var orders = await this.FindAsync(x => (x.UserId == userId || string.IsNullOrEmpty(userId))
            && (x.CreatedUtc >= startDateUtc.Value && startDateUtc.HasValue)
            && (x.CreatedUtc <= endDateUtc.Value && endDateUtc.HasValue),
               new PagingOptions { Page = page, Limit = limit });
            return orders;

        }
        public async Task<Order> GetByCustomOrderNumberAsync(string customOrderNumber) {
            if (String.IsNullOrEmpty(customOrderNumber))
                return null;

            customOrderNumber = customOrderNumber.ToLower();

            var orders = await this.FindAsync(x => x.CustomOrderNumber == customOrderNumber);
            return orders.FirstOrDefault();
        }
    }
}
