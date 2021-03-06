using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Repositories.Configuration;

namespace Foundatio.Skeleton.Domain.Repositories.Configuration {
    public class FarmerMap : NutEntityTypeConfiguration<Farmer> {

        public FarmerMap() {
            this.ToTable("Farmer");
            this.HasKey(c => c.Id);

            this.Property(u => u.Id).HasMaxLength(36);
            this.Property(u => u.OrganizationId).HasMaxLength(36);
            this.Property(u => u.Address).HasMaxLength(500);
            this.Property(u => u.Contact).HasMaxLength(50);
            this.Property(u => u.ContactIdCardNumber).HasMaxLength(20);
            this.Property(u => u.ContactAddress).HasMaxLength(500);
            this.Property(u => u.Notes).HasMaxLength(500);
            this.Property(u => u.FarmerName).HasMaxLength(50);
            this.Property(u => u.Phone).HasMaxLength(50);
        }
    }
}
