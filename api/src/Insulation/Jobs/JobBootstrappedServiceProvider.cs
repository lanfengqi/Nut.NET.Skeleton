using Foundatio.Logging;
using Foundatio.ServiceProviders;
using Foundatio.Skeleton.Core.Extensions;
using SimpleInjector;
using SimpleInjector.Extensions.ExecutionContextScoping;
using System;

namespace Foundatio.Skeleton.Insulation.Jobs
{
    public class JobBootstrappedServiceProvider : BootstrappedServiceProviderBase {
        protected override IServiceProvider BootstrapInternal(ILoggerFactory loggerFactory) {

            var container = new Container();
            container.Options.DefaultScopedLifestyle = new ExecutionContextScopeLifestyle();
            container.Options.AllowOverridingRegistrations = true;
            container.Options.ResolveUnregisteredCollections = true;

            container.RegisterSingleton<IServiceProvider>(this);
            Foundatio.Skeleton.Domain.Bootstrapper.RegisterServices(container, loggerFactory);
            Bootstrapper.RegisterServices(container, loggerFactory);

#if DEBUG
            container.Verify();
#endif

            container.RunStartupActionsAsync().GetAwaiter().GetResult();

            return container;
        }
    }
}
