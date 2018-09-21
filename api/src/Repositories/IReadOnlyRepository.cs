using Foundatio.Skeleton.Repositories.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Foundatio.Skeleton.Repositories {
    public interface IReadOnlyRepository<T> where T : class, new() {
        Task<long> CountAsync();
        Task<T> GetByIdAsync(string id, bool useCache = false, TimeSpan? expiresIn = null);
        Task<IReadOnlyCollection<T>> GetByIdsAsync(IEnumerable<string> ids, bool useCache = false, TimeSpan? expiresIn = null);
        Task<IReadOnlyCollection<T>> GetAllAsync(IPagingOptions paging = null);
        Task<bool> ExistsAsync(string id);
    }
}
