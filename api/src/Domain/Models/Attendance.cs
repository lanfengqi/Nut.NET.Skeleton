using Foundatio.Skeleton.Repositories.Model;
using System;

namespace Foundatio.Skeleton.Domain.Models {
    public class Attendance : IOwnedByOrganizationWithIdentity, IHaveCreatedDate, IMapPoint {
        public string Id { get; set; }

        public string OrganizationId { get; set; }

        public string UserId { get; set; }

        public DateTime AttendanceDate { get; set; }

        public string CarId { get; set; }

        public string Notes { get; set; }

        public decimal Lat { get; set; }

        public decimal Lng { get; set; }

        public DateTime CreatedUtc { get; set; }

        public virtual Car Car { get; set; }

        public virtual User User { get; set; }

    }
}
