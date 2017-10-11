using Foundatio.Skeleton.Repositories.Model;
using System;
using System.Collections.Generic;

namespace Foundatio.Skeleton.Api.Models {
    public class NewOrder {

        public string OrderNo { get; set; }

        public string CustomerId { get; set; }

        public decimal OrderTotal { get; set; }

        public DateTime PickupTime { get; set; }

        public virtual IList<NewOrderItem> OrderItems { get; set; }
    }

    public class NewOrderItem {

        public string ProductId { get; set; }

        public decimal Quantity { get; set; }

        public decimal Weight { get; set; }

        public decimal SalePrice { get; set; }

        public decimal SaleTotal { get; set; }
    }
}
