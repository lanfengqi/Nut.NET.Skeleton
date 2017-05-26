using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Repositories;
using System.Threading.Tasks;

namespace Foundatio.Skeleton.Domain.Repositories {
    public interface IUserRepository : IRepository<User> {
        Task<User> GetByEmailAddressAsync(string emailAddress);
    }
}
