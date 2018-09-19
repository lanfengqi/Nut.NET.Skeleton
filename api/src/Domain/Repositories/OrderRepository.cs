using FluentValidation;
using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Repositories;
using System.Data.Entity;

namespace Foundatio.Skeleton.Domain.Repositories {
    public class OrderRepository : EFRepositoryBase<Order>, IOrderRepository {

        public OrderRepository(IEFRepositoryContext eFRepositoryContext, IValidator<Order> validators)
            : base(eFRepositoryContext, validators) {

        }
    }
}
