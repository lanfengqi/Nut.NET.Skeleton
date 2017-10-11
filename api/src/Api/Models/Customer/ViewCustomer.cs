using Foundatio.Skeleton.Repositories.Model;
using System;

namespace Foundatio.Skeleton.Api.Models {
    public class ViewCustomer : IIdentity {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public DateTime CreatedUtc { get; set; }

        public DateTime UpdatedUtc { get; set; }
    }
}
