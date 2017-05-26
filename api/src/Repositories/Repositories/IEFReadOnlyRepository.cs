using Foundatio.Skeleton.Repositories.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Foundatio.Skeleton.Repositories.Repositories {
    public interface IEFReadOnlyRepository<T> : IReadOnlyRepository<T> where T : class, IIdentity, new() {

        Task<IReadOnlyCollection<T>> FindAsync(Expression<Func<T, bool>> specification);

        Task<PagedList<T>> FindAsync(Expression<Func<T, bool>> specification, IPagingOptions paging = null);

        Task<bool> ExistsAsync(Expression<Func<T, bool>> specification);

        Task<long> CountAsync(Expression<Func<T, bool>> specification);
    }
}
