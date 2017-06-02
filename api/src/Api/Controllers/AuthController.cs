using Foundatio.Logging;
using Foundatio.Skeleton.Api.Models;
using Foundatio.Skeleton.Api.Models.Auth;
using Foundatio.Skeleton.Core.Extensions;
using Foundatio.Skeleton.Domain;
using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Domain.Repositories;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace Foundatio.Skeleton.Api.Controllers {
    [RoutePrefix(API_PREFIX + "/auth")]
    public class AuthController : AppApiController {
        private readonly IUserRepository _userRepository;
        private readonly IOrganizationRepository _organizationRepository;
        private readonly ITokenRepository _tokenRepository;
        private readonly ILogger _logger;

        private static bool _isFirstUserChecked;
        private const string _invalidPasswordMessage = "The password must be at least 8 characters long.";

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

        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(TokenResponseModel))]
        [HttpPost]
        [Route("signup")]
        public async Task<IHttpActionResult> SignUp(SignupModel model) {
            if (!Settings.Current.EnableAccountCreation)
                return BadRequest("Sorry,this is not accepting new accountsa at this time.");

            if (string.IsNullOrWhiteSpace(model?.Email))
                return BadRequest("Email address is required.");

            if (string.IsNullOrWhiteSpace(model?.Password))
                return BadRequest("Email address is required.");

            User user;

            try {
                user = await _userRepository.GetByEmailAddressAsync(model.Email);
            } catch {
                return BadRequest("An error occured.");
            }

            if (user != null)
                return BadRequest("The Email address is already!");

            user = new Domain.Models.User {
                CreatedUtc = DateTime.UtcNow,
                UpdatedUtc = DateTime.UtcNow,
                IsActive = true,
                FullName = model.Name ?? model.Email,
                EmailAddress = model.Email,
                IsEmailAddressVerified = false,
                EmailNotificationsEnabled = false,
                Id = Guid.NewGuid().ToString(),
            };
            user.CreateVerifyEmailAddressToken();
            user.ResetPasswordResetToken();

            await AddGlobaAdminRoleIfFirstUser(user);

            if (!IsValidPassword(model.Password))
                return BadRequest(_invalidPasswordMessage);

            user.Salt = StringUtils.GetRandomString(16);
            user.Password = model.Password.ToSaltedHash(user.Salt);

            if (!String.IsNullOrWhiteSpace(model.OrganizationName)) {
                Organization organization;

                try {
                    organization = await _organizationRepository.GetByNameAsync(model.OrganizationName);
                } catch {
                    return BadRequest("An error occured.");
                }

                if (organization == null) {
                    organization = new Organization {
                        CreatedUtc = DateTime.UtcNow,
                        UpdatedUtc = DateTime.UtcNow,
                        Id = Guid.NewGuid().ToString(),
                        IsVerified = true,
                        Version = 1,
                        Name = model.OrganizationName
                    };
                    await _organizationRepository.AddAsync(organization);
                    user.OrganizationId = organization.Id;
                } else {
                    user.OrganizationId = organization.Id;
                }
            }
            await _userRepository.AddAsync(user);

            return Ok(new TokenResponseModel { Token = await GetToken(user, user.OrganizationId) });
        }


        [HttpPost]
        [Route("change-pasword")]
        [Authorize(Roles = AuthorizationRoles.User)]
        public async Task<IHttpActionResult> ChangePasword(ChangePasswordModel model) {
            if (model == null || !IsValidPassword(model.Password))
                return BadRequest(_invalidPasswordMessage);

            if (!String.IsNullOrWhiteSpace(CurrentUser.Password)) {
                if (String.IsNullOrWhiteSpace(model.CurrentPassword))
                    return BadRequest("The current password is incorrect.");

                string encodedPassword = model.CurrentPassword.ToSaltedHash(CurrentUser.Salt);
                if (!String.Equals(encodedPassword, CurrentUser.Password))
                    return BadRequest("The current password is incorrect.");
            }

            await ChangePassword(CurrentUser, model.Password);

            return Ok();
        }

        [HttpGet]
        [Route("forgot-password")]
        public async Task<IHttpActionResult> ForgotPassword(string emailAddress) {
            var email = new Email { Address = emailAddress };
            var validator = new Domain.Validators.EmailValidator();
            if (!validator.TryValidate(email))
                return BadRequest("Please specify a valid Email address");

            var user = await _userRepository.GetByEmailAddressAsync(email.Address);
            if (user != null) {

                if(user.PasswordResetTokenCreated < DateTime.UtcNow.Subtract(TimeSpan.FromDays(1))) {
                    user.PasswordResetToken = StringUtils.GetNewToken();
                    user.PasswordResetTokenCreated = DateTime.UtcNow;
                } else {
                    user.PasswordResetTokenCreated = DateTime.UtcNow;
                }

                await _userRepository.SaveAsync(user);
            }
            return Ok();
        }

        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(TokenResponseModel))]
        [HttpPost]
        [Route("reset-pasword")]
        [Authorize(Roles = AuthorizationRoles.User)]
        public async Task<IHttpActionResult> ResetPasword(ResetPasswordModel model) {
            if (String.IsNullOrEmpty(model?.PasswordResetToken))
                return BadRequest("Invalid password reset token.");

            var user = await _userRepository.GetByPasswordResetTokenAsync(model.PasswordResetToken);
            if (user == null)
                return BadRequest("Invalid password reset token.");

            if(!user.HasValidPasswordResetTokenExpiration())
                return BadRequest("Password Reset Token has expired.");

            if (!IsValidPassword(model.Password))
                return BadRequest(_invalidPasswordMessage);

            user.MarkEmailAddressVerified();

            await ChangePassword(user, model.Password);

            return Ok(new TokenResponseModel { Token = await GetToken(user, user.OrganizationId) });
        }

        private Task ChangePassword(User user,string password) {
            if (String.IsNullOrWhiteSpace(user.Salt))
                user.Salt = StringUtils.GetRandomString(16);

            user.Password = password.ToSaltedHash(user.Salt);
            user.ResetPasswordResetToken();

            return _userRepository.SaveAsync(user);
        }

        private async Task AddGlobaAdminRoleIfFirstUser(User user) {
            if (_isFirstUserChecked)
                return;

            bool isFirstUser = await _userRepository.CountAsync() == 0;
            if (isFirstUser)
                user.AddGlobalAdminRole();

            _isFirstUserChecked = true;
        }

        private async Task<string> GetToken(User user, string organizationId) {
            var token = await _tokenRepository.GetOrCreateUserToken(user.Id, organizationId);
            return token.Id;
        }

        public static bool IsValidPassword(string password) {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            return password.Length >= 8;
        }
    }
}
