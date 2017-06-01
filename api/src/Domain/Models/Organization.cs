using Foundatio.Skeleton.Core.Collections;
using Foundatio.Skeleton.Core.Models;
using Foundatio.Skeleton.Repositories.Model;
using System;

namespace Foundatio.Skeleton.Domain.Models {
    public class Organization : IHaveData, IOwnedByOrganizationWithIdentity, IHaveDates, IVersioned {
        /// <summary>
        /// Unique id that identifies the organization.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Name of the organization.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// If true, the organization has been verified.
        /// </summary>
        public bool IsVerified { get; set; }

        /// <summary>
        /// Optional data entries that contain additional configuration information for this organization.
        /// </summary>
        public DataDictionary Data { get; set; } = new DataDictionary();

        public DateTime CreatedUtc { get; set; }
        public DateTime UpdatedUtc { get; set; }

        string IOwnedByOrganization.OrganizationId {
            get {
                return Id;
            }
            set {
                Id = value;
            }
        }

        public long Version { get; set; }
    }

}
