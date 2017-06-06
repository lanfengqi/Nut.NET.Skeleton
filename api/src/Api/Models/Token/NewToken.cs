using System;
using System.Collections.Generic;
using Foundatio.Skeleton.Domain.Models;

namespace Foundatio.Skeleton.Api.Models
{
    public class NewToken : IOwnedByOrganization {
        public NewToken() {
        }

        public string OrganizationId { get; set; }
        public DateTime? ExpiresUtc { get; set; }
        public string Notes { get; set; }
    }
}