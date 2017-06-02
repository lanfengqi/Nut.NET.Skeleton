﻿using Foundatio.Skeleton.Core.Collections;
using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Repositories.Model;
using System;
using System.Collections.Generic;

namespace Foundatio.Skeleton.Api.Models {
    public class ViewOrganization : IIdentity, IVersioned {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsVerified { get; set; }
        public DataDictionary Data { get; set; }
        public DateTime CreatedUtc { get; set; }
        public long Version { get; set; }
    }
}
