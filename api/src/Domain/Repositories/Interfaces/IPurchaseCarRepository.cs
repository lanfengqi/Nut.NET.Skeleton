using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Foundatio.Skeleton.Domain.Repositories {
    public interface IPurchaseCarRepository : IRepository<PurchaseCar> {

        Task<IReadOnlyCollection<PurchaseCar>> GetByUserIdAsync(string userId);
    }
}
