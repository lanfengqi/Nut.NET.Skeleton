using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Repositories.Configuration;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundatio.Skeleton.Domain.Repositories.Configuration {
   public  class UserPasswordModelBuilder : ModelBuilderBase {
        public override void Configure(DbModelBuilder modelBuilder) {

            modelBuilder.Entity<UserPassword>().ToTable("UserPassword");
            modelBuilder.Entity<UserPassword>().HasKey(emp => emp.Id);


        }
    }
}
