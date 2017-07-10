using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Foundatio.Skeleton.Domain;
using Foundatio.Skeleton.Domain.Models;

namespace Foundatio.Skeleton.Api.Models
{
    public class ViewCurrentUser : ViewUser {
        public ViewCurrentUser(User user, string[] roles, string currentOrganizationId) {
            Id = user.Id;
            FullName = user.FullName;
            EmailAddress = user.EmailAddress;
            EmailNotificationsEnabled = user.EmailNotificationsEnabled;
            IsEmailAddressVerified = user.IsEmailAddressVerified;
            IsActive = user.IsActive;
            ProfileImagePath = user.ProfileImagePath;
            ProfileImageUrl = StorageHelper.GetPictureUrl(user.ProfileImagePath);
            Roles = new List<string>(roles);
             CurrentOrganizationId = currentOrganizationId;

            Hash = HMACSHA256HashString(user.Id);
            Data = user.Data;
            CreatedUtc = user.CreatedUtc;
            UpdatedUtc = user.UpdatedUtc;
        }

        public string ProfileImageUrl { get; set; }

        public ICollection<string> Roles { get; set; }
        public string Hash { get; set; }
        public bool HasLocalPassword { get; set; }

        public string CurrentOrganizationId { get; set; }

        private string HMACSHA256HashString(string value) {
            if (!Settings.Current.EnableIntercom)
                return null;

            byte[] secretKey = Encoding.UTF8.GetBytes(Settings.Current.IntercomAppSecret);
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            using (var hmac = new HMACSHA256(secretKey)) {
                hmac.ComputeHash(bytes);
                byte[] data = hmac.Hash;

                var builder = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                    builder.Append(data[i].ToString("x2"));

                return builder.ToString();
            }
        }
    }
}
