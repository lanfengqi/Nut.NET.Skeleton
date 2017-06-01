using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Repositories.Configuration;
using System.Data.Entity;

namespace Foundatio.Skeleton.Domain.Repositories.Configuration {
    public class OrganizationModelBuilder : ModelBuilderBase {

        public override void Configure(DbModelBuilder modelBuilder) {

            modelBuilder.Entity<Organization>().ToTable("Organization");
            modelBuilder.Entity<Organization>().HasKey(emp => emp.Id);

            modelBuilder.Entity<Organization>().Property(emp => emp.Name).HasMaxLength(500);

            modelBuilder.Entity<Organization>().Ignore(emp => emp.Data);
        }
    }
}
