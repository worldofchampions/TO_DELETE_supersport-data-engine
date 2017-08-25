namespace SuperSportDataEngine.Application.Container
{
    using Microsoft.Practices.Unity;
    using MongoDB.Driver;
    using StackExchange.Redis;
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

    // TODO: [Davide] Add a feature to apply registrations according to the running application scope.
    public static class UnityConfigurationManager
    {
        private const string PublicSportDataRepository = "PublicSportDataRepository";
        private const string SystemSportDataRepository = "SystemSportDataRepository";

        public static void RegisterTypes(IUnityContainer container)
        {
            ApplyRegistrationsForCache(container);
            ApplyRegistrationsForApplicationLogic(container);
            ApplyRegistrationsForRepositoryEntityFrameworkPublicSportData(container);
            ApplyRegistrationsForRepositoryEntityFrameworkSystemSportData(container);
            ApplyRegistrationsForStatsProzone(container);
            ApplyRegistrationsForRepositoryMongoDbPayloadData(container);
        }

        private static void ApplyRegistrationsForStatsProzone(IUnityContainer container)
        {
            container.RegisterType<IStatsProzoneRugbyIngestService, StatsProzoneRugbyIngestService>();
        }

        private static void ApplyRegistrationsForCache(IUnityContainer container)
        {
            container.RegisterInstance<ICache>(new Cache(ConnectionMultiplexer.Connect(WebConfigurationManager.ConnectionStrings["Redis"].ConnectionString)));
        }

        private static void ApplyRegistrationsForApplicationLogic(IUnityContainer container)
        {
            container.RegisterType<ITemporaryExampleService, TemporaryExampleService>();
            container.RegisterType<IRugbyService, RugbyService>();
            container.RegisterType<ILegacyAuthService, LegacyAuthService>();
        }

        private static void ApplyRegistrationsForRepositoryEntityFrameworkPublicSportData(IUnityContainer container)
        {
            container.RegisterType<DbContext, PublicSportDataContext>(PublicSportDataRepository);

            container.RegisterType<IBaseEntityFrameworkRepository<Player>, BaseEntityFrameworkRepository<Player>>(
                new InjectionFactory((x) => new BaseEntityFrameworkRepository<Player>(container.Resolve<DbContext>(PublicSportDataRepository))));

            container.RegisterType<IBaseEntityFrameworkRepository<Sport>, BaseEntityFrameworkRepository<Sport>>(
                new InjectionFactory((x) => new BaseEntityFrameworkRepository<Sport>(container.Resolve<DbContext>(PublicSportDataRepository))));

            container.RegisterType<IBaseEntityFrameworkRepository<Log>, BaseEntityFrameworkRepository<Log>>(
                new InjectionFactory((x) => new BaseEntityFrameworkRepository<Log>(container.Resolve<DbContext>(PublicSportDataRepository))));
        }

        private static void ApplyRegistrationsForRepositoryEntityFrameworkSystemSportData(IUnityContainer container)
        {
            container.RegisterType<DbContext, SystemSportDataContext>(SystemSportDataRepository);

            container.RegisterType<IBaseEntityFrameworkRepository<SportTournament>, BaseEntityFrameworkRepository<SportTournament>>(
                new InjectionFactory((x) => new BaseEntityFrameworkRepository<SportTournament>(container.Resolve<DbContext>(SystemSportDataRepository))));

            container.RegisterType<IBaseEntityFrameworkRepository<LegacyAuthFeedConsumer>, BaseEntityFrameworkRepository<LegacyAuthFeedConsumer>>(
              new InjectionFactory((x) => new BaseEntityFrameworkRepository<LegacyAuthFeedConsumer>(container.Resolve<DbContext>(SystemSportDataRepository))));

            container.RegisterType<IBaseEntityFrameworkRepository<LegacyZoneSite>, BaseEntityFrameworkRepository<LegacyZoneSite>>(
               new InjectionFactory((x) => new BaseEntityFrameworkRepository<LegacyZoneSite>(container.Resolve<DbContext>(SystemSportDataRepository))));
        }

        private static void ApplyRegistrationsForRepositoryMongoDbPayloadData(IUnityContainer container)
        {
            container.RegisterType<ITemporaryExampleMongoDbRepository, TemporaryExampleMongoDbRepository>();

            container.RegisterType<IMongoClient, MongoClient>(
                new ContainerControlledLifetimeManager(), 
                new InjectionFactory((x) => new MongoClient(ConfigurationManager.ConnectionStrings["MongoDB"].ConnectionString)));

            container.RegisterType<IMongoDbRugbyRepository, MongoDbRugbyRepository>();
            container.RegisterType<IRugbyIngestWorkerService, RugbyIngestWorkerService>();
            
        }
    }
}