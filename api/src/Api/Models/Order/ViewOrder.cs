using Foundatio.Skeleton.Repositories.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Foundatio.Skeleton.Api.Models {
    public class ViewOrder : IIdentity {

        public string Id { get; set; }

        public string OrderNo { get; set; }

        public string CustomerId { get; set; }

        public decimal OrderTotal { get; set; }

        public DateTime PickupTime { get; set; }

        public int Stat { get; set; }

        public DateTime CreatedUtc { get; set; }

        public DateTime UpdatedUtc { get; set; }

        public virtual IList<ViewOrderItem> OrderItems { get; set; }
    }

    public class ViewOrderItem : IIdentity {

        public string Id { get; set; }

        public string OrderId { get; set; }

        public string ProductId { get; set; }

        public decimal Quantity { get; set; }

        public decimal Weight { get; set; }

        public decimal SalePrice { get; set; }

        public decimal SaleTotal { get; set; }

        public DateTime CreatedUtc { get; set; }

        public DateTime UpdatedUtc { get; set; }
    }
}
