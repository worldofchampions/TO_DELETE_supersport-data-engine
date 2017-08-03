namespace SuperSportDataEngine.ApplicationLogic.Services
{
    using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.MongoDb.PayloadData.Interfaces;
    using System;

    // TODO: [Davide] Temporary example reference code for team (delete this later).
    public class TemporaryExampleService : ITemporaryExampleService
    {
        private readonly ITemporaryExampleMongoDbRepository _temporaryExampleMongoDbRepository;

        public TemporaryExampleService(ITemporaryExampleMongoDbRepository temporaryExampleMongoDbRepository)
        {
            _temporaryExampleMongoDbRepository = temporaryExampleMongoDbRepository;
        }

        public string HelloMessage()
        {
            return "Hello from: (SuperSportDataEngine.ApplicationLogic.Services).TemporaryExampleService"
                + Environment.NewLine + _temporaryExampleMongoDbRepository.HelloMessage();
        }
    }
}