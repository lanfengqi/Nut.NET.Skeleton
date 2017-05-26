using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundatio.Skeleton.Repositories.Repositories {
    public class EFDbContext : DbContext {


        public EFDbContext() {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
           
            //modelBuilder.Configurations.Add(new LanguageMap());



            base.OnModelCreating(modelBuilder);
        }
    }
}
