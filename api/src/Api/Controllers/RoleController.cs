using AutoMapper;
using Foundatio.Logging;
using Foundatio.Messaging;
using Foundatio.Skeleton.Api.Models;
using Foundatio.Skeleton.Core.JsonPatch;
using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Domain.Repositories;
using Foundatio.Skeleton.Repositories.Model;
using Swashbuckle.Swagger.Annotations;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace Foundatio.Skeleton.Api.Controllers {
    [RoutePrefix(API_PREFIX + "/roles")]
    [Authorize(Roles = AuthorizationRoles.User)]
    public class RoleController : RepositoryApiController<IRoleRepository, Role, ViewRole, ViewRole, ViewRole> {
        private readonly IUserRepository _userRepository;
        private readonly IMessagePublisher _messagePublisher;

        public RoleController(IRoleRepository roleRepository,
            ILoggerFactory loggerFactory,
            IUserRepository userRepository,
            IMessagePublisher messagePublisher,
            IMapper mapper)
            : base(loggerFactory, roleRepository, mapper) {
            _userRepository = userRepository;
            _messagePublisher = messagePublisher;
        }

        [HttpGet]
        [Route("admin")]
        [Authorize(Roles = AuthorizationRoles.GlobalAdmin)]
        public async Task<IHttpActionResult> GetForAdmins(int page = 1, int limit = 10) {

            page = GetPage(page);
            limit = GetLimit(limit);

            var organizations = await _repository.FindAsync(x => x.SystemName != "", new PagingOptions { Page = page, Limit = limit });

            return OkWithResourceLinks(organizations, organizations.TotalPages > page, page, organizations.TotalCount);
        }

        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ViewRole))]
        [HttpGet]
        [Route("{id:objectid}", Name = "GetRoleById")]
        public override async Task<IHttpActionResult> GetById(string id) {
            var role = await GetModel(id);
            if (role == null)
                return NotFound();

            var viewRole = await Map<ViewRole>(role);
            return Ok(viewRole);
        }

        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ViewRole))]
        [HttpPost]
        [Route]
        public override Task<IHttpActionResult> PostAsync(ViewRole value) {
            return base.PostAsync(value);
        }

        [HttpDelete]
        [Route("{id:objectid}")]
        public override Task<IHttpActionResult> DeleteAsync(string id) {
            return base.DeleteAsync(id);
        }

    }
}
