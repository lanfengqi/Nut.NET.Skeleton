using System.Data.Entity;
using Foundatio.Caching;

namespace Foundatio.Skeleton.Repositories {
    public interface IEFRepositoryContext {

        IDbContext Context { get; }

        ICacheClient Cache { get; }
    }
}
