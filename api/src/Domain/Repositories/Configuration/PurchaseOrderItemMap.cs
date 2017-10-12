using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Repositories.Configuration;

namespace Foundatio.Skeleton.Domain.Repositories.Configuration {
    public  class PurchaseOrderItemMap : NutEntityTypeConfiguration<PurchaseOrderItem> {
        public PurchaseOrderItemMap() {
            this.ToTable("purchaseorderitem");
            this.HasKey(c => c.Id);


        }
    }
}
