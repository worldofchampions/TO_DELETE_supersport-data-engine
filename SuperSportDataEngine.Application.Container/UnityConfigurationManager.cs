namespace SuperSportDataEngine.Application.Container
{
    using Microsoft.Practices.Unity;
    using StackExchange.Redis;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.MongoDb.PayloadData.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Services;
    using SuperSportDataEngine.Common.Boundaries;
    using SuperSportDataEngine.Common.Caching;
    using SuperSportDataEngine.Repository.EntityFramework.Common.Repositories.Base;
    using SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Context;
    using SuperSportDataEngine.Repository.EntityFramework.SystemSportData.Context;
    using SuperSportDataEngine.Repository.MongoDb.PayloadData.Repositories;
    using System.Data.Entity;
    using System.Web.Configuration;

    // TODO: [Davide] Add a feature to apply registrations according to the running application scope.
    public static class UnityConfigurationManager
    {
        private const string PublicSportDataRepository = "PublicSportDataRepository";
        private const string SystemSportDataRepository = "SystemSportDataRepository";

        public static void RegisterTypes(IUnityContainer container)
        {
            ApplyRegistrationsForApplicationLogic(container);
            ApplyRegistrationsForRepositoryEntityFrameworkPublicSportData(container);
            ApplyRegistrationsForRepositoryEntityFrameworkSystemSportData(container);
            ApplyRegistrationsForRepositoryMongoDbPayloadData(container);
        }

        private static void ApplyRegistrationsForApplicationLogic(IUnityContainer container)
        {
            container.RegisterType<ITemporaryExampleService, TemporaryExampleService>();
            container.RegisterType<IRugbyService, RugbyService>();
        }

        private static void ApplyRegistrationsForRepositoryEntityFrameworkPublicSportData(IUnityContainer container)
        {
            container.RegisterType<DbContext, PublicSportDataContext>(PublicSportDataRepository);

            container.RegisterType<IBaseEntityFrameworkRepository<Player>, BaseEntityFrameworkRepository<Player>>(
                new InjectionFactory((x) => new BaseEntityFrameworkRepository<Player>(container.Resolve<DbContext>(PublicSportDataRepository))));

            #region Cache

            container.RegisterInstance<ICache>(new Cache(ConnectionMultiplexer.Connect(WebConfigurationManager.ConnectionStrings["Redis"].ConnectionString)));

            #endregion Cache

            container.RegisterType<IBaseEntityFrameworkRepository<Sport>, BaseEntityFrameworkRepository<Sport>>(
                new InjectionFactory((x) => new BaseEntityFrameworkRepository<Sport>(container.Resolve<DbContext>(PublicSportDataRepository))));
        }

        private static void ApplyRegistrationsForRepositoryEntityFrameworkSystemSportData(IUnityContainer container)
        {
            container.RegisterType<DbContext, SystemSportDataContext>(SystemSportDataRepository);

            container.RegisterType<IBaseEntityFrameworkRepository<SportTournament>, BaseEntityFrameworkRepository<SportTournament>>(
                new InjectionFactory((x) => new BaseEntityFrameworkRepository<SportTournament>(container.Resolve<DbContext>(SystemSportDataRepository))));
        }

        private static void ApplyRegistrationsForRepositoryMongoDbPayloadData(IUnityContainer container)
        {
            container.RegisterType<ITemporaryExampleMongoDbRepository, TemporaryExampleMongoDbRepository>();
            container.RegisterType<IIngestWorkerService, IngestWorkerService>();
            container.RegisterType<IMongoDbRepository, MongoDbRepository>();
        }
    }
}