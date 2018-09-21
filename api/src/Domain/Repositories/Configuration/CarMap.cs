using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Repositories.Configuration;

namespace Foundatio.Skeleton.Domain.Repositories.Configuration {
    public class CarMap : NutEntityTypeConfiguration<Car> {

        public CarMap() {
            this.ToTable("Car");

            this.HasKey(c => c.Id);
            this.Property(c => c.OrganizationId).HasMaxLength(50);
            this.Property(c => c.Notes).HasMaxLength(500);
            this.Property(c => c.CarNum).HasMaxLength(20);
            this.Property(c => c.CarType).HasMaxLength(20);

            this.Property(c => c.CarOwner).HasMaxLength(50);
            this.Property(c => c.UseProperty).HasMaxLength(20);
            this.Property(c => c.Address).HasMaxLength(500);
            this.Property(c => c.BrandModel).HasMaxLength(200);
            this.Property(c => c.VIN).HasMaxLength(20);
            this.Property(c => c.EngineNo).HasMaxLength(20);
            this.Property(c => c.FileNumber).HasMaxLength(50);
            this.Property(c => c.OutlineSize).HasMaxLength(20);

            this.Property(c => c.TotalWeight).HasPrecision(18, 3);
            this.Property(c => c.EnteredWeight).HasPrecision(18, 3);
        }
    }
}
