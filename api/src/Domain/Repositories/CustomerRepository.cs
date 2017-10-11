using FluentValidation;
using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Repositories;

namespace Foundatio.Skeleton.Domain.Repositories {
    public class CustomerRepository : EFRepositoryBase<Customer>, ICustomerRepository {
        public CustomerRepository(IEFRepositoryContext efRepositoryContext, IValidator<Customer> validators)
            : base(efRepositoryContext, validators) {
        }


    }
}
