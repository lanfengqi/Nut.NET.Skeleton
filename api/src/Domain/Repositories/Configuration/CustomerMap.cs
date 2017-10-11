using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Repositories.Configuration;

namespace Foundatio.Skeleton.Domain.Repositories.Configuration {
    public class CustomerMap : NutEntityTypeConfiguration<Customer> {
        public CustomerMap() {
            this.ToTable("customer");
            this.HasKey(c => c.Id);

            this.Property(u => u.Name).HasMaxLength(500);
            this.Property(u => u.Code).HasMaxLength(50);

        }
    }
}
