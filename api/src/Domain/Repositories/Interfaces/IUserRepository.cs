using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Repositories;
using System.Threading.Tasks;

namespace Foundatio.Skeleton.Domain.Repositories {
    public interface IUserRepository : IRepository<User> {
        Task<User> GetByPhoneAsync(string phone);

        Task<User> GetByVerifyPhoneTokenAsync(string token);
    }
}
