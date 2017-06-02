using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Repositories;
using System.Threading.Tasks;

namespace Foundatio.Skeleton.Domain.Repositories {
    public interface IOrganizationRepository : IRepository<Organization> {

        Task<Organization> GetByNameAsync(string name);
    }
}
