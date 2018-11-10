using Foundatio.Skeleton.Repositories.Model;
using System;
using System.Collections.Generic;

namespace Foundatio.Skeleton.Domain.Models {
    public class Role : IIdentity, IHaveDates {
        public string Id { get; set; }

        public string Name { get; set; }

        public string SystemName { get; set; }

        public DateTime CreatedUtc { get; set; }

        public DateTime UpdatedUtc { get; set; }

        public virtual ICollection<User> Users {
            get { return _users ?? (_users = new List<User>()); }
            protected set { _users = value; }
        }
        private ICollection<User> _users;
    }
}
