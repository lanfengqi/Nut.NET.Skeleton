using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Repositories.Configuration;
using System.Data.Entity;

namespace Foundatio.Skeleton.Domain.Repositories.Configuration {
    public class RoleModelBuilder : ModelBuilderBase {

        public override void Configure(DbModelBuilder modelBuilder) {

            modelBuilder.Entity<Role>().ToTable("role");
            modelBuilder.Entity<Role>().HasKey(emp => emp.Id);

            modelBuilder.Entity<Role>().Property(emp => emp.Name).HasMaxLength(50);
            modelBuilder.Entity<Role>().Property(emp => emp.SystemName).HasMaxLength(500);
            modelBuilder.Entity<Role>().Property(emp => emp.OrganizationId).HasMaxLength(50);
        }
    }
}
