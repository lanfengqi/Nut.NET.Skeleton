using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Repositories.Configuration;

namespace Foundatio.Skeleton.Domain.Repositories.Configuration {
    public class PurchaseCarMap : NutEntityTypeConfiguration<PurchaseCar> {

        public PurchaseCarMap() {
            this.ToTable("PurchaseCar");
            this.HasKey(c => c.Id);

            this.Property(c => c.EnteredGrossWeight).HasPrecision(18, 3);
            this.Property(c => c.EnteredWeight).HasPrecision(18, 3);
            this.Property(c => c.EnteredPrice).HasPrecision(18, 3);
            this.Property(c => c.CarTotal).HasPrecision(18, 3);

            this.Property(c => c.Lat).HasPrecision(18, 6);
            this.Property(c => c.Lng).HasPrecision(18, 6);

            this.Property(c => c.Notes).HasMaxLength(500);
        }
    }
}
