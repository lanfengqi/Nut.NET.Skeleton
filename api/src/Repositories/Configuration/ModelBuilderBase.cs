using Foundatio.Skeleton.Repositories.Model;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Foundatio.Skeleton.Repositories.Configuration {
    public abstract class ModelBuilderBase :IModelBuilder {

        public abstract void Configure(DbModelBuilder modelBuilder);
    }
}
