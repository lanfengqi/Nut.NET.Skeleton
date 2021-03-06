using Foundatio.Caching;
using Foundatio.Logging;
using Foundatio.Metrics;
using Foundatio.Skeleton.Api.Models;
using Foundatio.Skeleton.Api.Models.Auth;
using Foundatio.Skeleton.Core.Extensions;
using Foundatio.Skeleton.Domain;
using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Domain.Repositories;
using Foundatio.Skeleton.Domain.Services;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace Foundatio.Skeleton.Api.Controllers {
    [RoutePrefix(API_PREFIX + "/auth")]
    public class AuthController : AppApiController {
        private readonly IUserRepository _userRepository;
        private readonly IUserPasswordRepository _userPasswordRepository;
        private readonly IOrganizationRepository _organizationRepository;
        private readonly ITokenRepository _tokenRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IMetricsClient _metricsClient;
        private readonly ITemplatedSmsService _templatedSmsService;
        private readonly ILogger _logger;
        private readonly ICacheClient _cacheClinet;

        private static bool _isFirstUserChecked;
        private const string _invalidPasswordMessage = "The password must be at least 8 characters long.";

        public AuthController(ILoggerFactory loggerFactory, IUserRepository userRepository, IUserPasswordRepository userPasswordRepository,
            IOrganizationRepository orgRepository, IMetricsClient metricsClient, ITemplatedSmsService templatedSmsService,
           ITokenRepository tokenRepository, IRoleRepository roleRepository, ICacheClient cacheClient) {
            _logger = loggerFactory?.CreateLogger<AuthController>() ?? NullLogger.Instance;
            _userRepository = userRepository;
            _userPasswordRepository = userPasswordRepository;
            _organizationRepository = orgRepository;
            _tokenRepository = tokenRepository;
            _templatedSmsService = templatedSmsService;
            _roleRepository = roleRepository;
            _metricsClient = metricsClient;
            _cacheClinet = cacheClient;
        }

        /// <summary>
        /// Login with a local username and password.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>An auth token.</returns>
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(TokenResponseModel))]
        [HttpPost]
        [Route("signin")]
        public async Task<IHttpActionResult> SignInAsync(SignInModel model) {
            if (String.IsNullOrWhiteSpace(model?.Phone))
                return BadRequest("Email Address is required.");

            if (String.IsNullOrWhiteSpace(model.Password))
                return BadRequest("Password is required.");

            User user;
            try {
                user = await _userRepository.GetByPhoneAsync(model.Phone);
            } catch (Exception) {
                return Unauthorized();
            }
            if (user == null || !user.IsActive)
                return BadRequest("user is not exist.Or use is active.");

            var loginFailCountCacheKey = string.Format("{0}-LoginFailCount", user.Id);
            var loginFailCount = (await _cacheClinet.GetAsync<int>(loginFailCountCacheKey))?.Value;
            if (loginFailCount.HasValue && loginFailCount.Value >= 5) {
                return BadRequest("More than 5 errors, please wait for 10 minutes to log in again");
            }

            var userPassword = await _userPasswordRepository.GetByUserIdAsync(user.Id);
            if (userPassword == null)
                return BadRequest("user password is not exist.");

            if (!userPassword.IsValidPassword(model.Password)) {
                loginFailCount++;
                await _cacheClinet.SetAsync(loginFailCountCacheKey, loginFailCount, TimeSpan.FromMinutes(10));
                return BadRequest(string.Format("Password Error.{0} more errors will lock the account", 6 - loginFailCount));
            }
            await _metricsClient.CounterAsync("User Login");

            return Ok(new TokenResponseModel { Token = await GetToken(user) });
        }

        [HttpGet]
        [Route("signout")]
        [Authorize(Roles = AuthorizationRoles.User)]
        public async Task<IHttpActionResult> SignOutAsync() {
            if (User.IsTokenAuthType())
                return Ok();
            string id = User.GetLoggedInUsersTokenId();
            if (string.IsNullOrEmpty(id))
                return Ok();
            try {
                await _tokenRepository.RemoveAsync(id);
            } catch (Exception ex) {
                throw ex;
            }
            return Ok();
        }


        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(TokenResponseModel))]
        [HttpPost]
        [Route("signup")]
        public async Task<IHttpActionResult> SignUpAsync(SignupModel model) {
            if (!Settings.Current.EnableAccountCreation)
                return BadRequest("Sorry,this is not accepting new accountsa at this time.");

            if (string.IsNullOrWhiteSpace(model?.Phone))
                return BadRequest("Phone is required.");

            if (string.IsNullOrWhiteSpace(model?.Password))
                return BadRequest("Password is required.");

            User user;

            try {
                user = await _userRepository.GetByPhoneAsync(model.Phone);
            } catch {
                return BadRequest("An error occured.");
            }

            if (user != null)
                return BadRequest("The Phone is already!");

            user = new Domain.Models.User {
                CreatedUtc = DateTime.UtcNow,
                UpdatedUtc = DateTime.UtcNow,
                IsActive = true,
                FullName = model.Name ?? model.Phone,
                Phone = model.Phone,
                IsPhoneVerified = false,
                PhoneNotificationsEnabled = false,
                Id = Guid.NewGuid().ToString("N")
            };

            user.CreateVerifyPhoneToken();
            var userPassword = new UserPassword {
                Id = Guid.NewGuid().ToString("N"),
                CreatedUtc = DateTime.UtcNow,
                UserId = user.Id
            };

            userPassword.ResetPasswordResetToken();

            if (!IsValidPassword(model.Password))
                return BadRequest(_invalidPasswordMessage);

            userPassword.Salt = StringUtils.GetRandomString(16);
            userPassword.Password = model.Password.ToSaltedHash(userPassword.Salt);

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
            await _userRepository.AddAsync(user).AnyContext();
            await _userPasswordRepository.AddAsync(userPassword).AnyContext();

            await AddGlobaAdminRoleIfFirstUser(user);
            await _userRepository.SaveAsync(user).AnyContext();

            //_templatedSmsService.SendPhoneVerifyNotification(user);
            await _metricsClient.CounterAsync("User Sign Up");
            return Ok(new TokenResponseModel { Token = await GetToken(user) });
        }


        [HttpPost]
        [Route("change-pasword")]
        [Authorize(Roles = AuthorizationRoles.User)]
        public async Task<IHttpActionResult> ChangePaswordAsync(ChangePasswordModel model) {
            if (model == null || !IsValidPassword(model.Password))
                return BadRequest(_invalidPasswordMessage);

            var userPassword = await _userPasswordRepository.GetByUserIdAsync(currentUser.Id);

            if (!String.IsNullOrWhiteSpace(userPassword.Password)) {
                if (String.IsNullOrWhiteSpace(model.CurrentPassword))
                    return BadRequest("The current password is incorrect.");

                string encodedPassword = model.CurrentPassword.ToSaltedHash(userPassword.Salt);
                if (!String.Equals(encodedPassword, userPassword.Password))
                    return BadRequest("The current password is incorrect.");
            }

            await ChangePassword(userPassword, model.Password);

            return Ok();
        }

        [HttpGet]
        [Route("forgot-password")]
        public async Task<IHttpActionResult> ForgotPasswordAsync(string phone) {
            //var email = new Email { Address = emailAddress };
            //var validator = new Domain.Validators.EmailValidator();
            //if (!validator.TryValidate(email))
            //    return BadRequest("Please specify a valid Email address");
            var passwordResetToken = "";

            var user = await _userRepository.GetByPhoneAsync(phone);
            if (user != null) {
                var userPassword = await _userPasswordRepository.GetByUserIdAsync(user.Id);
                if (userPassword != null) {

                    if (userPassword.PasswordResetTokenCreated < DateTime.UtcNow.Subtract(TimeSpan.FromDays(1))) {
                        userPassword.PasswordResetToken = StringUtils.GetNewToken();
                        userPassword.PasswordResetTokenCreated = DateTime.UtcNow;
                    } else {
                        userPassword.PasswordResetTokenCreated = DateTime.UtcNow;
                    }
                    passwordResetToken = userPassword.PasswordResetToken;

                    await _userPasswordRepository.SaveAsync(userPassword);
                }
            }
            return Ok(passwordResetToken);
        }

        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(TokenResponseModel))]
        [HttpPost]
        [Route("reset-pasword")]
        [Authorize(Roles = AuthorizationRoles.User)]
        public async Task<IHttpActionResult> ResetPaswordAsync(ResetPasswordModel model) {
            if (String.IsNullOrEmpty(model?.PasswordResetToken))
                return BadRequest("Invalid password reset token.");

            var userPassword = await _userPasswordRepository.GetByPasswordResetTokenAsync(model.PasswordResetToken);
            if (userPassword == null)
                return BadRequest("Invalid password reset token.");

            if (!userPassword.HasValidPasswordResetTokenExpiration())
                return BadRequest("Password Reset Token has expired.");

            if (!IsValidPassword(model.Password))
                return BadRequest(_invalidPasswordMessage);

            userPassword.User.MarkPhoneVerified();

            await ChangePassword(userPassword, model.Password);

            return Ok(new TokenResponseModel { Token = await GetToken(userPassword.User) });
        }

        private Task ChangePassword(UserPassword userPassword, string password) {
            if (String.IsNullOrWhiteSpace(userPassword.Salt))
                userPassword.Salt = StringUtils.GetRandomString(16);

            userPassword.Password = password.ToSaltedHash(userPassword.Salt);
            userPassword.ResetPasswordResetToken();

            return _userPasswordRepository.SaveAsync(userPassword);
        }

        private async Task AddGlobaAdminRoleIfFirstUser(User user) {
            if (_isFirstUserChecked)
                return;

            bool isFirstUser = await _userRepository.CountAsync() == 1;
            if (isFirstUser)
                await user.AddedGlobalAdminRole(_roleRepository);

            _isFirstUserChecked = true;
        }

        private async Task<string> GetToken(User user) {
            var token = await _tokenRepository.GetOrCreateUserToken(user.Id, user.OrganizationId);
            return token.Id;
        }

        public static bool IsValidPassword(string password) {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            return password.Length >= 8;
        }
    }
}
