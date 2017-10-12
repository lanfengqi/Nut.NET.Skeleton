using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Repositories.Configuration;

namespace Foundatio.Skeleton.Domain.Repositories.Configuration {
    public  class PurchaseOrderMap : NutEntityTypeConfiguration<PurchaseOrder> {
        public PurchaseOrderMap() {
            this.ToTable("purchaseorder");
            this.HasKey(c => c.Id);
        }
    }
}
