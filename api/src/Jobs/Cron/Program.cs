using System;
//using Foundatio.CronJob;
using Foundatio.Extensions;
using Foundatio.ServiceProviders;
using Foundatio.Skeleton.Core.Extensions;
using Foundatio.Skeleton.Domain;
using Foundatio.Skeleton.Domain.Jobs;
using Foundatio.Jobs;
using System.Threading.Tasks;
using System.Threading;
using Foundatio.Logging;
using System.Runtime;

namespace Foundatio.Skeleton.Jobs {
    public class Program {
        public static void Main() {
            AppDomain.CurrentDomain.SetDataDirectory();
            var loggerFactory = Settings.GetLoggerFactory();
            var serviceProvider = ServiceProvider.GetServiceProvider(Settings.JobBootstrappedServiceProvider, loggerFactory);
            var cronService = serviceProvider.GetService<CronService>();

            // data snapshot every hour
            cronService.Add(() => serviceProvider.GetService<TestJob>(), "0 * * * *");

            // cleanup snapshots every 6 hours
            //cronService.Add(() => serviceProvider.GetService<CleanupSnapshotJob>(), "0 */6 * * *");

            // cleanup indices every 6 hours
            //cronService.Add(() => serviceProvider.GetService<CleanupIndexesJob>(), "0 */6 * * *");

            cronService.RunAsService();
        }
    }

    public class TestJob : IJob {

        private readonly ILogger _logger;

        public TestJob(ILoggerFactory loggerFactory) {
            _logger = loggerFactory.CreateLogger(GetType());
        }

        public virtual async Task<JobResult> RunAsync(CancellationToken cancellationToken = default(CancellationToken)) {

            _logger.Info("Starting Test ...");

            await Task.Delay(TimeSpan.FromSeconds(10));

            _logger.Info("Ending Test ...");

            return JobResult.Success;
        }
    }
}
