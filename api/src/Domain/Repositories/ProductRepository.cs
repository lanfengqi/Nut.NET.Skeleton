using FluentValidation;
using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Repositories;

namespace Foundatio.Skeleton.Domain.Repositories {
    public class ProductRepository : EFRepositoryBase<Product>, IProductRepository {

        public ProductRepository(IEFRepositoryContext efRepositoryContext, IValidator<Product> validators)
            : base(efRepositoryContext, validators) {
        }
    }
}
