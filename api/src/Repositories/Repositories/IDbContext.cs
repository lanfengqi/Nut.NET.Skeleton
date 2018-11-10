using Foundatio.Skeleton.Repositories.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundatio.Skeleton.Repositories {
    public interface IDbContext {
        /// <summary>
        /// Get DbSet
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <returns>DbSet</returns>
        IDbSet<TEntity> Set<TEntity>() where TEntity : class, IIdentity, new();

        /// <summary>
        /// Save changes
        /// </summary>
        /// <returns>Result</returns>
        Task<int> SaveChangesAsync();
    }
}
