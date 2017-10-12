using Foundatio.Skeleton.Repositories.Model;
using System;
using System.Collections.Generic;

namespace Foundatio.Skeleton.Api.Models {
    public class ViewPurchaseOrder : IIdentity {

        public string Id { get; set; }

        public string PurchaseOrderNo { get; set; }

        public string PurchaseOrderMan { get; set; }

        public decimal Quantity { get; set; }

        public int Stat { get; set; }

        public DateTime CreatedUtc { get; set; }

        public DateTime UpdatedUtc { get; set; }

        public virtual IList<ViewPurchaseOrderItem> PurchaseOrderItems { get; set; }
    }

    public class ViewPurchaseOrderItem : IIdentity {

        public string Id { get; set; }

        public string PurchaseOrderId { get; set; }

        public string Level { get; set; }

        public decimal Quantity { get; set; }

        public DateTime CreatedUtc { get; set; }

        public DateTime UpdatedUtc { get; set; }
    }
}
