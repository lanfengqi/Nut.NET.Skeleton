using Foundatio.Skeleton.Repositories.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Foundatio.Caching;

namespace Foundatio.Skeleton.Repositories.Repositories {
    public class EFReadOnlyRepositoryBase<T> : IEFReadOnlyRepository<T> where T : class, IIdentity, new() {
        protected static readonly bool HasIdentity = typeof(IIdentity).IsAssignableFrom(typeof(T));
        protected static readonly string EntityTypeName = typeof(T).Name;
        protected readonly DbContext _context;
        protected readonly ICacheClient _cacheClient;


        public EFReadOnlyRepositoryBase(IEFRepositoryContext efRepositoryContext, ICacheClient cacheClient) {
            _context = efRepositoryContext.Context;
            _cacheClient = cacheClient;
        }

        public async Task<IReadOnlyCollection<T>> FindAsync(Expression<Func<T, bool>> specification) {
            if (specification == null)
                throw new ArgumentNullException("specification");

            var query = _context.Set<T>().Where(specification);

            return await query.ToListAsync();
        }

        public async Task<PagedList<T>> FindAsync(Expression<Func<T, bool>> specification, IPagingOptions paging = null) {
            if (specification == null)
                throw new ArgumentNullException("specification");

            var query = _context.Set<T>().Where(specification);

            var result = new PagedList<T>();

            int pageIndex = paging.Page.HasValue ? paging.Page.Value : 1;
            int pageSize = paging.Limit.HasValue ? paging.Limit.Value : 10;

            int total = await query.CountAsync();
            result.TotalCount = total;
            result.TotalPages = total / pageSize;

            if (total % pageSize > 0)
                result.TotalPages++;

            result.PageSize = pageSize;
            result.PageIndex = pageIndex;

            query = query.OrderBy(x => x.Id);
            result.AddRange(await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync());

            return result;
        }

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> specification) {
            if (specification == null)
                return false;

            var query = _context.Set<T>().Where(specification);
            var result = await query.CountAsync();

            return result > 0 ? true : false;
        }

        public async Task<long> CountAsync(Expression<Func<T, bool>> specification) {
            if (specification == null)
                throw new ArgumentNullException("specification");

            var query = _context.Set<T>().Where(specification);
            return await query.CountAsync();
        }

        public async Task<long> CountAsync() {
            return await this.CountAsync(x => x.Id != null);
        }
        public async Task<T> GetByIdAsync(string id, bool useCache = false, TimeSpan? expiresIn = null) {

            string cacheKey = string.Format("Ef.Cache.Id.{0}", id);
            //Bug?? EF 
            if (useCache) {
                var isExist = await _cacheClient.ExistsAsync(cacheKey);
                if (isExist) {
                    var cachedValue = await _cacheClient.GetAsync<T>(cacheKey);
                    return cachedValue.Value;
                }
            }

            var query = _context.Set<T>().Where(x => x.Id == id);
            var result = await query.FirstOrDefaultAsync();

            if (useCache)
                await _cacheClient.SetAsync(cacheKey, result, expiresIn);

            return result;

        }
        public async Task<IReadOnlyCollection<T>> GetByIdsAsync(IEnumerable<string> ids, bool useCache = false, TimeSpan? expiresIn = null) {

            var idList = ids?.Distinct().Where(i => !String.IsNullOrEmpty(i)).ToList();
            if (idList == null || idList.Count == 0)
                return null;

            if (!HasIdentity)
                throw new NotSupportedException("Model type must implement IIdentity.");
            //Bug?? EF 
            string cacheKey = string.Format("Ef.Cache.Ids.{0}", idList.GetHashCode());

            if (useCache && await _cacheClient.ExistsAsync(cacheKey)) {
                var cachedValue = await _cacheClient.GetAsync<IReadOnlyCollection<T>>(cacheKey);
                return cachedValue.Value;
            }

            var result = await this.FindAsync(x => ids.Contains(x.Id));

            if (useCache)
                await _cacheClient.SetAsync(cacheKey, result, expiresIn);

            return result;
        }


        public async Task<IReadOnlyCollection<T>> GetAllAsync(IPagingOptions paging = null) {
            return await this.FindAsync(x => x.Id != null, paging);
        }


        public async Task<bool> ExistsAsync(string id) {
            if (String.IsNullOrEmpty(id))
                return false;

            return await this.ExistsAsync(x => x.Id == id);
        }
    }
}
