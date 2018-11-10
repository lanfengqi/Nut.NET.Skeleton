using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Repositories.Configuration;

namespace Foundatio.Skeleton.Domain.Repositories.Configuration {
    public class OrderMap : NutEntityTypeConfiguration<Order> {
        public OrderMap() {
            this.ToTable("Order");
            this.HasKey(a => a.Id);

            this.Property(c => c.Id).HasMaxLength(36);
            this.Property(c => c.OrganizationId).HasMaxLength(36);
            this.Property(c => c.UserId).HasMaxLength(36);
            this.Property(c => c.CarId).HasMaxLength(36);
            this.Property(c => c.FarmerId).HasMaxLength(36);
            this.Property(c => c.Notes).HasMaxLength(500);

            this.Property(c => c.CustomOrderNumber).HasMaxLength(50);
            this.Property(c => c.PaymentMethodSystemName).HasMaxLength(50);
            this.Property(c => c.PaymentCardNumber).HasMaxLength(50);
            this.Property(c => c.OrderTotal).HasPrecision(18, 4);
            this.Property(c => c.OrderPaymentMoney).HasPrecision(18, 4);
        }
    }
}
