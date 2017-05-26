using Foundatio.Skeleton.Repositories.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Foundatio.Skeleton.Repositories {
    public interface IRepository<T> : IReadOnlyRepository<T> where T : class, IIdentity, new() {

        Task<T> SaveAsync(T document);
        Task SaveAsync(IEnumerable<T> documents);
        Task PatchAsync(T document);
        Task RemoveAsync(string id);
        Task RemoveAsync(IEnumerable<string> ids);
        Task RemoveAsync(T document);
        Task RemoveAsync(IEnumerable<T> documents);
    }
}
