using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Repositories.Configuration;

namespace Foundatio.Skeleton.Domain.Repositories.Configuration {
    public class OrderCostMap : NutEntityTypeConfiguration<OrderCost> {
        public OrderCostMap() {
            this.ToTable("OrderCost");
            this.HasKey(a => a.Id);

            this.Property(c => c.OrderId).HasMaxLength(50);
            this.Property(c => c.Notes).HasMaxLength(500);
            this.Property(c => c.CostSystemName).HasMaxLength(50);
            this.Property(c => c.EnteredMoney).HasPrecision(18, 4);

            this.HasRequired(orderItem => orderItem.Order)
                .WithMany(o => o.OrderCosts)
                .HasForeignKey(orderItem => orderItem.OrderId);
        }
    }
}
