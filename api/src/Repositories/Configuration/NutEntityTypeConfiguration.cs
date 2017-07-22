using Foundatio.Skeleton.Repositories.Model;
using System.Data.Entity.ModelConfiguration;

namespace Foundatio.Skeleton.Repositories.Configuration {
    public  class NutEntityTypeConfiguration<T>: EntityTypeConfiguration<T> where T:class, IIdentity {

        public NutEntityTypeConfiguration() {
            PostInitialize();
        }

        protected virtual void PostInitialize() {

        }
    }
}
