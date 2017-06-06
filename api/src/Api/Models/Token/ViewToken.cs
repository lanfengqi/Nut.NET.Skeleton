using System;
using System.Collections.Generic;
using Foundatio.Skeleton.Repositories.Model;

namespace Foundatio.Skeleton.Api.Models {
    public class ViewToken : IIdentity {
        public string Id { get; set; }
        public string OrganizationId { get; set; }
        public string UserId { get; set; }
        public DateTime? ExpiresUtc { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime ModifiedUtc { get; set; }
    }
}
