using SuperSportDataEngine.Common.Caching;

namespace SuperSportDataEngine.Application.Container
{
    using Hangfire;
    using Hangfire.SqlServer;
    using Microsoft.Practices.Unity;
    using MongoDB.Driver;
    using StackExchange.Redis;
    using SuperSportDataEngine.Application.Container.Enums;
    using SuperSportDataEngine.Application.WebApi.Common.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.MongoDb.PayloadData.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Services;
    using SuperSportDataEngine.Gateway.Http.StatsProzone.Services;
    using SuperSportDataEngine.Repository.EntityFramework.Common.Repositories.Base;
    using SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Context;
    using SuperSportDataEngine.Repository.EntityFramework.SystemSportData.Context;
    using SuperSportDataEngine.Repository.MongoDb.PayloadData.Repositories;
    using System.Configuration;
    using System.Data.Entity;
    using System.Web.Configuration;
    using SuperSportDataEngine.Common.Logging;
    using SuperSportDataEngine.Logging.NLog.Logging;
    using NLog.Slack;

    public static class UnityConfigurationManager
    {
        private const string PublicSportDataRepository = "PublicSportDataRepository";
        private const string SystemSportDataRepository = "SystemSportDataRepository";
        private static readonly ILoggingService Logger = LoggingService.GetLoggingService();

        private static readonly ICache Cache =
            new Cache(
                ConnectionMultiplexer.Connect(WebConfigurationManager.ConnectionStrings["Redis"].ConnectionString));

        public static void RegisterTypes(IUnityContainer container, ApplicationScope applicationScope)
        {
            // Hack sort of. To get the NLog.Slack.dll to be copied.
            new SlackClient();

            ApplyRegistrationsForLogging(container);
            ApplyRegistrationsForApplicationLogic(container, applicationScope);
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
            container.RegisterType<ILoggingService>(new InjectionFactory(l => Logger));
        }

        private static void ApplyRegistrationsForApplicationLogic(IUnityContainer container, ApplicationScope applicationScope)
        {
            container.RegisterType<ITemporaryExampleService, TemporaryExampleService>();
            container.RegisterType<IRugbyService, RugbyService>(new HierarchicalLifetimeManager());

            if (applicationScope == ApplicationScope.WebApiLegacyFeed ||
                applicationScope == ApplicationScope.WebApiSystemApi)
            {
                container.RegisterType<ILegacyAuthService, LegacyAuthService>();
            }
        }

        private static void ApplyRegistrationsForGatewayHttpCommon(IUnityContainer container, ApplicationScope applicationScope)
        {

            try
            {
                //if (applicationScope == ApplicationScope.WebApiLegacyFeed || applicationScope == ApplicationScope.WebApiPublicApi)
                {
                    container.RegisterType<ICache, Cache>(new InjectionFactory((x) => Cache));
                }
            }
            catch (System.Exception)
            {
                container.RegisterType<ICache, Cache>(new ContainerControlledLifetimeManager(),new InjectionFactory((x) => null));
            }

            Logger.Cache = container.Resolve<ICache>();
        }

        private static void ApplyRegistrationsForGatewayHttpStatsProzone(IUnityContainer container, ApplicationScope applicationScope)
        {
            if (applicationScope == ApplicationScope.ServiceSchedulerClient ||
                applicationScope == ApplicationScope.ServiceSchedulerIngestServer)
            {
                container.RegisterType<IStatsProzoneRugbyIngestService, StatsProzoneRugbyIngestService>(new HierarchicalLifetimeManager());
            }
        }

        private static void ApplyRegistrationsForRepositoryEntityFrameworkPublicSportData(IUnityContainer container)
        {
            // Having a DbContext registered in the IoC container causes issues for a few reasons.
            // 1. This context will be used for all the repositories.
            //    For example, multiple repositories will be using the same context. This is an Exeption thrown by the Hangfire jobs.
            // 2. We should ideally be using new DbContext's for each repository. Hence the changes below.
            // 3. Ammended, added back the context. The real issue was that multiple threads were using the same context.
            //    Multiple repositories in a Hangfire job using the same context is fine.
            container.RegisterType<DbContext, PublicSportDataContext>(PublicSportDataRepository, new HierarchicalLifetimeManager());

            container.RegisterType<IBaseEntityFrameworkRepository<RugbyCommentary>, BaseEntityFrameworkRepository<RugbyCommentary>>(
                new HierarchicalLifetimeManager(),
                new InjectionFactory(x => new BaseEntityFrameworkRepository<RugbyCommentary>(container.Resolve<DbContext>(PublicSportDataRepository))));

            container.RegisterType<IBaseEntityFrameworkRepository<RugbyEventType>, BaseEntityFrameworkRepository<RugbyEventType>>(
                new HierarchicalLifetimeManager(),
                new InjectionFactory(x => new BaseEntityFrameworkRepository<RugbyEventType>(container.Resolve<DbContext>(PublicSportDataRepository))));

            container.RegisterType<IBaseEntityFrameworkRepository<RugbyEventTypeProviderMapping>, BaseEntityFrameworkRepository<RugbyEventTypeProviderMapping>>(
                new HierarchicalLifetimeManager(),
                new InjectionFactory(x => new BaseEntityFrameworkRepository<RugbyEventTypeProviderMapping>(container.Resolve<DbContext>(PublicSportDataRepository))));

            container.RegisterType<IBaseEntityFrameworkRepository<RugbyFixture>, BaseEntityFrameworkRepository<RugbyFixture>>(
                new HierarchicalLifetimeManager(),
                new InjectionFactory(x => new BaseEntityFrameworkRepository<RugbyFixture>(container.Resolve<DbContext>(PublicSportDataRepository))));

            container.RegisterType<IBaseEntityFrameworkRepository<RugbyFlatLog>, BaseEntityFrameworkRepository<RugbyFlatLog>>(
                new HierarchicalLifetimeManager(),
                new InjectionFactory(x => new BaseEntityFrameworkRepository<RugbyFlatLog>(container.Resolve<DbContext>(PublicSportDataRepository))));

            container.RegisterType<IBaseEntityFrameworkRepository<RugbyGroupedLog>, BaseEntityFrameworkRepository<RugbyGroupedLog>>(
                new HierarchicalLifetimeManager(),
                new InjectionFactory(x => new BaseEntityFrameworkRepository<RugbyGroupedLog>(container.Resolve<DbContext>(PublicSportDataRepository))));

            container.RegisterType<IBaseEntityFrameworkRepository<RugbyLogGroup>, BaseEntityFrameworkRepository<RugbyLogGroup>>(
                new HierarchicalLifetimeManager(),
                new InjectionFactory(x => new BaseEntityFrameworkRepository<RugbyLogGroup>(container.Resolve<DbContext>(PublicSportDataRepository))));

            container.RegisterType<IBaseEntityFrameworkRepository<RugbyMatchEvent>, BaseEntityFrameworkRepository<RugbyMatchEvent>>(
                new HierarchicalLifetimeManager(),
                new InjectionFactory(x => new BaseEntityFrameworkRepository<RugbyMatchEvent>(container.Resolve<DbContext>(PublicSportDataRepository))));

            container.RegisterType<IBaseEntityFrameworkRepository<RugbyMatchStatistics>, BaseEntityFrameworkRepository<RugbyMatchStatistics>>(
                new HierarchicalLifetimeManager(),
                new InjectionFactory(x => new BaseEntityFrameworkRepository<RugbyMatchStatistics>(container.Resolve<DbContext>(PublicSportDataRepository))));

            container.RegisterType<IBaseEntityFrameworkRepository<RugbyPlayer>, BaseEntityFrameworkRepository<RugbyPlayer>>(
                new HierarchicalLifetimeManager(),
                new InjectionFactory(x => new BaseEntityFrameworkRepository<RugbyPlayer>(container.Resolve<DbContext>(PublicSportDataRepository))));

            container.RegisterType<IBaseEntityFrameworkRepository<RugbyPlayerLineup>, BaseEntityFrameworkRepository<RugbyPlayerLineup>>(
                new HierarchicalLifetimeManager(),
                new InjectionFactory(x => new BaseEntityFrameworkRepository<RugbyPlayerLineup>(container.Resolve<DbContext>(PublicSportDataRepository))));

            container.RegisterType<IBaseEntityFrameworkRepository<RugbySeason>, BaseEntityFrameworkRepository<RugbySeason>>(
                new HierarchicalLifetimeManager(),
                new InjectionFactory(x => new BaseEntityFrameworkRepository<RugbySeason>(container.Resolve<DbContext>(PublicSportDataRepository))));

            container.RegisterType<IBaseEntityFrameworkRepository<RugbyTeam>, BaseEntityFrameworkRepository<RugbyTeam>>(
                new HierarchicalLifetimeManager(),
                new InjectionFactory(x => new BaseEntityFrameworkRepository<RugbyTeam>(container.Resolve<DbContext>(PublicSportDataRepository))));

            container.RegisterType<IBaseEntityFrameworkRepository<RugbyTournament>, BaseEntityFrameworkRepository<RugbyTournament>>(
                new HierarchicalLifetimeManager(),
                new InjectionFactory(x => new BaseEntityFrameworkRepository<RugbyTournament>(container.Resolve<DbContext>(PublicSportDataRepository))));

            container.RegisterType<IBaseEntityFrameworkRepository<RugbyVenue>, BaseEntityFrameworkRepository<RugbyVenue>>(
                new HierarchicalLifetimeManager(),
                new InjectionFactory(x => new BaseEntityFrameworkRepository<RugbyVenue>(container.Resolve<DbContext>(PublicSportDataRepository))));
        }

        private static void ApplyRegistrationsForRepositoryEntityFrameworkSystemSportData(IUnityContainer container)
        {
            // Having a DbContext registered in the IoC container causes issues for a few reasons.
            // 1. This context will be used for all the repositories.
            //    For example, multiple repositories will be using the same context. This is an Exeption thrown by the Hangfire jobs.
            // 2. We should ideally be using new DbContext's for each repository. Hence the changes below.
            // 3. Ammended, added back the context. The real issue was that multiple threads were using the same context.
            //    Multiple repositories in a Hangfire job using the same context is fine.
            container.RegisterType<DbContext, SystemSportDataContext>(SystemSportDataRepository, new HierarchicalLifetimeManager());

            container.RegisterType<IBaseEntityFrameworkRepository<LegacyAuthFeedConsumer>, BaseEntityFrameworkRepository<LegacyAuthFeedConsumer>>(
                new HierarchicalLifetimeManager(),
                new InjectionFactory(x => new BaseEntityFrameworkRepository<LegacyAuthFeedConsumer>(container.Resolve<DbContext>(SystemSportDataRepository))));

            container.RegisterType<IBaseEntityFrameworkRepository<LegacyZoneSite>, BaseEntityFrameworkRepository<LegacyZoneSite>>(
                new HierarchicalLifetimeManager(),
                new InjectionFactory(x => new BaseEntityFrameworkRepository<LegacyZoneSite>(container.Resolve<DbContext>(SystemSportDataRepository))));

            container.RegisterType<IBaseEntityFrameworkRepository<SchedulerDashboardUser>, BaseEntityFrameworkRepository<SchedulerDashboardUser>>(
                new HierarchicalLifetimeManager(),
                new InjectionFactory(x => new BaseEntityFrameworkRepository<SchedulerDashboardUser>(container.Resolve<DbContext>(SystemSportDataRepository))));

            container.RegisterType<IBaseEntityFrameworkRepository<SchedulerTrackingRugbyFixture>, BaseEntityFrameworkRepository<SchedulerTrackingRugbyFixture>>(
                new HierarchicalLifetimeManager(),
                new InjectionFactory(x => new BaseEntityFrameworkRepository<SchedulerTrackingRugbyFixture>(container.Resolve<DbContext>(SystemSportDataRepository))));

            container.RegisterType<IBaseEntityFrameworkRepository<SchedulerTrackingRugbySeason>, BaseEntityFrameworkRepository<SchedulerTrackingRugbySeason>>(
                new HierarchicalLifetimeManager(),
                new InjectionFactory(x => new BaseEntityFrameworkRepository<SchedulerTrackingRugbySeason>(container.Resolve<DbContext>(SystemSportDataRepository))));

            container.RegisterType<IBaseEntityFrameworkRepository<SchedulerTrackingRugbyTournament>, BaseEntityFrameworkRepository<SchedulerTrackingRugbyTournament>>(
                new HierarchicalLifetimeManager(),
                new InjectionFactory(x => new BaseEntityFrameworkRepository<SchedulerTrackingRugbyTournament>(container.Resolve<DbContext>(SystemSportDataRepository))));
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

                container.RegisterType<ITemporaryExampleMongoDbRepository, TemporaryExampleMongoDbRepository>();

                container.RegisterType<ISchedulerClientService, SchedulerClientService>();
            }
        }
    }
}