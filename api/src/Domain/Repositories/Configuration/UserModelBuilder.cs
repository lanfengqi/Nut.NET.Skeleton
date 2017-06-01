using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Repositories.Configuration;
using System.Data.Entity;

namespace Foundatio.Skeleton.Domain.Repositories.Configuration {
    public sealed class UserModelBuilder : ModelBuilderBase {

        public override void Configure(DbModelBuilder modelBuilder) {

            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<User>().HasKey(emp => emp.Id);

            modelBuilder.Entity<User>().Property(emp => emp.EmailAddress).HasMaxLength(500);

            modelBuilder.Entity<User>().Ignore(emp => emp.Data);
            modelBuilder.Entity<User>().Ignore(emp => emp.Memberships);
            modelBuilder.Entity<User>().Ignore(emp => emp.Roles);
        }

    }
}
