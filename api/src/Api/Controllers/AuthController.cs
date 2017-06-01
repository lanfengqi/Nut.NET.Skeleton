using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Exceptionless;
using Exceptionless.DateTimeExtensions;
using FluentValidation;
using Foundatio.Skeleton.Api.Extensions;
using Foundatio.Skeleton.Core.Extensions;
using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Domain.Repositories;
using Foundatio.Skeleton.Domain.Services;
using Foundatio.Logging;
using Foundatio.Skeleton.Api.Models;
using Foundatio.Skeleton.Api.Models.Auth;
using Foundatio.Skeleton.Api.Utility;
using Foundatio.Skeleton.Domain;
using OAuth2.Client;
using OAuth2.Client.Impl;
using OAuth2.Configuration;
using OAuth2.Infrastructure;
using OAuth2.Models;
using Swashbuckle.Swagger.Annotations;

namespace Foundatio.Skeleton.Api.Controllers {
    [RoutePrefix(API_PREFIX + "/auth")]
    public class AuthController : AppApiController {
        private readonly IUserRepository _userRepository;
        private readonly IOrganizationRepository _organizationRepository;
        private readonly ITokenRepository _tokenRepository;
        private readonly ILogger _logger;


        public AuthController(ILoggerFactory loggerFactory, IUserRepository userRepository, IOrganizationRepository orgRepository,
           ITokenRepository tokenRepository) {
            _logger = loggerFactory?.CreateLogger<AuthController>() ?? NullLogger.Instance;
            _userRepository = userRepository;
            _organizationRepository = orgRepository;
            _tokenRepository = tokenRepository;
        }

        /// <summary>
        /// Login with a local username and password.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>An auth token.</returns>
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(TokenResponseModel))]
        [HttpPost]
        [Route("login")]
        public async Task<IHttpActionResult> Login(LoginModel model) {
            if (String.IsNullOrWhiteSpace(model?.Email))
                return BadRequest("Email Address is required.");

            if (String.IsNullOrWhiteSpace(model.Password))
                return BadRequest("Password is required.");

            User user;
            try {
                user = await _userRepository.GetByEmailAddressAsync(model.Email);
            } catch (Exception) {
                return Unauthorized();
            }

            if (user == null || !user.IsActive)
                return Unauthorized();

            if (!user.IsValidPassword(model.Password)) {
                return Unauthorized();
            }

            return Ok(new TokenResponseModel { Token = await GetToken(user, user.OrganizationId) });
        }

        private async Task<string> GetToken(User user, string organizationId) {
            var token = await _tokenRepository.GetOrCreateUserToken(user.Id, organizationId);
            return token.Id;
        }
    }
}
