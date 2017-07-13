using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Repositories;
using System.Threading.Tasks;

namespace Foundatio.Skeleton.Domain.Repositories {
    public interface IOrganizationRepository : IEFRepository<Organization> {

        Task<Organization> GetByNameAsync(string name);
    }
}
