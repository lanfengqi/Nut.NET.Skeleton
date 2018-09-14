using Foundatio.Skeleton.Repositories.Model;
using System;

namespace Foundatio.Skeleton.Domain.Models {
    public class Car : IOwnedByOrganizationWithIdentity, IHaveDates {
        public string Id { get; set; }

        public string OrganizationId { get; set; }

        public string Notes { get; set; }

        public string CarNum { get; set; }

        public string CarType { get; set; }

        public decimal EnteredWeight { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedUtc { get; set; }

        public DateTime UpdatedUtc { get; set; }
    }
}
