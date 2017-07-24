using Foundatio.Skeleton.Repositories.Model;
using System;

namespace Foundatio.Skeleton.Api.Models {
    public class ViewOrganization : IIdentity, IVersioned {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsVerified { get; set; }
        public DateTime CreatedUtc { get; set; }
        public long Version { get; set; }
    }
}
