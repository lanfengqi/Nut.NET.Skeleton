using Foundatio.Skeleton.Repositories.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Foundatio.Skeleton.Repositories {
    public interface IEFReadOnlyRepository<T> : IReadOnlyRepository<T> where T : class, IIdentity, new() {

        Task<IReadOnlyCollection<T>> FindAsync(Expression<Func<T, bool>> specification);

        Task<IReadOnlyCollection<T>> FindAsync<Property>(Expression<Func<T, bool>> specification, Expression<Func<T, Property>> orderByExpression, SortOrder sortOrder);

        Task<PagedList<T>> FindAsync(Expression<Func<T, bool>> specification, IPagingOptions paging = null);

        Task<PagedList<T>> FindAsync<Property>(Expression<Func<T, bool>> specification, Expression<Func<T, Property>> orderByExpression, SortOrder sortOrder, IPagingOptions paging = null);

        Task<bool> ExistsAsync(Expression<Func<T, bool>> specification);

        Task<long> CountAsync(Expression<Func<T, bool>> specification);
    }
}
