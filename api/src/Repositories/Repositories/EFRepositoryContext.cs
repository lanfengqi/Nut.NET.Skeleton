using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundatio.Skeleton.Repositories {
    public class EFRepositoryContext : IEFRepositoryContext {

        private readonly DbContext _dbContext;

        public EFRepositoryContext(DbContext dbContext) {
            _dbContext = dbContext;
        }
        /// <summary>
        /// Gets the <see cref="DbContext"/> instance handled by Entity Framework.
        /// </summary>
        public DbContext Context {
            get {
                if (_dbContext != null)
                    return _dbContext;

                return null;
            }
        }
    }
}
