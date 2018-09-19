using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Repositories;
using System.Data.Entity;

namespace Foundatio.Skeleton.Domain.Repositories {
    public class FarmerRepository : EFRepositoryBase<Farmer>, IFarmerRepository {

        public FarmerRepository(IEFRepositoryContext eFRepositoryContext)
            : base(eFRepositoryContext, null) {
        }
    }
}
