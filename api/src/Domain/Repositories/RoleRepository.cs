using FluentValidation;
using Foundatio.Caching;
using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Foundatio.Skeleton.Domain.Repositories {
    public class RoleRepository : EFRepositoryBase<Role>, IRoleRepository {

        public RoleRepository(IEFRepositoryContext efRepositoryContext, ICacheClient cacheClient, IValidator<Role> validators)
            : base(efRepositoryContext, cacheClient, validators) {
        }

        public async Task<Role> GetBySystemNameAsync(string systemName) {
            if (String.IsNullOrEmpty(systemName))
                return null;

            systemName = systemName.ToLower();

            var roles = await this.FindAsync(x => x.SystemName.Equals(systemName));
            return roles.FirstOrDefault();
        }

        public async Task<IReadOnlyCollection<Role>> GetBySystemNamesAsync(string[] systemNames) {
            var docs = systemNames?.ToList();
            if (docs == null || docs.Any(d => d == null))
                throw new ArgumentNullException(nameof(systemNames));

            if (docs.Count == 0)
                return new List<Role>();

            var roles = await this.FindAsync(x => systemNames.Contains(x.SystemName));
            return roles;

        }
    }
}
