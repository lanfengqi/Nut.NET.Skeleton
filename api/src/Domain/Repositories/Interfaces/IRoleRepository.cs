﻿using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Foundatio.Skeleton.Domain.Repositories {
    public interface IRoleRepository : IEFRepository<Role> {

        Task<Role> GetBySystemNameAsync(string systemName);

        Task<IReadOnlyCollection<Role>> GetBySystemNamesAsync(string[] systemNames);
    }
}
