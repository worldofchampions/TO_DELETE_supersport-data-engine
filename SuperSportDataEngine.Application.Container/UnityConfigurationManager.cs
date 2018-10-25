using SuperSportDataEngine.Common.DependencyTracking;

namespace SuperSportDataEngine.Application.Container
{
    using Hangfire.SqlServer;
    using Hangfire;
    using MongoDB.Driver.Core.Events;
    using MongoDB.Driver;
    using StackExchange.Redis;
    using SuperSportDataEngine.Application.Container.Enums;
    using SuperSportDataEngine.Application.Service.Common.Interfaces;
    using SuperSportDataEngine.Application.Service.Common.Services;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces.DeprecatedFeed;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces.LegacyFeed;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.CmsLogic.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.DeprecatedFeed.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.Stats.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.UnitOfWork;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.UnitOfWork;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.MongoDb.PayloadData.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Services.Cms;
    using SuperSportDataEngine.ApplicationLogic.Services.DeprecatedFeed;
    using SuperSportDataEngine.ApplicationLogic.Services.LegacyFeed;
    using SuperSportDataEngine.ApplicationLogic.Services;
    using SuperSportDataEngine.Common.Caching;
    using SuperSportDataEngine.Common.Interfaces;
    using SuperSportDataEngine.Common.Logging;
    using SuperSportDataEngine.Gateway.Http.DeprecatedFeed.Services;
    using SuperSportDataEngine.Gateway.Http.Stats.Services;
    using SuperSportDataEngine.Gateway.Http.StatsProzone.Services;
    using SuperSportDataEngine.Logging.NLog.Logging;
    using SuperSportDataEngine.Logging.NLog.MSTeams;
    using SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Context;
    using SuperSportDataEngine.Repository.EntityFramework.PublicSportData.UnitOfWork;
    using SuperSportDataEngine.Repository.EntityFramework.SystemSportData.Context;
    using SuperSportDataEngine.Repository.EntityFramework.SystemSportData.UnitOfWork;
    using SuperSportDataEngine.Repository.MongoDb.PayloadData.Repositories;
    using System.Configuration;
    using System.Data.Entity;
    using System;
    using Unity.Injection;
    using Unity.Lifetime;
    using Unity;

    public static class UnityConfigurationManager
    {
        private const string PublicSportDataRepository = "PublicSportDataRepository";
        private const string SystemSportDataRepository = "SystemSportDataRepository";
        private static readonly ILoggingService Logger = LoggingService.GetLoggingService();

        private static readonly Lazy<ConnectionMultiplexer> LazyRedisConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            var connectionMultiplexer = ConnectionMultiplexer.Connect(ConfigurationManager.ConnectionStrings["Redis"].ConnectionString);
            connectionMultiplexer.PreserveAsyncOrder = false;
            return connectionMultiplexer;
        });

        private static readonly Lazy<IMongoClient> LazyMongoClient = new Lazy<IMongoClient>(() =>
        {
            var connectionString = ConfigurationManager.ConnectionStrings["MongoDB"].ConnectionString;
            var mongoUrl = new MongoUrl(connectionString);
            var mongoClientSettings = MongoClientSettings.FromUrl(mongoUrl);
            mongoClientSettings.ClusterConfigurator = clusterConfigurator =>
            {
                const string dependencyTypeName = "MongoDb";
                var dependencyName = connectionString;

                clusterConfigurator.Subscribe<CommandSucceededEvent>(e =>
                {
                    // Log a successful dependency event on every CommandSucceededEvent
                    Logger.SendTelemetry(dependencyTypeName, dependencyName, e.CommandName, DateTimeOffset.UtcNow.Subtract(e.Duration), e.Duration, true);
                });

                clusterConfigurator.Subscribe<CommandFailedEvent>(e =>
                {
                    // Log a failed dependency event on every CommandFailedEvent 
                    Logger.SendTelemetry(dependencyTypeName, dependencyName, e.Failure.Message, DateTimeOffset.UtcNow.Subtract(e.Duration), e.Duration, false);
                });
            };

            var mongoClient = new MongoClient(mongoClientSettings);

            return mongoClient;
        });

        private static readonly Lazy<IApplicationInsightDependencyTrackingModule> LazyDependencyTrackingModule = new Lazy<IApplicationInsightDependencyTrackingModule>(() =>
        {
            var instrumentationKey = ConfigurationManager.AppSettings["AppInsightsInstrumentationKey"];

            var dependencyTrackingModule = new ApplicationInsightDependencyTrackingModule(instrumentationKey);

            return dependencyTrackingModule;
        });

        private static ConnectionMultiplexer RedisConnection => LazyRedisConnection.Value;
        private static IMongoClient MongoClient => LazyMongoClient.Value;
        private static IApplicationInsightDependencyTrackingModule DependencyTrackingModule => LazyDependencyTrackingModule.Value;

        public static void RegisterTypes(IUnityContainer container, ApplicationScope applicationScope)
        {
            //   I had to add explicit usage of third party assemblies to the UnityConfigurationManager class just so that the third party dlls gets copied.

            //    Here's the reason why it happens.
            //    Before hack:
            //    - *Container* project is a class library which has the reference to SuperSportDataEngine.Logging.NLog.MSTeams.dll
            //    - *SchedulerClient* project references* Container* but does not make use of the SuperSportDataEngine.Logging.NLog.MSTeams.dll, so the dll is
            //      not copied to the */bin* folder of the SchedulerClient.

            //    After Hack:
            //    - SchedulerClient references *Container* which makes direct use of the dll. So the dll is copied.

            // Reference: https://stackoverflow.com/questions/20280717/references-from-class-library-are-not-copied-to-running-project-bin-folder

            // ReSharper disable once ObjectCreationAsStatement
            new MSTeamsTarget();
            new Microsoft.ApplicationInsights.NLogTarget.ApplicationInsightsTarget();

            ApplyRegistrationsForLogging(container);
            ApplyRegistrationsForApplicationLogic(container, applicationScope);
            ApplyRegistrationsForGatewayHttpDeprecatedFeed(container, applicationScope);
            ApplyRegistrationsForGatewayHttpStatsProzone(container, applicationScope);
            ApplyRegistrationsForGatewayHttpStats(container, applicationScope);
            ApplyRegistrationsForRepositoryEntityFrameworkPublicSportData(container);
            ApplyRegistrationsForRepositoryEntityFrameworkSystemSportData(container);
            ApplyRegistrationsForRepositoryMongoDbPayloadData(container, applicationScope);
        }

        public static void RegisterApiGlobalTypes(IUnityContainer container, ApplicationScope applicationScope)
        {
            ApplyRegistrationsForGatewayHttpCommon(container, applicationScope);
        }

        private static void ApplyRegistrationsForLogging(IUnityContainer container)
        {
            container.RegisterType<ILoggingService>(new InjectionFactory(l => Logger));
        }

        private static void ApplyRegistrationsForApplicationLogic(IUnityContainer container, ApplicationScope applicationScope)
        {
            container.RegisterType<ISystemTimeService, SystemTimeService>();
            container.RegisterType<ITemporaryExampleService, TemporaryExampleService>();
            container.RegisterType<IRugbyService, RugbyService>(new HierarchicalLifetimeManager());
            container.RegisterType<IMotorsportService, MotorsportService>(new HierarchicalLifetimeManager());

            if (applicationScope == ApplicationScope.WebApiLegacyFeed ||
                applicationScope == ApplicationScope.WebApiSystemApi)
            {
                container.RegisterType<ILegacyAuthService, LegacyAuthService>(new HierarchicalLifetimeManager());
            }

            if (applicationScope == ApplicationScope.WebApiSystemApi)
            {
                container.RegisterType<IRugbyCmsService, RugbyCmsService>(new HierarchicalLifetimeManager());
                container.RegisterType<IMotorsportCmsService, MotorsportCmsService>(new HierarchicalLifetimeManager());
            }

            if (applicationScope == ApplicationScope.WebApiLegacyFeed)
            {
                container.RegisterType<IDeprecatedFeedIntegrationServiceMotorsport, DeprecatedFeedIntegrationServiceMotorsport>(new HierarchicalLifetimeManager());
                container.RegisterType<IDeprecatedFeedIntegrationServiceRugby, DeprecatedFeedIntegrationServiceRugby>(new HierarchicalLifetimeManager());
                container.RegisterType<IMotorsportLegacyFeedService, MotorsportLegacyFeedService>(new HierarchicalLifetimeManager());
            }
        }

        private static void ApplyRegistrationsForGatewayHttpCommon(IUnityContainer container, ApplicationScope applicationScope)
        {
            try
            {
                container.RegisterType<ICache>(new ContainerControlledLifetimeManager(),
                    new InjectionFactory(x => new Cache(RedisConnection)));

                Logger.Cache = container.Resolve<ICache>();
            }
            catch (Exception exception)
            {
                container.RegisterType<ICache>(new ContainerControlledLifetimeManager(), new InjectionFactory(x => null));

                Logger.Error("NoCacheInDIContainer",
                    exception,
                    $"Message: {exception.Message}\n" +
                    $"StackTrace: {exception.StackTrace}\n" +
                    $"Inner Exception {exception.InnerException}");
            }
        }

        private static void ApplyRegistrationsForGatewayHttpDeprecatedFeed(IUnityContainer container, ApplicationScope applicationScope)
        {
            if (applicationScope == ApplicationScope.WebApiLegacyFeed)
            {
                container.RegisterType<IDeprecatedFeedService, DeprecatedFeedService>(new HierarchicalLifetimeManager());
            }
        }

        private static void ApplyRegistrationsForGatewayHttpStatsProzone(IUnityContainer container, ApplicationScope applicationScope)
        {
            if (applicationScope == ApplicationScope.ServiceSchedulerClient ||
                applicationScope == ApplicationScope.ServiceSchedulerIngestServer)
            {
                container.RegisterType<IStatsProzoneRugbyIngestService, StatsProzoneRugbyIngestService>(
                    new HierarchicalLifetimeManager());

                container.RegisterType<IApplicationInsightDependencyTrackingModule>(
                    new ContainerControlledLifetimeManager(), new InjectionFactory(x => DependencyTrackingModule));
            }
        }

        private static void ApplyRegistrationsForGatewayHttpStats(IUnityContainer container, ApplicationScope applicationScope)
        {
            if (applicationScope == ApplicationScope.ServiceSchedulerClient || applicationScope == ApplicationScope.ServiceSchedulerIngestServer)
            {
                var motorsportWebRequest = new StatsMotorsportMotorsportWebRequest("http://api.stats.com", "ta3dprpc4sn79ecm2wg7tqbg", "JDgQnhPVZQ");
                container.RegisterType<IStatsMotorsportIngestService, StatsMotorsportIngestService>(
                    new HierarchicalLifetimeManager(),
                    new InjectionConstructor(motorsportWebRequest, Logger));

                container.RegisterType<IApplicationInsightDependencyTrackingModule>(
                    new ContainerControlledLifetimeManager(), new InjectionFactory(x => DependencyTrackingModule));
            }
        }

        private static void ApplyRegistrationsForRepositoryEntityFrameworkPublicSportData(IUnityContainer container)
        {
            // The DbContext is shared across all the repositories in the container.
            container.RegisterType<DbContext, PublicSportDataContext>(PublicSportDataRepository, new HierarchicalLifetimeManager());

            // The Unit of work is shared accross all services per container.
            // The unit of work is responsible for the creation and disposing of the repositories.
            // The services will access the repositories through the unit of work object.
            container.RegisterType<IPublicSportDataUnitOfWork>(
                new HierarchicalLifetimeManager(),
                new InjectionFactory(x => new PublicSportDataUnitOfWork(
                    container.Resolve<DbContext>(PublicSportDataRepository))));
        }

        private static void ApplyRegistrationsForRepositoryEntityFrameworkSystemSportData(IUnityContainer container)
        {
            // The DbContext is shared across all the repositories in the container.
            container.RegisterType<DbContext, SystemSportDataContext>(SystemSportDataRepository, new HierarchicalLifetimeManager());

            // The Unit of work is shared accross all services per container.
            // The unit of work is responsible for the creation and disposing of the repositories.
            // The services will access the repositories through the unit of work object.
            container.RegisterType<ISystemSportDataUnitOfWork>(
                new HierarchicalLifetimeManager(),
                new InjectionFactory(x => new SystemSportDataUnitOfWork(
                    container.Resolve<DbContext>(SystemSportDataRepository))));
        }

        private static void ApplyRegistrationsForRepositoryMongoDbPayloadData(IUnityContainer container, ApplicationScope applicationScope)
        {
            if (applicationScope == ApplicationScope.ServiceSchedulerClient ||
                applicationScope == ApplicationScope.ServiceSchedulerIngestServer)
            {
                container.RegisterType<IRecurringJobManager>(
                    new HierarchicalLifetimeManager(),
                    new InjectionFactory(x => new RecurringJobManager(
                        new SqlServerStorage(ConfigurationManager.ConnectionStrings["SqlDatabase_Hangfire"].ConnectionString))));

                container.RegisterType<IMongoClient>(
                    new ContainerControlledLifetimeManager(),
                    new InjectionFactory(x => MongoClient));

                container.RegisterType<IMongoDbRugbyRepository, MongoDbRugbyRepository>();
                container.RegisterType<IMongoDbMotorsportRepository, MongoDbMotorsportRepository>();

                container.RegisterType<IRugbyIngestWorkerService, RugbyIngestWorkerService>(new HierarchicalLifetimeManager());

                container.RegisterType<IMotorsportStorageService, MotorsportStorageService>(new HierarchicalLifetimeManager());

                container.RegisterType<IMotorsportIngestWorkerService, MotorsportIngestWorkerService>(new HierarchicalLifetimeManager());

                container.RegisterType<ITemporaryExampleMongoDbRepository, TemporaryExampleMongoDbRepository>();

                container.RegisterType<ISchedulerClientService, SchedulerClientService>();
            }
        }
    }
}
