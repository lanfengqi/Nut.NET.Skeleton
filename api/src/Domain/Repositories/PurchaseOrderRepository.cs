using FluentValidation;
using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Repositories;

namespace Foundatio.Skeleton.Domain.Repositories {
    public class PurchaseOrderRepository : EFRepositoryBase<PurchaseOrder>, IPurchaseOrderRepository {
        public PurchaseOrderRepository(IEFRepositoryContext efRepositoryContext, IValidator<PurchaseOrder> validators)
            : base(efRepositoryContext, validators) {
        }
    }
}
