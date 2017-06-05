using Foundatio.Caching;
using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Repositories.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Foundatio.Skeleton.Domain.Repositories {
    public class RoleRepository : EFRepositoryBase<Role>, IRoleRepository {

        public RoleRepository(IEFRepositoryContext efRepositoryContext, ICacheClient cacheClient)
            : base(efRepositoryContext, cacheClient, null) {
        }

        public async Task<Role> GetBySystemNameAsync(string systemName) {
            if (String.IsNullOrEmpty(systemName))
                return null;

            systemName = systemName.ToLower();

            var roles = await this.FindAsync(x => x.SystemName.Equals(systemName));
            return roles.FirstOrDefault();
        }
    }
}
