using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Repositories;
using Foundatio.Skeleton.Repositories.Model;
using System;
using System.Threading.Tasks;

namespace Foundatio.Skeleton.Domain.Repositories {
    public class AttendanceRepository : EFRepositoryBase<Attendance>, IAttendanceRepository {

        public AttendanceRepository(IEFRepositoryContext efRepositoryContext)
            : base(efRepositoryContext, null) {

        }

        public async Task<PagedList<Attendance>> SearchAttendanceAsync(string userId = "", DateTime? attendanceDate = null, int page = 1, int limit = 10) {

            if (!attendanceDate.HasValue)
                attendanceDate = DateTime.Parse(DateTime.UtcNow.ToShortDateString());

            var attendance = await this.FindAsync(x => (x.UserId == userId || string.IsNullOrEmpty(userId)) && attendanceDate == attendanceDate.Value,
                new PagingOptions { Page = page, Limit = limit });
            return attendance;
        }
    }
}
