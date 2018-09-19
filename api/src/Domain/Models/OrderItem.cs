using Foundatio.Skeleton.Repositories.Model;
using System;

namespace Foundatio.Skeleton.Domain.Models {
    public class OrderItem : IIdentity, IHaveDates {

        public string Id { get; set; }

        public string OrderId { get; set; }

        public int Quantity { get; set; }

        public decimal EnteredPrice { get; set; }

        public decimal EnteredWeight { get; set; }

        public decimal EnteredGrossWeight { get; set; }

        public decimal TtemTotal { get; set; }

        public decimal Lat { get; set; }

        public decimal Lng { get; set; }

        public DateTime CreatedUtc { get; set; }

        public DateTime UpdatedUtc { get; set; }

        public virtual Order Order { get; set; }
    }
}
