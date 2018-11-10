using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Repositories.Configuration;

namespace Foundatio.Skeleton.Domain.Repositories.Configuration {
    public class RoleMap : NutEntityTypeConfiguration<Role> {

        public RoleMap() {
            this.ToTable("role");
            this.HasKey(c => c.Id);

            this.Property(u => u.Id).HasMaxLength(36);
            this.Property(u => u.Name).HasMaxLength(50);
            this.Property(u => u.SystemName).HasMaxLength(50);
            
        }
    }
}
