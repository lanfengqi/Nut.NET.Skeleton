using System.Data.Entity;

namespace Foundatio.Skeleton.Repositories {
    public class EFRepositoryContext : IEFRepositoryContext {
        // <summary>
        /// Gets the <see cref="DbContext"/> instance handled by Entity Framework.
        /// </summary>
        public DbContext Context {
            get {
                return new EFDbContext();
            }
        }
    }
}
