using Foundatio.Skeleton.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using IIdentity = System.Security.Principal.IIdentity;

namespace Foundatio.Skeleton.Domain.Services {
    public static class IdentityService {
        public const string TokenAuthenticationType = "Token";
        public const string UserAuthenticationType = "User";
        public const string UserIdClaim = "UserId";
        public const string OrganizationIdClaim = "OrganizationId";
        public const string LoggedInUserTokenId = "LoggedInUserTokenId";

        public static ClaimsIdentity ToIdentity(this Token token) {
            if (token == null || token.Type != TokenType.Access)
                return WindowsIdentity.GetAnonymous();

            if (!String.IsNullOrEmpty(token.UserId))
                throw new ApplicationException("Can't create token type identity for user token.");

            var claims = new List<Claim> {
                new Claim(ClaimTypes.NameIdentifier, token.Id),
                new Claim(OrganizationIdClaim, token.OrganizationId)
            };

            return new ClaimsIdentity(claims, TokenAuthenticationType);
        }

        public static ClaimsIdentity ToIdentity(this User user, Token token) {
            if (user == null)
                return WindowsIdentity.GetAnonymous();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.EmailAddress),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                 new Claim(OrganizationIdClaim, user.OrganizationId)
            };

            if (token != null) {
                claims.Add(new Claim(LoggedInUserTokenId, token.Id));
            }

            var userRoles = user.Roles.Select(x => x.SystemName).ToList();

            if (userRoles.Any()) {
                // add implied scopes
                if (userRoles.Contains(AuthorizationRoles.GlobalAdmin))
                    userRoles.Add(AuthorizationRoles.User);

                if (userRoles.Contains(AuthorizationRoles.User))
                    userRoles.Add(AuthorizationRoles.Client);

                claims.AddRange(userRoles.Select(scope => new Claim(ClaimTypes.Role, scope)));
            } else {
                claims.Add(new Claim(ClaimTypes.Role, AuthorizationRoles.Client));
                claims.Add(new Claim(ClaimTypes.Role, AuthorizationRoles.User));
            }

            return new ClaimsIdentity(claims, UserAuthenticationType);
        }

        public static AuthType GetAuthType(this IPrincipal principal) {
            if (principal?.Identity == null || !principal.Identity.IsAuthenticated)
                return AuthType.Anonymous;

            return IsTokenAuthType(principal) ? AuthType.Token : AuthType.User;
        }

        public static bool IsTokenAuthType(this IPrincipal principal) {
            var identity = GetClaimsIdentity(principal);
            if (identity == null)
                return false;

            return identity.AuthenticationType == TokenAuthenticationType;
        }

        public static bool IsUserAuthType(this IPrincipal principal) {
            var identity = GetClaimsIdentity(principal);
            if (identity == null)
                return false;

            return identity.AuthenticationType == UserAuthenticationType;
        }

        public static ClaimsPrincipal GetClaimsPrincipal(this IPrincipal principal) {
            return principal as ClaimsPrincipal;
        }

        public static ClaimsIdentity GetClaimsIdentity(this IPrincipal principal) {
            var claimsPrincipal = GetClaimsPrincipal(principal);

            return claimsPrincipal?.Identity as ClaimsIdentity;
        }

        public static string GetUserId(this IPrincipal principal) {
            return IsTokenAuthType(principal) ? GetClaimValue(principal, UserIdClaim) : GetClaimValue(principal, ClaimTypes.NameIdentifier);
        }

        public static string GetLoggedInUsersTokenId(this IPrincipal principal) {
            return IsUserAuthType(principal) ? GetClaimValue(principal, LoggedInUserTokenId) : null;
        }

        public static string GetOrganizationId(this IPrincipal principal) {
            return GetClaimValue(principal, OrganizationIdClaim);
        }

        public static string GetClaimValue(this IPrincipal principal, string type) {
            var identity = principal?.GetClaimsIdentity();
            if (identity == null)
                return null;

            return GetClaimValue(identity, type);
        }

        public static string GetClaimValue(this IIdentity identity, string type) {
            var claimsIdentity = identity as ClaimsIdentity;
            var claim = claimsIdentity?.FindAll(type).FirstOrDefault();

            return claim?.Value;
        }
    }
}
