using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Repositories.Configuration;

namespace Foundatio.Skeleton.Domain.Repositories.Configuration {
    public class UserMap : NutEntityTypeConfiguration<User> {

        public UserMap() {
            this.ToTable("User");
            this.HasKey(c => c.Id);

            this.Property(u => u.EmailAddress).HasMaxLength(500);

            this.HasMany(p => p.Roles)
                .WithMany()
                .Map(m => {
                    m.ToTable("user_roles_mapping");
                    m.MapLeftKey("UserId");
                    m.MapRightKey("RoleId");
                });
        }
    }
}
