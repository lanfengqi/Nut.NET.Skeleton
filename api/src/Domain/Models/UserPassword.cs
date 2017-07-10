using Foundatio.Skeleton.Repositories.Model;
using System;

namespace Foundatio.Skeleton.Domain.Models {
    public class UserPassword : IIdentity, IHaveCreatedDate {

        public string Id { get; set; }

        public string UserId { get; set; }

        public string Password { get; set; }

        public string Salt { get; set; }

        public string PasswordResetToken { get; set; }

        public DateTime PasswordResetTokenCreated { get; set; }

        public DateTime CreatedUtc { get; set; }

        public virtual User User { get; set; }
    }
}
