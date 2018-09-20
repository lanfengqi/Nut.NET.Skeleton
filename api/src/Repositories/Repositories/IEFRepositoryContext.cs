using System.Data.Entity;
using Foundatio.Caching;

namespace Foundatio.Skeleton.Repositories {
    public interface IEFRepositoryContext {

        DbContext Context { get; }

        ICacheClient Cache { get; }
    }
}
