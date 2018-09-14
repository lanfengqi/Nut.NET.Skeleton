using FluentValidation;
using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Foundatio.Skeleton.Domain.Repositories {
    public class PurchaseCarRepository : EFRepositoryBase<PurchaseCar>, IPurchaseCarRepository {

        public PurchaseCarRepository(IEFRepositoryContext efRepositoryContext, IValidator<PurchaseCar> validators)
            : base(efRepositoryContext, validators) {
        }

        public async Task<IReadOnlyCollection<PurchaseCar>> GetByUserIdAsync(string userId) {

            if (userId == string.Empty)
                return null;

            return await FindAsync(x => x.UserId == userId, o => o.UpdatedUtc, SortOrder.Descending);
        }
    }
}
