using Foundatio.Skeleton.Repositories.Model;
using System;

namespace Foundatio.Skeleton.Domain.Models {
    public class Product : IIdentity, IHaveDates {

        public string Id { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public string Unit { get; set; }

        public decimal Specification { get; set; }

        public DateTime CreatedUtc { get; set; }

        public DateTime UpdatedUtc { get; set; }
    }
}
