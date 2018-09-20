using System.Data.Entity;
using Foundatio.Caching;

namespace Foundatio.Skeleton.Repositories {
    public class EFRepositoryContext : IEFRepositoryContext {
        private readonly ICacheClient _cache;
        public EFRepositoryContext(ICacheClient cache) {
            _cache = cache;
        }

        public DbContext Context {
            get {
                return new EFDbContext();
            }
        }

        public ICacheClient Cache {
            get {
                if (_cache == null)
                    return new NullCacheClient();
                return _cache;
            }
        }
    }
}
