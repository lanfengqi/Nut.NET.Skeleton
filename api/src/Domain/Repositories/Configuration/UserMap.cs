using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Repositories.Configuration;

namespace Foundatio.Skeleton.Domain.Repositories.Configuration {
    public class UserMap : NutEntityTypeConfiguration<User> {

        public UserMap() {
            this.ToTable("User");
            this.HasKey(c => c.Id);

            this.Property(u => u.Id).HasMaxLength(36);
            this.Property(u => u.Phone).HasMaxLength(50);
            this.Property(u => u.FullName).HasMaxLength(200);
            this.Property(u => u.VerifyPhoneToken).HasMaxLength(50);
            this.Property(u => u.ProfileImagePath).HasMaxLength(200);
            this.Property(u => u.OrganizationId).HasMaxLength(36);

            this.HasMany(p => p.Roles)
                .WithMany(r => r.Users)
                .Map(m => m.ToTable("user_roles_mapping"));
        }
    }
}
