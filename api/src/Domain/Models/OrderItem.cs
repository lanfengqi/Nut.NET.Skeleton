using Foundatio.Skeleton.Repositories.Model;
using System;

namespace Foundatio.Skeleton.Domain.Models {
    public class OrderItem : IIdentity, IHaveDates {

        public string Id { get; set; }

        public string OrderId { get; set; }

        public string ProductId { get; set; }

        public decimal Quantity { get; set; }

        public decimal Weight { get; set; }

        public decimal SalePrice { get; set; }

        public decimal SaleTotal { get; set; }

        public DateTime CreatedUtc { get; set; }

        public DateTime UpdatedUtc { get; set; }

        public virtual Order Order { get; set; }

        public virtual Product Product { get; set; }

    }
}
