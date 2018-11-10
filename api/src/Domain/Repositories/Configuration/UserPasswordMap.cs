using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Repositories.Configuration;

namespace Foundatio.Skeleton.Domain.Repositories.Configuration {
    public class UserPasswordMap : NutEntityTypeConfiguration<UserPassword> {
        public UserPasswordMap() {
            this.ToTable("UserPassword");
            this.HasKey(k => k.Id);

            this.Property(p => p.Id).HasMaxLength(36);
            this.Property(p => p.UserId).HasMaxLength(36);
            this.Property(p => p.Password).HasMaxLength(200);
            this.Property(p => p.Salt).HasMaxLength(20);
            this.Property(p => p.PasswordResetToken).HasMaxLength(50);
        }
    }
}
