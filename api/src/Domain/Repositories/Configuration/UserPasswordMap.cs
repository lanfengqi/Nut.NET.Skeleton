using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Repositories.Configuration;

namespace Foundatio.Skeleton.Domain.Repositories.Configuration {
    public class UserPasswordMap : NutEntityTypeConfiguration<UserPassword> {
        public UserPasswordMap() {
            this.ToTable("UserPassword");
            this.HasKey(emp => emp.Id);
        }
    }
}
