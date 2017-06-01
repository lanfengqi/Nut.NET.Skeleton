using System;
using System.Collections.Generic;
using System.Linq;
using Foundatio.Skeleton.Core.Collections;
using Foundatio.Skeleton.Core.Extensions;

namespace Foundatio.Skeleton.Domain.Models {
    public static class UserExtensions {
       
        public static void CreateVerifyEmailAddressToken(this User user) {
            if (user == null)
                return;

            user.VerifyEmailAddressToken = StringUtils.GetNewToken();
            user.VerifyEmailAddressTokenCreated = DateTime.UtcNow;
        }

        public static bool HasValidEmailAddressTokenExpiration(this User user) {
            if (user == null)
                return false;

            //  todo:  revisit expiration date
            return user.VerifyEmailAddressTokenCreated != DateTime.MinValue && user.VerifyEmailAddressTokenCreated < DateTime.UtcNow.AddDays(30);
        }

        public static void MarkEmailAddressVerified(this User user) {
            if (user == null)
                return;

            user.IsEmailAddressVerified = true;
            user.VerifyEmailAddressToken = null;
            user.VerifyEmailAddressTokenCreated = DateTime.MinValue;
        }

        public static bool IsValidPassword(this User user, string password) {
            if (string.IsNullOrEmpty(user.Salt) || string.IsNullOrEmpty(user.Password)) {
                return false;
            }

            string encodedPassword = password.ToSaltedHash(user.Salt);
            return string.Equals(encodedPassword, user.Password);
        }

        public static void ResetPasswordResetToken(this User user) {
            if (user == null)
                return;

            user.PasswordResetToken = null;
            user.PasswordResetTokenCreated = DateTime.MinValue;
        }

        public static bool HasValidPasswordResetTokenExpiration(this User user) {
            if (user == null)
                return false;

            return user.PasswordResetTokenCreated != DateTime.MinValue && user.PasswordResetTokenCreated < DateTime.UtcNow.AddHours(24);
        }

        public static bool IsGlobalAdmin(this User user) {
            return user.Roles.Contains(AuthorizationRoles.GlobalAdmin);
        }

        public static bool IsAdmin(this User user, string organizationId) {
            var userOrganizationId = user.OrganizationId;
            return userOrganizationId.Equals(organizationId);
        }

        public static bool AddedMembershipRole(this User user, string organizationId, string role) {
            var roles = AuthorizationRoles.GetScope(role);
            return user.AddedMembershipRoles(organizationId, roles);
        }

        public static bool AddedMembershipRoles(this User user, string organizationId, ICollection<string> roles) {
            if (roles.Contains(AuthorizationRoles.GlobalAdmin))
                return false;

            if(user.OrganizationId == organizationId) {
                user.Roles.AddRange(roles);
                return true;
            }

            return false;
        }

        public static void AddAdminMembership(this User user, string organizationId) {
            user.AddedAdminMembershipRoles(organizationId);
        }

        public static bool AddedAdminMembershipRoles(this User user, string organizationId) {
            var roles = AuthorizationRoles.AdminScope;
            return user.AddedMembershipRoles(organizationId, roles);
        }

        public static void AddGlobalAdminRole(this User user) {
            user.AddedGlobalAdminRole();
        }

        public static bool AddedGlobalAdminRole(this User user) {
            if (user.Roles.Contains(AuthorizationRoles.GlobalAdmin, StringComparer.OrdinalIgnoreCase))
                return false;

            user.Roles.Add(AuthorizationRoles.GlobalAdmin);
            return true;
        }
    }
}
