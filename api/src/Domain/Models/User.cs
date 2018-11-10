using Foundatio.Skeleton.Repositories.Model;
using System;
using System.Collections.Generic;

namespace Foundatio.Skeleton.Domain.Models {
    public class User : IIdentity, IHaveDates {
        public string Id { get; set; }

        public string FullName { get; set; }

        public string Phone { get; set; }

        public bool PhoneNotificationsEnabled { get; set; }

        public bool IsActive { get; set; }

        public bool IsPhoneVerified { get; set; }

        public string VerifyPhoneToken { get; set; }

        public DateTime VerifyPhoneTokenCreated { get; set; }

        public string ProfileImagePath { get; set; }

        public DateTime CreatedUtc { get; set; }
        public DateTime UpdatedUtc { get; set; }

        public string OrganizationId { get; set; }

        public virtual ICollection<Role> Roles {
            get { return _roles ?? (_roles = new List<Role>()); }
            protected set { _roles = value; }
        }
        private ICollection<Role> _roles;

        public User() {
            PhoneNotificationsEnabled = true;
            IsActive = true;
        }
    }

}
