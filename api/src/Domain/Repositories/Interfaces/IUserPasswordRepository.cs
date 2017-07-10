using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Repositories;
using System.Threading.Tasks;

namespace Foundatio.Skeleton.Domain.Repositories {
    public  interface IUserPasswordRepository : IRepository<UserPassword> {

        Task<UserPassword> GetByPasswordResetTokenAsync(string token);

        Task<UserPassword> GetByUserIdAsync(string userId);
    }
}
