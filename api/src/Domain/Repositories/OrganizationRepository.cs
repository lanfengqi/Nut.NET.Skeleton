using Foundatio.Skeleton.Domain.Models;
using Foundatio.Caching;
using Foundatio.Skeleton.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace Foundatio.Skeleton.Domain.Repositories {
    public class OrganizationRepository : EFRepositoryBase<Organization>, IOrganizationRepository {
        public OrganizationRepository(IEFRepositoryContext efRepositoryContext,ICacheClient cacheClient, IValidator<Organization> validators)
            : base(efRepositoryContext, cacheClient, validators) {
        }

        public async Task<Organization> GetByNameAsync(string name) {

            if (String.IsNullOrEmpty(name))
                return null;

            name = name.ToLower();

            var organizations = await this.FindAsync(x => x.Name.Equals(name));
            return organizations.FirstOrDefault();
        }
    }
}
