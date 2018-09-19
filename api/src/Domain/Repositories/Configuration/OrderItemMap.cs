using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Repositories.Configuration;

namespace Foundatio.Skeleton.Domain.Repositories.Configuration {
    public class OrderItemMap : NutEntityTypeConfiguration<OrderItem> {
        public OrderItemMap() {
            this.ToTable("OrderItem");
            this.HasKey(a => a.Id);

            this.Property(c => c.OrderId).HasMaxLength(50);

            this.Property(c => c.EnteredGrossWeight).HasPrecision(18, 3);
            this.Property(c => c.EnteredWeight).HasPrecision(18, 3);
            this.Property(c => c.EnteredPrice).HasPrecision(18, 4);
            this.Property(c => c.TtemTotal).HasPrecision(18, 4);

            this.Property(c => c.Lat).HasPrecision(18, 6);
            this.Property(c => c.Lng).HasPrecision(18, 6);

            this.HasRequired(orderItem => orderItem.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(orderItem => orderItem.OrderId).WillCascadeOnDelete(true);
        }
    }
}
