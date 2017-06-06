using Foundatio.Skeleton.Repositories.Model;
using System;

namespace Foundatio.Skeleton.Api.Models {
    public class ViewRole : IIdentity, IHaveDates {

        public string Id { get; set; }

        public string Name { get; set; }

        public string SystemName { get; set; }

        public DateTime CreatedUtc { get; set; }

        public DateTime UpdatedUtc { get; set; }

        public string OrganizationId { get; set; }
    }
}