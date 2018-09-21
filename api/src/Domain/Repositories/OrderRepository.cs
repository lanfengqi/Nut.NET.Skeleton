using FluentValidation;
using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Repositories;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace Foundatio.Skeleton.Domain.Repositories {
    public class OrderRepository : EFRepositoryBase<Order>, IOrderRepository {

        public OrderRepository(IEFRepositoryContext eFRepositoryContext, IValidator<Order> validators)
            : base(eFRepositoryContext, validators) {

        }


        public async Task<Order> GetByCustomOrderNumberAsync(string customOrderNumber) {
            if (String.IsNullOrEmpty(customOrderNumber))
                return null;

            customOrderNumber = customOrderNumber.ToLower();

            var roles = await this.FindAsync(x => x.CustomOrderNumber == customOrderNumber);
            return roles.FirstOrDefault();
        }
    }
}
