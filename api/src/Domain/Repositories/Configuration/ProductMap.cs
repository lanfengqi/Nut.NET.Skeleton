using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Repositories.Configuration;

namespace Foundatio.Skeleton.Domain.Repositories.Configuration {
    public class ProductMap : NutEntityTypeConfiguration<Product> {
        public ProductMap() {
            this.ToTable("product");
            this.HasKey(c => c.Id);

            this.Property(u => u.Name).HasMaxLength(500);
            this.Property(u => u.Code).HasMaxLength(50);
            this.Property(u => u.Unit).HasMaxLength(50);
        }
    }
}
