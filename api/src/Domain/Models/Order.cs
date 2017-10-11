using Foundatio.Skeleton.Repositories.Model;
using System;
using System.Collections.Generic;

namespace Foundatio.Skeleton.Domain.Models {
    public class Order : IIdentity, IHaveDates {

        public string Id { get; set; }

        public string OrderNo { get; set; }

        public string CustomerId { get; set; }

        public decimal OrderTotal { get; set; }

        public DateTime PickupTime { get; set; }

        public int Stat { get; set; }

        public DateTime CreatedUtc { get; set; }

        public DateTime UpdatedUtc { get; set; }

        public virtual Customer Customer { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}
