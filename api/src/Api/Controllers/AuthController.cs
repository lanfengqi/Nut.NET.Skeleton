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
        private readonly IUserPasswordRepository _userPasswordRepository;
        private readonly IOrganizationRepository _organizationRepository;
        private readonly ITokenRepository _tokenRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly ILogger _logger;

        private static bool _isFirstUserChecked;

        public AuthController(ILoggerFactory loggerFactory, IUserRepository userRepository, IUserPasswordRepository userPasswordRepository,
            IOrganizationRepository orgRepository,
           ITokenRepository tokenRepository, IRoleRepository roleRepository) {
            _logger = loggerFactory?.CreateLogger<AuthController>() ?? NullLogger.Instance;
            _userRepository = userRepository;
            _userPasswordRepository = userPasswordRepository;
            _organizationRepository = orgRepository;
            _tokenRepository = tokenRepository;
            _roleRepository = roleRepository;

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
                return BadRequest((await T("Auth.Login.Email.Required")).Text);

            if (String.IsNullOrWhiteSpace(model.Password))
                return BadRequest((await T("Auth.Login.Password.Required")).Text);

            User user;
            try {
                user = await _userRepository.GetByEmailAddressAsync(model.Email);
            } catch (Exception) {
                return Unauthorized();
            }

            if (user == null || !user.IsActive)
                return BadRequest((await T("Auth.Login.User.NotExist")).Text);

            var userPassword = await _userPasswordRepository.GetByUserIdAsync(user.Id);
            if (userPassword == null)
                return BadRequest((await T("Auth.Login.User.PaswordNotExist")).Text);

            if (!userPassword.IsValidPassword(model.Password)) {
                return BadRequest((await T("Auth.Login.Password.Error")).Text);
            }

            return Ok(new TokenResponseModel { Token = await GetToken(user) });
        }

        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(TokenResponseModel))]
        [HttpPost]
        [Route("signup")]
        public async Task<IHttpActionResult> SignUp(SignupModel model) {
            if (!Settings.Current.EnableAccountCreation)
                return BadRequest((await T("Auth.SignUp.EnableAccountCreation")).Text);

            if (string.IsNullOrWhiteSpace(model?.Email))
                return BadRequest((await T("Auth.SignUp.Email.Required")).Text);

            if (string.IsNullOrWhiteSpace(model?.Password))
                return BadRequest((await T("Auth.SignUp.Password.Required")).Text);

            User user;

            try {
                user = await _userRepository.GetByEmailAddressAsync(model.Email);
            } catch {
                return BadRequest((await T("Common.Error")).Text);
            }

            if (user != null)
                return BadRequest((await T("Auth.SignUp.Email.IsExist")).Text);

            user = new Domain.Models.User {
                CreatedUtc = DateTime.UtcNow,
                UpdatedUtc = DateTime.UtcNow,
                IsActive = true,
                FullName = model.Name ?? model.Email,
                EmailAddress = model.Email,
                IsEmailAddressVerified = false,
                EmailNotificationsEnabled = false,
                Id = Guid.NewGuid().ToString("N"),
            };
            user.CreateVerifyEmailAddressToken();
            var userPassword = new UserPassword {
                Id = Guid.NewGuid().ToString("N"),
                CreatedUtc = DateTime.UtcNow,
                UserId = user.Id
            };
            userPassword.ResetPasswordResetToken();

            await AddGlobaAdminRoleIfFirstUser(user);

            if (!IsValidPassword(model.Password))
                return BadRequest((await T("Common.Password.Invalid")).Text);

            userPassword.Salt = StringUtils.GetRandomString(16);
            userPassword.Password = model.Password.ToSaltedHash(userPassword.Salt);

            if (!String.IsNullOrWhiteSpace(model.OrganizationName)) {
                Organization organization;

                try {
                    organization = await _organizationRepository.GetByNameAsync(model.OrganizationName);
                } catch {
                    return BadRequest((await T("Common.Error")).Text);
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
            await _userPasswordRepository.AddAsync(userPassword);

            return Ok(new TokenResponseModel { Token = await GetToken(user) });
        }


        [HttpPost]
        [Route("change-pasword")]
        [Authorize(Roles = AuthorizationRoles.User)]
        public async Task<IHttpActionResult> ChangePasword(ChangePasswordModel model) {
            if (model == null || !IsValidPassword(model.Password))
                return BadRequest((await T("Common.Password.Invalid")).Text);

            var userPassword = await _userPasswordRepository.GetByUserIdAsync(CurrentUser.Id);

            if (!String.IsNullOrWhiteSpace(userPassword.Password)) {
                if (String.IsNullOrWhiteSpace(model.CurrentPassword))
                    return BadRequest((await T("Auth.ChangePasword.Password.Incorrect")).Text);

                string encodedPassword = model.CurrentPassword.ToSaltedHash(userPassword.Salt);
                if (!String.Equals(encodedPassword, userPassword.Password))
                    return BadRequest((await T("Auth.ChangePasword.Password.Incorrect")).Text);
            }

            await ChangePassword(userPassword, model.Password);

            return Ok();
        }

        [HttpGet]
        [Route("forgot-password")]
        public async Task<IHttpActionResult> ForgotPassword(string emailAddress) {
            var email = new Email { Address = emailAddress };
            var validator = new Domain.Validators.EmailValidator();
            if (!validator.TryValidate(email))
                return BadRequest((await T("Auth.ForgotPassword.Email.Valid")).Text);

            var user = await _userRepository.GetByEmailAddressAsync(email.Address);
            if (user != null) {
                var userPassword = await _userPasswordRepository.GetByUserIdAsync(user.Id);
                if (userPassword != null) {

                    if (userPassword.PasswordResetTokenCreated < DateTime.UtcNow.Subtract(TimeSpan.FromDays(1))) {
                        userPassword.PasswordResetToken = StringUtils.GetNewToken();
                        userPassword.PasswordResetTokenCreated = DateTime.UtcNow;
                    } else {
                        userPassword.PasswordResetTokenCreated = DateTime.UtcNow;
                    }

                    await _userPasswordRepository.SaveAsync(userPassword);
                }
            }
            return Ok();
        }

        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(TokenResponseModel))]
        [HttpPost]
        [Route("reset-pasword")]
        [Authorize(Roles = AuthorizationRoles.User)]
        public async Task<IHttpActionResult> ResetPasword(ResetPasswordModel model) {
            if (String.IsNullOrEmpty(model?.PasswordResetToken))
                return BadRequest((await T("Auth.ResetPasword.Token.Invalid")).Text);

            var userPassword = await _userPasswordRepository.GetByPasswordResetTokenAsync(model.PasswordResetToken);
            if (userPassword == null)
                return BadRequest((await T("Auth.ResetPasword.Token.Invalid")).Text);

            if (!userPassword.HasValidPasswordResetTokenExpiration())
                return BadRequest((await T("Auth.ResetPasword.Token.Expired")).Text);

            if (!IsValidPassword(model.Password))
                return BadRequest((await T("Common.Password.Invalid")).Text);

            userPassword.User.MarkEmailAddressVerified();

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

            bool isFirstUser = await _userRepository.CountAsync() == 0;
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
