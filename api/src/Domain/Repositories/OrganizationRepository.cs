using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Repositories.Repositories;

namespace Foundatio.Skeleton.Domain.Repositories {
    public class OrganizationRepository : EFRepositoryBase<Organization>, IOrganizationRepository {
        public OrganizationRepository(IEFRepositoryContext efRepositoryContext)
            : base(efRepositoryContext, null, null) {
        }
    }
}
