using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Repositories.Repositories;
using System.Threading.Tasks;

namespace Foundatio.Skeleton.Domain.Repositories {
    public interface IRoleRepository : IEFRepository<Role> {

        Task<Role> GetBySystemNameAsync(string systemName);
    }
}
