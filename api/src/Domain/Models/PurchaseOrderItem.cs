using Foundatio.Skeleton.Repositories.Model;
using System;

namespace Foundatio.Skeleton.Domain.Models {
    public class PurchaseOrderItem : IIdentity, IHaveDates {
        public string Id { get; set; }

        public string Level { get; set; }

        public decimal Quantity { get; set; }

        public DateTime CreatedUtc { get; set; }

        public DateTime UpdatedUtc { get; set; }
    }
}
