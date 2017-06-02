﻿using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Repositories.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Foundatio.Skeleton.Domain.Repositories {
    public class OrganizationRepository : EFRepositoryBase<Organization>, IOrganizationRepository {
        public OrganizationRepository(IEFRepositoryContext efRepositoryContext)
            : base(efRepositoryContext, null, null) {
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
