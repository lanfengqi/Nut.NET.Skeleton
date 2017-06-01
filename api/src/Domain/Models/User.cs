using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Foundatio.Skeleton.Core.Collections;
using Foundatio.Skeleton.Core.Models;
using Foundatio.Skeleton.Repositories.Model;

namespace Foundatio.Skeleton.Domain.Models {
    public class User : IHaveData, IIdentity, IHaveDates {
        public string Id { get; set; }

        public string FullName { get; set; }

        public string EmailAddress { get; set; }

        public string Password { get; set; }

        public string Salt { get; set; }

        public string PasswordResetToken { get; set; }

        public DateTime PasswordResetTokenCreated { get; set; }

        public bool EmailNotificationsEnabled { get; set; }

        public bool IsActive { get; set; }

        public bool IsEmailAddressVerified { get; set; }

        public string VerifyEmailAddressToken { get; set; }

        public DateTime VerifyEmailAddressTokenCreated { get; set; }

        public string ProfileImagePath { get; set; }

        public DataDictionary Data { get; set; }

        public DateTime CreatedUtc { get; set; }
        public DateTime UpdatedUtc { get; set; }

        public string OrganizationId { get; set; }

        /// <summary>
        /// General user role (type of user)
        /// </summary>
        public ICollection<string> Roles { get; set; }

        public User() {
            EmailNotificationsEnabled = true;
            IsActive = true;
            Roles = new Collection<string>();
            Data = new DataDictionary();
        }
    }

}
