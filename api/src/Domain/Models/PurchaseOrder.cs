using Foundatio.Skeleton.Repositories.Model;
using System;
using System.Collections.Generic;

namespace Foundatio.Skeleton.Domain.Models {
    public class PurchaseOrder : IIdentity, IHaveDates {
        public string Id { get; set; }

        public string PurchaseOrderNo { get; set; }

        public string PurchaseOrderMan { get; set; }

        public decimal Quantity { get; set; }

        public PurchaseOrderStatus Stat { get; set; }

        public DateTime CreatedUtc { get; set; }

        public DateTime UpdatedUtc { get; set; }

        public virtual ICollection<PurchaseOrderItem> PurchaseOrderItems { get; set; }
    }
}
