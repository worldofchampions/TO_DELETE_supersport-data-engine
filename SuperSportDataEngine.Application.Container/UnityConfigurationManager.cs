namespace SuperSportDataEngine.Application.Container
{
    using Microsoft.Practices.Unity;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.MongoDb.PayloadData.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Services;
    using SuperSportDataEngine.Repository.MongoDb.PayloadData.Repositories;

    // TODO: [Davide] Add a feature to apply registrations according to the running application scope.
    public static class UnityConfigurationManager
    {
        public static void RegisterTypes(IUnityContainer container)
        {
            container.RegisterType<ITemporaryExampleService, TemporaryExampleService>();
            container.RegisterType<ITemporaryExampleMongoDbRepository, TemporaryExampleMongoDbRepository>();
            container.RegisterType<IIngestWorkerService, IngestWorkerService>();
        }
    }
}