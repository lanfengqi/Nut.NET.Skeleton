using AutoMapper;
using FluentValidation;
using Foundatio.Caching;
using Foundatio.Jobs;
using Foundatio.Lock;
using Foundatio.Logging;
using Foundatio.Messaging;
using Foundatio.Metrics;
using Foundatio.Queues;
using Foundatio.Skeleton.Core.Dependency;
using Foundatio.Skeleton.Core.Extensions;
using Foundatio.Skeleton.Core.Sms;
using Foundatio.Skeleton.Core.Sms.Model;
using Foundatio.Skeleton.Core.Utility;
using Foundatio.Skeleton.Domain.Repositories;
using Foundatio.Skeleton.Domain.Services;
using Foundatio.Skeleton.Repositories;
using Foundatio.Skeleton.Repositories.Configuration;
using Foundatio.Storage;
using SimpleInjector;
using SimpleInjector.Advanced;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Net.Http;

namespace Foundatio.Skeleton.Domain {
    public class Bootstrapper {
        public static void RegisterServices(Container container, ILoggerFactory loggerFactory) {
            var logger = loggerFactory.CreateLogger<Bootstrapper>();
            logger.Info().Message(() => $"Bootstrapping \"{Process.GetCurrentProcess().ProcessName}\" on \"{Environment.MachineName}\".").Property("data", Settings.Current).Write();

            container.RegisterLogger(loggerFactory);

            container.RegisterSingleton<IDependencyResolver>(() => new SimpleInjectorDependencyContainer(container));
            container.RegisterSingleton<IMetricsClient>(() => new InMemoryMetricsClient());

            container.RegisterSingleton<EFConfiguration>();

            container.RegisterSingleton<DbContext>(() => new EFDbContext());
            container.RegisterSingleton<IEFRepositoryContext, EFRepositoryContext>();

            container.RegisterSingleton<ICacheClient, InMemoryCacheClient>();

            container.RegisterSingleton<IEnumerable<IQueueBehavior<WorkItemData>>>(() => new[] { new MetricsQueueBehavior<WorkItemData>(container.GetInstance<IMetricsClient>()) });

            var handlers = new WorkItemHandlers();
            container.RegisterSingleton(handlers);

            container.RegisterSingleton<IQueue<WorkItemData>>(() => new InMemoryQueue<WorkItemData>(behaviors: container.GetAllInstances<IQueueBehavior<WorkItemData>>(), workItemTimeout: TimeSpan.FromHours(1)));
            container.RegisterSingleton<IQueue<SmsMessage>>(() => new InMemoryQueue<SmsMessage>());

            container.RegisterSingleton<IMessageBus, InMemoryMessageBus>();
            container.RegisterSingleton<IMessagePublisher>(container.GetInstance<IMessageBus>);
            container.RegisterSingleton<IMessageSubscriber>(container.GetInstance<IMessageBus>);
            container.RegisterSingleton<HttpMessageHandler, HttpClientHandler>();

            if (!String.IsNullOrEmpty(Settings.Current.StorageFolder)) {
                try {
                    container.RegisterSingleton<IFileStorage>(new FolderFileStorage($"{Settings.Current.StorageFolder}\\private"));
                    container.RegisterSingleton<IPublicFileStorage>(new PublicFileStorage(new FolderFileStorage($"{Settings.Current.StorageFolder}\\public")));
                } catch (Exception ex) {
                    logger.Error(ex, $"Error setting folder storage: {ex.Message}");
                    container.RegisterSingleton<IFileStorage>(new InMemoryFileStorage());
                    container.RegisterSingleton<IPublicFileStorage>(new PublicFileStorage(new InMemoryFileStorage()));
                }
            } else {
                container.RegisterSingleton<IFileStorage>(new InMemoryFileStorage());
                container.RegisterSingleton<IPublicFileStorage>(new PublicFileStorage(new InMemoryFileStorage()));
            }

            container.Register(typeof(IValidator<>), new[] { typeof(Bootstrapper).Assembly }, Lifestyle.Singleton);

            container.RegisterSingleton<IOrganizationRepository, OrganizationRepository>();
            container.RegisterSingleton<IRoleRepository, RoleRepository>();
            container.RegisterSingleton<IUserRepository, UserRepository>();
            container.RegisterSingleton<IUserPasswordRepository, UserPasswordRepository>();
            container.RegisterSingleton<ITokenRepository, TokenRepository>();
            container.RegisterSingleton<IPurchaseCarRepository, PurchaseCarRepository>();
            container.RegisterSingleton<ICarRepository, CarRepository>();
            container.RegisterSingleton<ITemplatedSmsService, TemplatedSmsService>();

            container.RegisterSingleton<AliyunSmsOption>(() => new AliyunSmsOption(accessKeyId: "LTAInWMed2C2HnjD", accessKeySecret: "yjXA5bMaKgSgcotE98oPjCqP9ISmBC", signName: "华宝食品"));
            container.RegisterSingleton<ISmsSender, AliyunSmsSender>();



            container.RegisterSingleton<ILockProvider, CacheLockProvider>();
            container.RegisterSingleton<FirstInsatllService>();

            container.AddStartupAction(() => container.GetInstance<EFConfiguration>().ConfigureDatabase());

            container.AppendToCollection(typeof(Profile), typeof(DomainMappings));
            container.RegisterSingleton<IMapper>(() => {
                var profiles = container.GetAllInstances<Profile>();
                var config = new MapperConfiguration(cfg => {
                    cfg.ConstructServicesUsing(container.GetInstance);

                    foreach (var profile in profiles)
                        cfg.AddProfile(profile);
                });

                return config.CreateMapper();
            });
        }
    }

    public class DomainMappings : Profile {
        protected override void Configure() {

        }
    }
}
