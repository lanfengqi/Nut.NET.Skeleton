﻿using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Repositories.Configuration;

namespace Foundatio.Skeleton.Domain.Repositories.Configuration {
    public class OrganizationMap : NutEntityTypeConfiguration<Organization> {

        public OrganizationMap() {
            this.ToTable("Organization");
            this.HasKey(c => c.Id);

            this.Property(u => u.Name).HasMaxLength(500);

            this.Ignore(o => o.Data);
        }
    }
}
