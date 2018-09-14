using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Repositories;
using System.Threading.Tasks;

namespace Foundatio.Skeleton.Domain.Repositories {
    public interface ICarRepository : IEFRepository<Car> {

        Task<Car> GetByCarNumAsync(string carNum);
    }
}
