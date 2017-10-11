using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Repositories.Configuration;

namespace Foundatio.Skeleton.Domain.Repositories.Configuration {
    public  class OrderItemMap : NutEntityTypeConfiguration<OrderItem> {
        public OrderItemMap() {
            this.ToTable("orderitem");
            this.HasKey(c => c.Id);


        }
    }
}
