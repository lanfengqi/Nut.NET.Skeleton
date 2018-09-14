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
            this.Property(c => c.EnteredWeight).HasPrecision(18, 3);
        }
    }
}
