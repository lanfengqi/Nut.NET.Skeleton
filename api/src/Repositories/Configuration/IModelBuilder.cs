using Foundatio.Skeleton.Repositories.Model;
using System.Data.Entity;

namespace Foundatio.Skeleton.Repositories.Configuration {
    public interface IModelBuilder {

        void Configure(DbModelBuilder modelBuilder);
    }
}
