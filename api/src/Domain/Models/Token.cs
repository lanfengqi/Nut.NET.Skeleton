using Foundatio.Skeleton.Core.Collections;
using Foundatio.Skeleton.Repositories.Model;
using System;

namespace Foundatio.Skeleton.Domain.Models {
    public class Token : IOwnedByOrganizationWithIdentity, IHaveDates {

        public string Id { get; set; }

        public string OrganizationId { get; set; }

        public string UserId { get; set; }

        public string Refresh { get; set; }

        public TokenType Type { get; set; }

        public DateTime? ExpiresUtc { get; set; }

        public string Notes { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedUtc { get; set; }

        public DateTime UpdatedUtc { get; set; }
    }

    public enum TokenType : Int16 {
        Authentication = 0,
        Access = 1
    }
}
