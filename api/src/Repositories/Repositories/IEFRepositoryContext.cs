using System.Data.Entity;

namespace Foundatio.Skeleton.Repositories.Repositories {
    public interface IEFRepositoryContext {

        /// <summary>
        /// Gets the <see cref="DbContext"/> instance handled by Entity Framework.
        /// </summary>
        DbContext Context { get; }
    }
}
