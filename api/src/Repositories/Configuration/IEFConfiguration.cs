using Foundatio.Caching;
using Foundatio.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Linq;

namespace Foundatio.Skeleton.Repositories.Configuration {
    public interface IEFConfiguration : IDisposable {

        ICacheClient Cache { get; }

        ILoggerFactory LoggerFactory { get; set; }

        ICollection<IModelBuilder> ModelBuilders { get; }

        void ConfigureModelBuilders(DbModelBuilder context);
    }


    public class EFConfiguration : IEFConfiguration {
        protected readonly ILogger _logger;
        protected readonly ICacheClient _cacheClient;
        private readonly List<IModelBuilder> _modelBuilders = new List<IModelBuilder>();

        public EFConfiguration(ICacheClient cacheClient = null, ILoggerFactory loggerFactory = null) {
            _logger = loggerFactory.CreateLogger(GetType());
            LoggerFactory = loggerFactory;

            _cacheClient = cacheClient;
        }

        public ICacheClient Cache => _cacheClient;

        public ILoggerFactory LoggerFactory { get; set; }

        public ICollection<IModelBuilder> ModelBuilders => _modelBuilders;

        public void AddModelBuilder(IModelBuilder modelBuilder) {
            if (modelBuilder == null)
                throw new ArgumentNullException("modelBuilder");

            _modelBuilders.Add(modelBuilder);
        }

        public void ConfigureModelBuilders(DbModelBuilder context) {
            var modelBuilders = ModelBuilders?.Distinct().ToList();
            if (modelBuilders == null || modelBuilders.Count == 0)
                return;

            foreach (var bulider in modelBuilders) {
                bulider.Configure(context);
            }
        }

        public virtual void Dispose() {
            Cache.Dispose();
        }
    }
}
