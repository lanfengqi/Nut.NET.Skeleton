using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Repositories.Configuration;

namespace Foundatio.Skeleton.Domain.Repositories.Configuration {
    public class AttendanceMap : NutEntityTypeConfiguration<Attendance> {

        public AttendanceMap() {
            this.ToTable("Attendance");
            this.HasKey(a => a.Id);

            this.Property(c => c.OrganizationId).HasMaxLength(50);
            this.Property(c => c.UserId).HasMaxLength(50);
            this.Property(c => c.CarId).HasMaxLength(50);
            this.Property(c => c.Notes).HasMaxLength(500);
        }
    }
}
