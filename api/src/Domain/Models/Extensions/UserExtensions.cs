using System;
using System.Collections.Generic;
using System.Linq;
using Foundatio.Skeleton.Core.Collections;
using Foundatio.Skeleton.Core.Extensions;
using Foundatio.Skeleton.Domain.Repositories;
using System.Threading.Tasks;

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

        public static bool IsValidPassword(this UserPassword userPassword, string password) {
            if (string.IsNullOrEmpty(userPassword.Salt) || string.IsNullOrEmpty(userPassword.Password)) {
                return false;
            }

            string encodedPassword = password.ToSaltedHash(userPassword.Salt);
            return string.Equals(encodedPassword, userPassword.Password);
        }

        public static void ResetPasswordResetToken(this UserPassword userPassword) {
            if (userPassword == null)
                return;

            userPassword.PasswordResetToken = null;
            userPassword.PasswordResetTokenCreated = DateTime.MinValue;
        }

        public static bool HasValidPasswordResetTokenExpiration(this UserPassword userPassword) {
            if (userPassword == null)
                return false;

            return userPassword.PasswordResetTokenCreated != DateTime.MinValue && userPassword.PasswordResetTokenCreated < DateTime.UtcNow.AddHours(24);
        }

        public static bool IsGlobalAdmin(this User user) {
            return user.Roles.Any(x => x.SystemName == AuthorizationRoles.GlobalAdmin);
        }

        public static bool IsAdmin(this User user, string organizationId) {
            var userOrganizationId = user.OrganizationId;
            return userOrganizationId.Equals(organizationId);
        }

        public static async Task<bool> AddedMembershipRole(this User user, IRoleRepository roleRepository, string organizationId, string role) {
            var roles = AuthorizationRoles.GetScope(role);
            var roleList = await roleRepository.GetBySystemNamesAsync(roles);

            return user.AddedMembershipRoles(organizationId, roleList.ToList());
        }

        public static bool AddedMembershipRoles(this User user, string organizationId, ICollection<Role> roles) {
            if (roles.Any(x => x.SystemName == AuthorizationRoles.GlobalAdmin))
                return false;

            if (user.OrganizationId == organizationId) {
                user.Roles.AddRange(roles);
                return true;
            }

            return false;
        }

        //public static void AddAdminMembership(this User user, string organizationId) {
        //    user.AddedAdminMembershipRoles(organizationId);
        //}

        public static async Task<bool> AddedAdminMembershipRoles(this User user, IRoleRepository roleRepository, string organizationId) {
            var roles = AuthorizationRoles.AdminScope;
            var roleList = await roleRepository.GetBySystemNamesAsync(roles);
            return user.AddedMembershipRoles(organizationId, roleList.ToList());
        }

        //public static void AddGlobalAdminRole(this User user) {
        //    user.AddedGlobalAdminRole();
        //}

        public static async Task<bool> AddedGlobalAdminRole(this User user, IRoleRepository roleRepository) {
            if (user.Roles.Any(x => x.SystemName == AuthorizationRoles.GlobalAdmin))
                return false;
            var role = await roleRepository.GetBySystemNameAsync(AuthorizationRoles.GlobalAdmin);

            user.Roles.Add(role);

            return true;
        }
    }
}
