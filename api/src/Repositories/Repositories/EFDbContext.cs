using Foundatio.Skeleton.Repositories.Configuration;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using SimpleInjector;
using Foundatio.Skeleton.Core.Dependency;

namespace Foundatio.Skeleton.Repositories.Repositories {
    public class EFDbContext : DbContext {

        private readonly IDependencyResolver _dependencyResolver;

        public EFDbContext(IDependencyResolver dependencyResolver)
            : base("DatabaseConnectionString") {

            _dependencyResolver = dependencyResolver;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            var _modelBuliders = _dependencyResolver.GetServices<IModelBuilder>();

            foreach (var model in _modelBuliders)
                model.Configure(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }
    }
}
