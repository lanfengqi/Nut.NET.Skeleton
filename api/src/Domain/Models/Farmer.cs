using Foundatio.Skeleton.Repositories.Model;
using System;

namespace Foundatio.Skeleton.Domain.Models {
    public class Farmer : IOwnedByOrganizationWithIdentity, IHaveDates {
        public string Id { get; set; }

        public string OrganizationId { get; set; }

        public string FarmerName { get; set; }

        public string Address { get; set; }

        public string Phone { get; set; }

        public string Contact { get; set; }

        public string ContactIdCardNumber { get; set; }

        public string ContactAddress { get; set; }

        public bool Deleted { get; set; }

        public string Notes { get; set; }

        public DateTime CreatedUtc { get; set; }

        public DateTime UpdatedUtc { get; set; }
    }
}
