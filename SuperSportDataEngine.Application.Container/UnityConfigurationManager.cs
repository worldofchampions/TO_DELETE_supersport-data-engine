namespace SuperSportDataEngine.Application.Container
{
    using Microsoft.Practices.Unity;
    using StackExchange.Redis;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.MongoDb.PayloadData.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Services;
    using SuperSportDataEngine.Common.Boundaries;
    using SuperSportDataEngine.Common.Caching;
    using SuperSportDataEngine.Repository.MongoDb.PayloadData.Repositories;
    using System.Web.Configuration;
    using SuperSportDataEngine.Gateway.Http.StatsProzone.Services;

    // TODO: [Davide] Add a feature to apply registrations according to the running application scope.
    public static class UnityConfigurationManager
    {
        public static void RegisterTypes(IUnityContainer container)
        {
            container.RegisterType<IStatsProzoneIngestService, StatsProzoneIngestService>();
            #region Services
            
            container.RegisterType<ITemporaryExampleService, TemporaryExampleService>();
            container.RegisterType<IRugbyService, RugbyService>();

            #endregion Services

            #region Cache

            container.RegisterInstance<ICache>(new Cache(ConnectionMultiplexer.Connect(WebConfigurationManager.ConnectionStrings["Redis"].ConnectionString)));

            #endregion Cache

            container.RegisterType<ITemporaryExampleMongoDbRepository, TemporaryExampleMongoDbRepository>();
            container.RegisterType<IIngestWorkerService, IngestWorkerService>();
            container.RegisterType<IMongoDbRepository, MongoDbRepository>();
        }
    }
}