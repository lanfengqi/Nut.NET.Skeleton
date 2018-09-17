using AutoMapper;
using Foundatio.Logging;
using Foundatio.Skeleton.Api.Extensions;
using Foundatio.Skeleton.Api.Models;
using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Domain.Repositories;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace Foundatio.Skeleton.Api.Controllers {

    [RoutePrefix(API_PREFIX + "/attendances")]
    [Authorize(Roles = AuthorizationRoles.User)]
    public class AttendanceController : RepositoryApiController<IAttendanceRepository, Attendance, ViewAttendance, NewAttendance, NewAttendance> {

        public AttendanceController(
            ILoggerFactory loggerFactory,
            IAttendanceRepository attendanceRepository,
            IMapper mapper) : base(loggerFactory, attendanceRepository, mapper) {

        }

        [HttpGet]
        [Route("admin")]
        [Authorize(Roles = AuthorizationRoles.GlobalAdmin)]
        public async Task<IHttpActionResult> GetForAdminsAsync(string userId = "", DateTime? attendanceDate = null, int page = 1, int limit = 10) {

            page = GetPage(page);
            limit = GetLimit(limit);

            var attendances = await _repository.SearchAttendanceAsync(userId, attendanceDate, page, limit);
            return OkWithResourceLinks(attendances, attendances.TotalPages > page, page, attendances.TotalCount);
        }

        [HttpGet]
        [Route("GetForCurrentUser")]
        public async Task<IHttpActionResult> GetForUserAsync(DateTime attendanceDate) {
            if (base.currentUser == null)
                return base.NotFound();
            var user = base.currentUser;

            attendanceDate = DateTime.Parse(attendanceDate.ToShortDateString());

            var attendances = await _repository.FindAsync(x => x.UserId == user.Id && x.AttendanceDate == attendanceDate);
            var attendance = attendances.FirstOrDefault();
            if (attendance == null)
                return base.NotFound();

            var model = await Map<ViewAttendance>(attendance);
            return Ok(model);
        }

        [HttpGet]
        [Route("{id:objectid}", Name = "GetAttendanceById")]
        public override Task<IHttpActionResult> GetByIdAsync(string id) {
            return base.GetByIdAsync(id);
        }

        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(NewAttendance))]
        [HttpPost]
        [Route]
        public override Task<IHttpActionResult> PostAsync(NewAttendance value) {
            return base.PostAsync(value);
        }

        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(NewAttendance))]
        [HttpPatch]
        [Route("{id:objectid}")]
        public override Task<IHttpActionResult> PatchAsync(string id, NewAttendance value, long? version = null) {
            return base.PatchAsync(id, value, version);
        }

        [HttpDelete]
        [Route("{id:objectid}")]
        public override Task<IHttpActionResult> DeleteAsync(string id) {
            return base.DeleteAsync(id);
        }

        protected override Task<Domain.Models.Attendance> AddModelAsync(Domain.Models.Attendance value) {
            value.Id = Guid.NewGuid().ToString("N");
            value.CreatedUtc = DateTime.UtcNow;
            value.OrganizationId = Request.GetSelectedOrganizationId();
            value.UserId = Request.GetUser().Id;
            value.AttendanceDate = DateTime.Parse(value.AttendanceDate.ToShortDateString());

            return base.AddModelAsync(value);
        }

    }
}
