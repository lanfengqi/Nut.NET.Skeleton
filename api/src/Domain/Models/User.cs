using Foundatio.Skeleton.Repositories.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Foundatio.Skeleton.Domain.Models {
    public class User : IIdentity, IHaveDates {
        public string Id { get; set; }

        public string FullName { get; set; }

        public string EmailAddress { get; set; }

        public string Phone { get; set; }

        public bool EmailNotificationsEnabled { get; set; }

        public bool IsActive { get; set; }

        public bool IsEmailAddressVerified { get; set; }

        public string VerifyEmailAddressToken { get; set; }

        public DateTime VerifyEmailAddressTokenCreated { get; set; }

        public string ProfileImagePath { get; set; }

        public DateTime CreatedUtc { get; set; }
        public DateTime UpdatedUtc { get; set; }

        public string OrganizationId { get; set; }

        /// <summary>
        /// General user role (type of user)
        /// </summary>
        public virtual ICollection<Role> Roles { get; set; }

        public User() {
            EmailNotificationsEnabled = true;
            IsActive = true;
            Roles = new Collection<Role>();
        }
    }

}
