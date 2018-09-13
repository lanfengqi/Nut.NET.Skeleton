using Foundatio.Skeleton.Repositories.Model;
using System;

namespace Foundatio.Skeleton.Api.Models {
    public class ViewUser : IIdentity, IHaveDates {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public bool PhoneNotificationsEnabled { get; set; }
        public bool IsPhoneVerified { get; set; }
        public bool IsActive { get; set; }
        public string ProfileImagePath { get; set; }
        public bool IsGlobalAdmin { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime UpdatedUtc { get; set; }
    }
}
