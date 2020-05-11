using Foundatio.Caching;

namespace Foundatio.Skeleton.Repositories
{
    public class EFRepositoryContext : IEFRepositoryContext {
        private readonly ICacheClient _cache;
        private readonly IDbContext _dbContext;
        public EFRepositoryContext(IDbContext dbContext, ICacheClient cache) {
            _cache = cache;
            _dbContext = dbContext;
        }

        public IDbContext Context {
            get {
                if (_dbContext == null)
                    return new EFDbContext();
                return _dbContext;
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
