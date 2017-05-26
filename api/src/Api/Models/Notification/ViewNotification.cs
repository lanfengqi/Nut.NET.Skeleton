using Foundatio.Skeleton.Core.Collections;
using Foundatio.Skeleton.Repositories.Model;
using System;
using System.Collections.Generic;

namespace Foundatio.Skeleton.Api.Models {
    public class ViewNotification : IIdentity {
        public string Id { get; set; }
        public string OrganizationId { get; set; }
        public string UserId { get; set; }

        public string Type { get; set; }
        public string Message { get; set; }

        public DataDictionary Data { get; set; }
        public ISet<string> Readers { get; set; }

        public DateTime CreatedUtc { get; set; }
        public DateTime ModifiedUtc { get; set; }
    }
}
