using Foundatio.Skeleton.Repositories.Model;
using System;

namespace Foundatio.Skeleton.Domain.Models {
    public class PurchaseCar : IOwnedByOrganizationWithIdentity, IHaveDates, IMapPoint {

        public string Id { get; set; }

        public string OrganizationId { get; set; }

        public string UserId { get; set; }

        public int Quantity { get; set; }

        public decimal EnteredPrice { get; set; }

        public decimal EnteredWeight { get; set; }

        public decimal EnteredGrossWeight { get; set; }

        public decimal CarTotal { get; set; }

        public string Notes { get; set; }

        public decimal Lat { get; set; }

        public decimal Lng { get; set; }

        public DateTime CreatedUtc { get; set; }

        public DateTime UpdatedUtc { get; set; }
    }
}
