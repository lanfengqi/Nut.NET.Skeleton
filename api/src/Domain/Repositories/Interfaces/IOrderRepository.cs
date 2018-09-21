using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Repositories;
using System.Threading.Tasks;

namespace Foundatio.Skeleton.Domain.Repositories {
    public interface IOrderRepository : IEFRepository<Order> {

        Task<Order> GetByCustomOrderNumberAsync(string customOrderNumber);
    }
}
