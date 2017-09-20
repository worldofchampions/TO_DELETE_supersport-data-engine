namespace SuperSportDataEngine.Application.Container
{
    using Hangfire;
    using Hangfire.SqlServer;
    using Microsoft.Practices.Unity;
    using MongoDB.Driver;
    using StackExchange.Redis;
    using SuperSportDataEngine.Application.Container.Enums;
    using SuperSportDataEngine.Application.Service.Common.Hangfire.Configuration;
    using SuperSportDataEngine.Application.WebApi.Common.Caching;
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

    public static class UnityConfigurationManager
    {
        private const string PublicSportDataRepository = "PublicSportDataRepository";
        private const string SystemSportDataRepository = "SystemSportDataRepository";

        public static void RegisterTypes(IUnityContainer container, ApplicationScope applicationScope)
        {
            ApplyRegistrationsForApplicationLogic(container, applicationScope);
            ApplyRegistrationsForGatewayHttpCommon(container, applicationScope);
            ApplyRegistrationsForGatewayHttpStatsProzone(container, applicationScope);
            ApplyRegistrationsForRepositoryEntityFrameworkPublicSportData(container);
            ApplyRegistrationsForRepositoryEntityFrameworkSystemSportData(container);
            ApplyRegistrationsForRepositoryMongoDbPayloadData(container, applicationScope);
        }

        private static void ApplyRegistrationsForApplicationLogic(IUnityContainer container, ApplicationScope applicationScope)
        {
            container.RegisterType<ITemporaryExampleService, TemporaryExampleService>();
            container.RegisterType<IRugbyService, RugbyService>();

            if (applicationScope == ApplicationScope.WebApiLegacyFeed ||
                applicationScope == ApplicationScope.WebApiSystemApi)
            {
                container.RegisterType<ILegacyAuthService, LegacyAuthService>();
            }
        }

        private static void ApplyRegistrationsForGatewayHttpCommon(IUnityContainer container, ApplicationScope applicationScope)
        {
            if (applicationScope == ApplicationScope.WebApiLegacyFeed ||
                applicationScope == ApplicationScope.WebApiPublicApi)
            {
                container.RegisterInstance<ICache>(new Cache(ConnectionMultiplexer.Connect(WebConfigurationManager.ConnectionStrings["Redis"].ConnectionString)));
            }
        }

        private static void ApplyRegistrationsForGatewayHttpStatsProzone(IUnityContainer container, ApplicationScope applicationScope)
        {
            // TODO: [Davide] Conceptually this scope should be restricted to execution on ServiceSchedulerIngestServer only. Finalize this condition after determining what the Hangfire dependencies are for job definition creation etc.
            //if (applicationScope == ApplicationScope.ServiceSchedulerIngestServer)
            //{
            //    container.RegisterType<IStatsProzoneRugbyIngestService, StatsProzoneRugbyIngestService>();
            //}

            if (applicationScope == ApplicationScope.ServiceSchedulerClient ||
                applicationScope == ApplicationScope.ServiceSchedulerIngestServer)
            {
                container.RegisterType<IStatsProzoneRugbyIngestService, StatsProzoneRugbyIngestService>();
            }
        }

        private static void ApplyRegistrationsForRepositoryEntityFrameworkPublicSportData(IUnityContainer container)
        {
            container.RegisterType<DbContext, PublicSportDataContext>(PublicSportDataRepository, new PerThreadLifetimeManager());

            container.RegisterType<IBaseEntityFrameworkRepository<RugbyFixture>, BaseEntityFrameworkRepository<RugbyFixture>>(
                new ContainerControlledLifetimeManager(),
                new InjectionFactory(x => new BaseEntityFrameworkRepository<RugbyFixture>(container.Resolve<DbContext>(PublicSportDataRepository))));

            container.RegisterType<IBaseEntityFrameworkRepository<RugbyLog>, BaseEntityFrameworkRepository<RugbyLog>>(
                new ContainerControlledLifetimeManager(),
                new InjectionFactory(x => new BaseEntityFrameworkRepository<RugbyLog>(container.Resolve<DbContext>(PublicSportDataRepository))));

            container.RegisterType<IBaseEntityFrameworkRepository<RugbyPlayer>, BaseEntityFrameworkRepository<RugbyPlayer>>(
                new ContainerControlledLifetimeManager(),
                new InjectionFactory(x => new BaseEntityFrameworkRepository<RugbyPlayer>(container.Resolve<DbContext>(PublicSportDataRepository))));

            container.RegisterType<IBaseEntityFrameworkRepository<RugbySeason>, BaseEntityFrameworkRepository<RugbySeason>>(
                new ContainerControlledLifetimeManager(),
                new InjectionFactory(x => new BaseEntityFrameworkRepository<RugbySeason>(container.Resolve<DbContext>(PublicSportDataRepository))));

            container.RegisterType<IBaseEntityFrameworkRepository<RugbyTeam>, BaseEntityFrameworkRepository<RugbyTeam>>(
                new ContainerControlledLifetimeManager(),
                new InjectionFactory(x => new BaseEntityFrameworkRepository<RugbyTeam>(container.Resolve<DbContext>(PublicSportDataRepository))));

            container.RegisterType<IBaseEntityFrameworkRepository<RugbyTournament>, BaseEntityFrameworkRepository<RugbyTournament>>(
                new ContainerControlledLifetimeManager(),
                new InjectionFactory(x => new BaseEntityFrameworkRepository<RugbyTournament>(container.Resolve<DbContext>(PublicSportDataRepository))));

            container.RegisterType<IBaseEntityFrameworkRepository<RugbyVenue>, BaseEntityFrameworkRepository<RugbyVenue>>(
                new ContainerControlledLifetimeManager(),
                new InjectionFactory(x => new BaseEntityFrameworkRepository<RugbyVenue>(container.Resolve<DbContext>(PublicSportDataRepository))));
        }

        private static void ApplyRegistrationsForRepositoryEntityFrameworkSystemSportData(IUnityContainer container)
        {
            container.RegisterType<DbContext, SystemSportDataContext>(SystemSportDataRepository, new PerThreadLifetimeManager());

            container.RegisterType<IBaseEntityFrameworkRepository<LegacyAuthFeedConsumer>, BaseEntityFrameworkRepository<LegacyAuthFeedConsumer>>(
                new ContainerControlledLifetimeManager(),
                new InjectionFactory(x => new BaseEntityFrameworkRepository<LegacyAuthFeedConsumer>(container.Resolve<DbContext>(SystemSportDataRepository))));

            container.RegisterType<IBaseEntityFrameworkRepository<LegacyZoneSite>, BaseEntityFrameworkRepository<LegacyZoneSite>>(
                new ContainerControlledLifetimeManager(),
                new InjectionFactory(x => new BaseEntityFrameworkRepository<LegacyZoneSite>(container.Resolve<DbContext>(SystemSportDataRepository))));

            container.RegisterType<IBaseEntityFrameworkRepository<SchedulerDashboardUser>, BaseEntityFrameworkRepository<SchedulerDashboardUser>>(
                new ContainerControlledLifetimeManager(),
                new InjectionFactory(x => new BaseEntityFrameworkRepository<SchedulerDashboardUser>(container.Resolve<DbContext>(SystemSportDataRepository))));

            container.RegisterType<IBaseEntityFrameworkRepository<SchedulerTrackingRugbyFixture>, BaseEntityFrameworkRepository<SchedulerTrackingRugbyFixture>>(
                new ContainerControlledLifetimeManager(),
                new InjectionFactory(x => new BaseEntityFrameworkRepository<SchedulerTrackingRugbyFixture>(container.Resolve<DbContext>(SystemSportDataRepository))));

            container.RegisterType<IBaseEntityFrameworkRepository<SchedulerTrackingRugbySeason>, BaseEntityFrameworkRepository<SchedulerTrackingRugbySeason>>(
                new ContainerControlledLifetimeManager(),
                new InjectionFactory(x => new BaseEntityFrameworkRepository<SchedulerTrackingRugbySeason>(container.Resolve<DbContext>(SystemSportDataRepository))));

            container.RegisterType<IBaseEntityFrameworkRepository<SchedulerTrackingRugbyTournament>, BaseEntityFrameworkRepository<SchedulerTrackingRugbyTournament>>(
                new ContainerControlledLifetimeManager(),
                new InjectionFactory(x => new BaseEntityFrameworkRepository<SchedulerTrackingRugbyTournament>(container.Resolve<DbContext>(SystemSportDataRepository))));
        }

        private static void ApplyRegistrationsForRepositoryMongoDbPayloadData(IUnityContainer container, ApplicationScope applicationScope)
        {
            // TODO: [Davide] Conceptually this scope should be restricted to execution on ServiceSchedulerIngestServer only. Finalize this condition after determining what the Hangfire dependencies are for job definition creation etc.
            //if (applicationScope == ApplicationScope.ServiceSchedulerIngestServer)
            //{
            //    container.RegisterType<IRugbyIngestWorkerService, RugbyIngestWorkerService>();
            //}

            if (applicationScope == ApplicationScope.ServiceSchedulerClient ||
                applicationScope == ApplicationScope.ServiceSchedulerIngestServer)
            {
                container.RegisterType<IRecurringJobManager, RecurringJobManager>(
                    new InjectionFactory((x) => new RecurringJobManager(
                        new SqlServerStorage(ConfigurationManager.ConnectionStrings["SqlDatabase_Hangfire"].ConnectionString))));

                container.RegisterType<IMongoClient, MongoClient>(
                    new ContainerControlledLifetimeManager(),
                    new InjectionFactory((x) => new MongoClient(
                        ConfigurationManager.ConnectionStrings["MongoDB"].ConnectionString)));

                container.RegisterType<IMongoDbRugbyRepository, MongoDbRugbyRepository>();
                container.RegisterType<IRugbyIngestWorkerService, RugbyIngestWorkerService>();
                container.RegisterType<ITemporaryExampleMongoDbRepository, TemporaryExampleMongoDbRepository>();
                container.RegisterType<ISchedulerClientService, SchedulerClientService>();
            }
        }
    }
}