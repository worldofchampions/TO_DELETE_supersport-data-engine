namespace SuperSportDataEngine.Application.Container
{
    using Microsoft.Practices.Unity;
    using MongoDB.Driver;
    using StackExchange.Redis;
    using SuperSportDataEngine.Application.Container.Enums;
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
            container.RegisterType<DbContext, PublicSportDataContext>(PublicSportDataRepository);

            container.RegisterType<IBaseEntityFrameworkRepository<Player>, BaseEntityFrameworkRepository<Player>>(
                new InjectionFactory(x => new BaseEntityFrameworkRepository<Player>(container.Resolve<DbContext>(PublicSportDataRepository))));

            container.RegisterType<IBaseEntityFrameworkRepository<Sport>, BaseEntityFrameworkRepository<Sport>>(
                new InjectionFactory(x => new BaseEntityFrameworkRepository<Sport>(container.Resolve<DbContext>(PublicSportDataRepository))));

            container.RegisterType<IBaseEntityFrameworkRepository<Log>, BaseEntityFrameworkRepository<Log>>(
                new InjectionFactory(x => new BaseEntityFrameworkRepository<Log>(container.Resolve<DbContext>(PublicSportDataRepository))));
        }

        private static void ApplyRegistrationsForRepositoryEntityFrameworkSystemSportData(IUnityContainer container)
        {
            container.RegisterType<DbContext, SystemSportDataContext>(SystemSportDataRepository);

            container.RegisterType<IBaseEntityFrameworkRepository<DataProvider>, BaseEntityFrameworkRepository<DataProvider>>(
                new InjectionFactory(x => new BaseEntityFrameworkRepository<DataProvider>(container.Resolve<DbContext>(SystemSportDataRepository))));

            container.RegisterType<IBaseEntityFrameworkRepository<LegacyAuthFeedConsumer>, BaseEntityFrameworkRepository<LegacyAuthFeedConsumer>>(
                new InjectionFactory(x => new BaseEntityFrameworkRepository<LegacyAuthFeedConsumer>(container.Resolve<DbContext>(SystemSportDataRepository))));

            container.RegisterType<IBaseEntityFrameworkRepository<LegacyZoneSite>, BaseEntityFrameworkRepository<LegacyZoneSite>>(
                new InjectionFactory(x => new BaseEntityFrameworkRepository<LegacyZoneSite>(container.Resolve<DbContext>(SystemSportDataRepository))));

            container.RegisterType<IBaseEntityFrameworkRepository<RugbySeason>, BaseEntityFrameworkRepository<RugbySeason>>(
                new InjectionFactory(x => new BaseEntityFrameworkRepository<RugbySeason>(container.Resolve<DbContext>(SystemSportDataRepository))));

            container.RegisterType<IBaseEntityFrameworkRepository<RugbyTournament>, BaseEntityFrameworkRepository<RugbyTournament>>(
                new InjectionFactory(x => new BaseEntityFrameworkRepository<RugbyTournament>(container.Resolve<DbContext>(SystemSportDataRepository))));

            container.RegisterType<IBaseEntityFrameworkRepository<SchedulerDashboardUser>, BaseEntityFrameworkRepository<SchedulerDashboardUser>>(
                new InjectionFactory(x => new BaseEntityFrameworkRepository<SchedulerDashboardUser>(container.Resolve<DbContext>(SystemSportDataRepository))));

            container.RegisterType<IBaseEntityFrameworkRepository<SportTournament>, BaseEntityFrameworkRepository<SportTournament>>(
                new InjectionFactory(x => new BaseEntityFrameworkRepository<SportTournament>(container.Resolve<DbContext>(SystemSportDataRepository))));
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
                container.RegisterType<IMongoClient, MongoClient>(
                    new ContainerControlledLifetimeManager(),
                    new InjectionFactory((x) => new MongoClient(ConfigurationManager.ConnectionStrings["MongoDB"].ConnectionString)));

                container.RegisterType<IMongoDbRugbyRepository, MongoDbRugbyRepository>();
                container.RegisterType<IRugbyIngestWorkerService, RugbyIngestWorkerService>();
                container.RegisterType<ITemporaryExampleMongoDbRepository, TemporaryExampleMongoDbRepository>();
                container.RegisterType<ISchedulerClientService, SchedulerClientService>();
            }
        }
    }
}