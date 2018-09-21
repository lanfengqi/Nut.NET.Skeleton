using System;

namespace Foundatio.Skeleton.Api.Models {
    public class NewCar {

        public string CarNum { get; set; }

        public string CarType { get; set; }

        public string CarOwner { get; set; }

        public string UseProperty { get; set; }

        public string Address { get; set; }

        public string BrandModel { get; set; }

        public string VIN { get; set; }

        public string EngineNo { get; set; }

        public DateTime RegisterDate { get; set; }

        public DateTime IssueDate { get; set; }

        public string FileNumber { get; set; }

        public decimal TotalWeight { get; set; }

        public decimal EnteredWeight { get; set; }

        public string OutlineSize { get; set; }

        public string Notes { get; set; }

    }
}
