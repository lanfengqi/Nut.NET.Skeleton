using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Repositories.Configuration;

namespace Foundatio.Skeleton.Domain.Repositories.Configuration {
    public class OrderMap : NutEntityTypeConfiguration<Order> {
        public OrderMap() {
            this.ToTable("order");
            this.HasKey(c => c.Id);


        }
    }
}
