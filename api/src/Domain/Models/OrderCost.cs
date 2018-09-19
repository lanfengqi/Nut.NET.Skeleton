using Foundatio.Skeleton.Repositories.Model;
using System;

namespace Foundatio.Skeleton.Domain.Models {
    public class OrderCost : IIdentity, IHaveDates {

        public string Id { get; set; }

        public string OrderId { get; set; }

        public string CostSystemName { get; set; }

        public decimal EnteredMoney { get; set; }

        public string Notes { get; set; }

        public DateTime CreatedUtc { get; set; }

        public DateTime UpdatedUtc { get; set; }

        public virtual Order Order { get; set; }
    }
}
