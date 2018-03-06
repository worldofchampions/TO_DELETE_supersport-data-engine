namespace SuperSportDataEngine.Application.Container
{
    using Hangfire;
    using Hangfire.SqlServer;
    using Microsoft.Practices.Unity;
    using MongoDB.Driver;
    using NLog.Slack;
    using StackExchange.Redis;
    using SuperSportDataEngine.Application.Container.Enums;
    using SuperSportDataEngine.Application.WebApi.Common.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces.LegacyFeed;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.DeprecatedFeed.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.UnitOfWork;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.UnitOfWork;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.MongoDb.PayloadData.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Services;
    using SuperSportDataEngine.ApplicationLogic.Services.LegacyFeed;
    using SuperSportDataEngine.Common.Caching;
    using SuperSportDataEngine.Common.Logging;
    using SuperSportDataEngine.Gateway.Http.DeprecatedFeed.Services;
    using SuperSportDataEngine.Gateway.Http.StatsProzone.Services;
    using SuperSportDataEngine.Logging.NLog.Logging;
    using SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Context;
    using SuperSportDataEngine.Repository.EntityFramework.PublicSportData.UnitOfWork;
    using SuperSportDataEngine.Repository.EntityFramework.SystemSportData.Context;
    using SuperSportDataEngine.Repository.EntityFramework.SystemSportData.UnitOfWork;
    using SuperSportDataEngine.Repository.MongoDb.PayloadData.Repositories;
    using System.Configuration;
    using System.Data.Entity;
    using System.Web.Configuration;

    public static class UnityConfigurationManager
    {
        private const string PublicSportDataRepository = "PublicSportDataRepository";
        private const string SystemSportDataRepository = "SystemSportDataRepository";
        private static ILoggingService logger = LoggingService.GetLoggingService();

        public static void RegisterTypes(IUnityContainer container, ApplicationScope applicationScope)
        {
            // Hack sort of. To get the NLog.Slack.dll to be copied.
            new SlackClient();

            ApplyRegistrationsForLogging(container);
            ApplyRegistrationsForApplicationLogic(container, applicationScope);
            ApplyRegistrationsForGatewayHttpDeprecatedFeed(container, applicationScope);
            ApplyRegistrationsForGatewayHttpStatsProzone(container, applicationScope);
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
            container.RegisterType<ILoggingService>(new InjectionFactory(l => logger));
        }

        private static void ApplyRegistrationsForApplicationLogic(IUnityContainer container, ApplicationScope applicationScope)
        {
            container.RegisterType<ITemporaryExampleService, TemporaryExampleService>();
            container.RegisterType<IRugbyService, RugbyService>(new HierarchicalLifetimeManager());
            container.RegisterType<IMotorsportService, MotorsportService>(new HierarchicalLifetimeManager());

            if (applicationScope == ApplicationScope.WebApiLegacyFeed ||
                applicationScope == ApplicationScope.WebApiSystemApi)
            {
                container.RegisterType<ILegacyAuthService, LegacyAuthService>(new HierarchicalLifetimeManager());
            }

            if (applicationScope == ApplicationScope.WebApiLegacyFeed)
            {
                container.RegisterType<IDeprecatedFeedIntegrationService, DeprecatedFeedIntegrationService>(new HierarchicalLifetimeManager());
                container.RegisterType<IMotorsportLegacyFeedService, MotorsportLegacyFeedService>(new HierarchicalLifetimeManager());
            }
        }

        private static void ApplyRegistrationsForGatewayHttpCommon(IUnityContainer container, ApplicationScope applicationScope)
        {
            try
            {
                //if (applicationScope == ApplicationScope.WebApiLegacyFeed || applicationScope == ApplicationScope.WebApiPublicApi)
                {
                    container.RegisterType<ICache, Cache>(new ContainerControlledLifetimeManager(),
                        new InjectionFactory((x) => new Cache(ConnectionMultiplexer.Connect(WebConfigurationManager.ConnectionStrings["Redis"].ConnectionString))));

                    logger.Cache = container.Resolve<ICache>();
                }
            }
            catch (System.Exception exception)
            {
                container.RegisterType<ICache, Cache>(new ContainerControlledLifetimeManager(), new InjectionFactory((x) => null));

                logger.Error("NoCacheInDIContainer",
                    "Message: \n" + exception.Message +
                    "StackTrace: \n" + exception.StackTrace +
                    "Inner Exception \n" + exception.InnerException);
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

                //container.RegisterType<ILoggingService>(new InjectionFactory(l => logger));

                var temp = new StatsProzoneMotorWebRequest("http://api.stats.com", "ta3dprpc4sn79ecm2wg7tqbg", "JDgQnhPVZQ");
                container.RegisterType<IStatsProzoneMotorIngestService, StatsProzoneMotorIngestService>(
                    new HierarchicalLifetimeManager(),
                    new InjectionConstructor(temp, logger));
            }
        }

        private static void ApplyRegistrationsForRepositoryEntityFrameworkPublicSportData(IUnityContainer container)
        {
            // The DbContext is shared across all the repositories in the container.
            container.RegisterType<DbContext, PublicSportDataContext>(PublicSportDataRepository, new HierarchicalLifetimeManager());

            // The Unit of work is shared accross all services per container.
            // The unit of work is responsible for the creation and disposing of the repositories.
            // The services will access the repositories through the unit of work object.
            container.RegisterType<IPublicSportDataUnitOfWork, PublicSportDataUnitOfWork>(
                new HierarchicalLifetimeManager(),
                new InjectionFactory((x) => new PublicSportDataUnitOfWork(
                    container.Resolve<DbContext>(PublicSportDataRepository))));
        }

        private static void ApplyRegistrationsForRepositoryEntityFrameworkSystemSportData(IUnityContainer container)
        {
            // The DbContext is shared across all the repositories in the container.
            container.RegisterType<DbContext, SystemSportDataContext>(SystemSportDataRepository, new HierarchicalLifetimeManager());

            // The Unit of work is shared accross all services per container.
            // The unit of work is responsible for the creation and disposing of the repositories.
            // The services will access the repositories through the unit of work object.
            container.RegisterType<ISystemSportDataUnitOfWork, SystemSportDataUnitOfWork>(
                new HierarchicalLifetimeManager(),
                new InjectionFactory((x) => new SystemSportDataUnitOfWork(
                    container.Resolve<DbContext>(SystemSportDataRepository))));
        }

        private static void ApplyRegistrationsForRepositoryMongoDbPayloadData(IUnityContainer container, ApplicationScope applicationScope)
        {
            if (applicationScope == ApplicationScope.ServiceSchedulerClient ||
                applicationScope == ApplicationScope.ServiceSchedulerIngestServer)
            {
                container.RegisterType<IRecurringJobManager, RecurringJobManager>(
                    new HierarchicalLifetimeManager(),
                    new InjectionFactory((x) => new RecurringJobManager(
                        new SqlServerStorage(ConfigurationManager.ConnectionStrings["SqlDatabase_Hangfire"].ConnectionString))));

                container.RegisterType<IMongoClient, MongoClient>(
                    new ContainerControlledLifetimeManager(),
                    new InjectionFactory((x) => new MongoClient(
                        ConfigurationManager.ConnectionStrings["MongoDB"].ConnectionString)));

                container.RegisterType<IMongoDbRugbyRepository, MongoDbRugbyRepository>();

                container.RegisterType<IRugbyIngestWorkerService, RugbyIngestWorkerService>(new HierarchicalLifetimeManager());

                container.RegisterType<IMotorsportIngestWorkerService, MotorsportIngestWorkerService>(new HierarchicalLifetimeManager());

                container.RegisterType<ITemporaryExampleMongoDbRepository, TemporaryExampleMongoDbRepository>();

                container.RegisterType<ISchedulerClientService, SchedulerClientService>();
            }
        }
    }
}
