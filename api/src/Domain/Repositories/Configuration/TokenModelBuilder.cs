using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Repositories.Configuration;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundatio.Skeleton.Domain.Repositories.Configuration {
    public sealed class TokenModelBuilder : ModelBuilderBase {

        public override void Configure(DbModelBuilder modelBuilder) {

            modelBuilder.Entity<Token>().ToTable("Token");
            modelBuilder.Entity<Token>().HasKey(emp => emp.Id);

            modelBuilder.Entity<Token>().Property(emp => emp.OrganizationId).HasMaxLength(50);
            modelBuilder.Entity<Token>().Property(emp => emp.UserId).HasMaxLength(50);
            modelBuilder.Entity<Token>().Property(emp => emp.Refresh).HasMaxLength(50);
            modelBuilder.Entity<Token>().Property(emp => emp.Notes).HasMaxLength(50);
            modelBuilder.Entity<Token>().Property(emp => emp.CreatedBy).HasMaxLength(50);

            modelBuilder.Entity<Token>().Ignore(emp => emp.Type);
            modelBuilder.Entity<Token>().Ignore(emp => emp.Data);
        }
    }
}
