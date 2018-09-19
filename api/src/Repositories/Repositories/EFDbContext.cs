using Foundatio.Skeleton.Repositories.Configuration;
using System;
using System.Data.Entity;
using System.Linq;
using System.Reflection;

namespace Foundatio.Skeleton.Repositories {
    public class EFDbContext : DbContext {

        public EFDbContext()
            : base("DatabaseConnectionString") {

            Init();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            Assembly domainAssembly = null;
            try {
                domainAssembly = Assembly.Load("Foundatio.Skeleton.Domain");
            } catch (Exception ex) {
                throw ex;
            }
            var typesToRegister = domainAssembly.GetTypes()
           .Where(type => !String.IsNullOrEmpty(type.Namespace))
           .Where(type => type.BaseType != null && type.BaseType.IsGenericType &&
               type.BaseType.GetGenericTypeDefinition() == typeof(NutEntityTypeConfiguration<>));
            foreach (var type in typesToRegister) {
                dynamic configurationInstance = Activator.CreateInstance(type);
                modelBuilder.Configurations.Add(configurationInstance);
            }

            base.OnModelCreating(modelBuilder);
        }

        public static void Init() {
            var connectionFactory = new MySqlConnectionFactory();
#pragma warning disable 0618
            Database.DefaultConnectionFactory = connectionFactory;
        }
    }

}
