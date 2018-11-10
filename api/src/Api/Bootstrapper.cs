using AutoMapper;
using Foundatio.Caching;
using Foundatio.Logging;
using Foundatio.Skeleton.Api.MessageBus;
using Foundatio.Skeleton.Api.Models;
using Foundatio.Skeleton.Api.Security;
using Foundatio.Skeleton.Api.Utility;
using Foundatio.Skeleton.Core.Utility;
using Foundatio.Skeleton.Domain;
using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Domain.Services;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Infrastructure;
using SimpleInjector;
using SimpleInjector.Advanced;
using System;

namespace Foundatio.Skeleton.Api {
    public class Bootstrapper {
        public static void RegisterServices(Container container, ILoggerFactory loggerFactory) {
            container.Register<MessageBusConnection>();
            container.RegisterSingleton<IConnectionMapping, ConnectionMapping>();
            container.RegisterSingleton<MessageBusBroker>();

            var resolver = new SimpleInjectorSignalRDependencyResolver(container);
            container.RegisterSingleton<IDependencyResolver>(resolver);
            container.RegisterSingleton<IConnectionManager>(() => new ConnectionManager(resolver));

            container.RegisterSingleton<IUserIdProvider, MessageBus.PrincipalUserIdProvider>();
            container.RegisterSingleton<ThrottlingHandler>(() => new ThrottlingHandler(container.GetInstance<ICacheClient>(), userIdentifier => Settings.Current.ApiThrottleLimit, TimeSpan.FromMinutes(15)));
            container.RegisterSingleton<XHttpMethodOverrideDelegatingHandler>();
            container.RegisterSingleton<EncodingDelegatingHandler>();
            container.RegisterSingleton<AuthMessageHandler>();

            container.AppendToCollection(typeof(Profile), typeof(ApiMappings));
        }

        public class ApiMappings : Profile {
            protected override void Configure() {

                CreateMap<Organization, ViewOrganization>();
                CreateMap<Organization, NewOrganization>();
                CreateMap<NewOrganization, Organization>();
                CreateMap<ViewOrganization, NewOrganization>();

                CreateMap<Role, ViewRole>();
                CreateMap<ViewRole, Role>();

                CreateMap<Domain.Models.Token, ViewToken>();
                CreateMap<NewToken, Domain.Models.Token>().ForMember(m => m.Type, m => m.Ignore());

                CreateMap<User, ViewUser>().AfterMap((u, vu) => vu.IsGlobalAdmin = u.IsGlobalAdmin());
                CreateMap<User, UpdateUser>();
                CreateMap<UpdateUser, User>();


                CreateMap<PurchaseCar, ViewPurchaseCar>();
                CreateMap<PurchaseCar, NewPurchaseCar>();
                CreateMap<NewPurchaseCar, PurchaseCar>();

                CreateMap<Car, ViewCar>();
                CreateMap<ViewCar, Car>();
                CreateMap<Car, NewCar>();
                CreateMap<NewCar, Car>();
                CreateMap<UpdateCar, Car>();

                CreateMap<Attendance, ViewAttendance>();
                CreateMap<NewAttendance, Attendance>();
                CreateMap<Attendance, NewAttendance>();

                CreateMap<Farmer, ViewFarmer>();
                CreateMap<NewFarmer, Farmer>();
                CreateMap<Farmer, NewFarmer>();

                CreateMap<OrderCost, ViewOrderCost>();
                CreateMap<OrderItem, ViewOrderItem>();
                CreateMap<Order, ViewOrder>()
                    .ForMember(dest => dest.OrderItems, mo => mo.MapFrom(src => src.OrderItems))
                    .ForMember(dest => dest.OrderCosts, mo => mo.MapFrom(src => src.OrderCosts));

                CreateMap<OrderByUserReportLine, OrderByUserReportLineModel> ();
            }
        }
    }
}
