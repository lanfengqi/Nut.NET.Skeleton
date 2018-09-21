using Foundatio.Skeleton.Repositories.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Foundatio.Caching;
using Foundatio.Skeleton.Core.Extensions;

namespace Foundatio.Skeleton.Repositories {
    public class EFReadOnlyRepositoryBase<T> : IEFReadOnlyRepository<T> where T : class, IIdentity, new() {
        protected static readonly bool HasIdentity = typeof(IIdentity).IsAssignableFrom(typeof(T));
        protected static readonly string EntityTypeName = typeof(T).Name;
        protected readonly DbContext _context;

        private ScopedCacheClient _scopedCacheClient;

        public EFReadOnlyRepositoryBase(IEFRepositoryContext eFRepositoryContext) {
            _context = eFRepositoryContext.Context;
            SetCache(eFRepositoryContext.Cache);
        }

        public async Task<IReadOnlyCollection<T>> FindAsync(Expression<Func<T, bool>> specification) {
            if (specification == null)
                throw new ArgumentNullException("specification");

            var query = _context.Set<T>().Where(specification).AsNoTracking();

            return await query.ToListAsync();
        }

        public async Task<IReadOnlyCollection<T>> FindAsync<Property>(Expression<Func<T, bool>> specification, Expression<Func<T, Property>> orderByExpression, SortOrder sortOrder) {
            if (specification == null)
                throw new ArgumentNullException("specification");

            var query = _context.Set<T>().Where(specification).AsNoTracking();
            if (orderByExpression != null) {
                switch (sortOrder) {
                    case SortOrder.Ascending:
                        query = query.OrderBy<T, Property>(orderByExpression);
                        break;
                    case SortOrder.Descending:
                        query = query.OrderByDescending<T, Property>(orderByExpression);
                        break;
                    default:
                        break;
                }
            }
            return await query.ToListAsync();
        }

        public async Task<PagedList<T>> FindAsync(Expression<Func<T, bool>> specification, IPagingOptions paging = null) {
            if (specification == null)
                throw new ArgumentNullException("specification");

            var query = _context.Set<T>().Where(specification).AsNoTracking();

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

        public async Task<PagedList<T>> FindAsync<Property>(Expression<Func<T, bool>> specification, Expression<Func<T, Property>> orderByExpression, SortOrder sortOrder, IPagingOptions paging = null) {

            if (specification == null)
                throw new ArgumentNullException("specification");

            var query = _context.Set<T>().Where(specification).AsNoTracking();

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

            if (orderByExpression != null) {
                switch (sortOrder) {
                    case SortOrder.Ascending:
                        query = query.OrderBy<T, Property>(orderByExpression);
                        break;
                    case SortOrder.Descending:
                        query = query.OrderByDescending<T, Property>(orderByExpression);
                        break;
                    default:
                        query = query.OrderBy(x => x.Id);
                        break;
                }
            }

            result.AddRange(await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync());

            return result;
        }

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> specification) {
            if (specification == null)
                return false;

            var query = _context.Set<T>().Where(specification).AsNoTracking();
            var result = await query.CountAsync();

            return result > 0 ? true : false;
        }

        public async Task<long> CountAsync(Expression<Func<T, bool>> specification) {
            if (specification == null)
                throw new ArgumentNullException("specification");

            var query = _context.Set<T>().Where(specification).AsNoTracking();
            return await query.CountAsync();
        }

        public async Task<long> CountAsync() {
            return await this.CountAsync(x => x.Id != null);
        }

        public async Task<T> GetByIdAsync(string id, bool useCache = false, TimeSpan? expiresIn = null) {
            if (string.IsNullOrEmpty(id))
                return null;

            T hit = null;
            if (IsCacheEnabled && useCache)
                hit = await Cache.GetAsync<T>(id, default(T)).AnyContext();

            if (hit != null) {
                return hit;
            }

            hit = await _context.Set<T>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (IsCacheEnabled && hit != null && useCache)
                await Cache.SetAsync(id, hit, expiresIn ?? TimeSpan.FromSeconds(60)).AnyContext();

            return hit;
        }

        public async Task<IReadOnlyCollection<T>> GetByIdsAsync(IEnumerable<string> ids, bool useCache = false, TimeSpan? expiresIn = null) {

            var idList = ids?.Distinct().Where(i => !String.IsNullOrEmpty(i)).ToList();
            if (idList == null || idList.Count == 0)
                return null;

            var hits = new List<T>();
            if (IsCacheEnabled && useCache) {
               var  cachehits = await Cache.GetAllAsync<T>(ids).AnyContext();
                hits.AddRange(cachehits.Where(kvp => kvp.Value.HasValue).Select(kvp => kvp.Value.Value));
            }

            if (hits.Any()) {
                return hits;
            }

            hits = (await this.FindAsync(x => ids.Contains(x.Id))).ToList();
            if (IsCacheEnabled && hits.Any() && useCache) {
                foreach(var hit in  hits)
                    await Cache.SetAsync(hit.Id, hit, expiresIn ?? TimeSpan.FromSeconds(60)).AnyContext();
            }
               
            return hits;
        }

        public async Task<IReadOnlyCollection<T>> GetAllAsync(IPagingOptions paging = null) {
            return await this.FindAsync(x => x.Id != null, paging);
        }

        public async Task<bool> ExistsAsync(string id) {
            if (String.IsNullOrEmpty(id))
                return false;

            return await this.ExistsAsync(x => x.Id == id);
        }

        public bool IsCacheEnabled { get; set; }
        protected ScopedCacheClient Cache => _scopedCacheClient ?? new ScopedCacheClient(new NullCacheClient());

        private void SetCache(ICacheClient cache) {
            IsCacheEnabled = cache != null;
            _scopedCacheClient = new ScopedCacheClient(cache ?? new NullCacheClient(), EntityTypeName);
        }

        protected void DisableCache() {
            IsCacheEnabled = false;
            _scopedCacheClient = new ScopedCacheClient(new NullCacheClient(), EntityTypeName);
        }

    }
}
