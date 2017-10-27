using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Repositories.Configuration;

namespace Foundatio.Skeleton.Domain.Repositories.Configuration {
    public class TokenMap : NutEntityTypeConfiguration<Token> {

        public TokenMap() {
            this.ToTable("Token");
            this.HasKey(c => c.Id);

            this.Property(u => u.OrganizationId).HasMaxLength(50);
            this.Property(u => u.UserId).HasMaxLength(50);
            this.Property(u => u.Refresh).HasMaxLength(50);
            this.Property(u => u.Notes).HasMaxLength(500);
            this.Property(u => u.CreatedBy).HasMaxLength(50);
        }
    }
}
