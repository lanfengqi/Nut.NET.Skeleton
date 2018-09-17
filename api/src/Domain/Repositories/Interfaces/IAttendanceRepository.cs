using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Foundatio.Skeleton.Domain.Repositories {
    public interface IAttendanceRepository : IEFRepository<Attendance> {

        Task<PagedList<Attendance>> SearchAttendanceAsync(string userId = "", DateTime? attendanceDate = null, int page = 1, int limit = 10);
    }
}
