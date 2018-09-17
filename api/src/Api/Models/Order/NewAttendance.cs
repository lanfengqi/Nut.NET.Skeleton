using System;

namespace Foundatio.Skeleton.Api.Models {
    public class NewAttendance {
        public DateTime AttendanceDate { get; set; }

        public string CarId { get; set; }

        public string Notes { get; set; }

        public decimal Lat { get; set; }

        public decimal Lng { get; set; }
    }
}
