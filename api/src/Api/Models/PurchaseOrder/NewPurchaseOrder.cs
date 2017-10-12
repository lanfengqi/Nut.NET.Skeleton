using System.Collections.Generic;

namespace Foundatio.Skeleton.Api.Models {
    public class NewPurchaseOrder {

        public string PurchaseOrderNo { get; set; }

        public string PurchaseOrderMan { get; set; }

        public decimal Quantity { get; set; }
        
        public virtual IList<NewPurchaseOrderItem> PurchaseOrderItems { get; set; }
    }

    public class NewPurchaseOrderItem {

        public string Level { get; set; }

        public decimal Quantity { get; set; }
    }
}
