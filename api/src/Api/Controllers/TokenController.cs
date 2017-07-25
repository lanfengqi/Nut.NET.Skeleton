using AutoMapper;
using Foundatio.Logging;
using Foundatio.Skeleton.Api.Extensions;
using Foundatio.Skeleton.Api.Models;
using Foundatio.Skeleton.Core.Extensions;
using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Domain.Repositories;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace Foundatio.Skeleton.Api.Controllers {
    [RoutePrefix(API_PREFIX + "/tokens")]
    [Authorize(Roles = AuthorizationRoles.User)]
    public class TokenController : RepositoryApiController<ITokenRepository, Domain.Models.Token, ViewToken, NewToken, Domain.Models.Token> {

        public TokenController(
            ILoggerFactory loggerFactory,
            ITokenRepository tokenRepository,  
            IMapper mapper) : base(loggerFactory, tokenRepository, mapper) {
        }


        [HttpGet]
        [Route("{id:token}", Name = "GetTokenById")]
        public override Task<IHttpActionResult> GetById(string id) {
            return base.GetById(id);
        }

        protected override async Task<Domain.Models.Token> GetModel(string id) {
            if (String.IsNullOrEmpty(id))
                return null;

            var model = await _repository.GetByIdAsync(id);
            if (model == null)
                return null;

            if (!String.IsNullOrEmpty(model.OrganizationId) && model.OrganizationId != GetSelectedOrganizationId())
                return null;

            if (!String.IsNullOrEmpty(model.UserId) && model.UserId != Request.GetUser().Id)
                return null;

            if (model.Type != TokenType.Access)
                return null;

            return model;
        }

        protected override async Task<PermissionResult> CanAddAsync(Domain.Models.Token value) {
            if (String.IsNullOrEmpty(value.OrganizationId))
                return PermissionResult.Deny;

            return await base.CanAddAsync(value);
        }

        protected override Task<Domain.Models.Token> AddModelAsync(Domain.Models.Token value) {
            value.Id = StringUtils.GetNewToken();
            value.CreatedUtc = value.UpdatedUtc = DateTime.UtcNow;
            value.Type = TokenType.Access;
            value.CreatedBy = Request.GetUser().Id;
            value.OrganizationId = Request.GetSelectedOrganizationId();

            return base.AddModelAsync(value);
        }

    }
}
