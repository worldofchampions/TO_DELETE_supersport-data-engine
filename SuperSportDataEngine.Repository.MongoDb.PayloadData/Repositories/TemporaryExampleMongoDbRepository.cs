namespace SuperSportDataEngine.Repository.MongoDb.PayloadData.Repositories
{
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.MongoDb.PayloadData.Interfaces;

    // TODO: [Davide] Temporary example reference code for team (delete this later).
    public class TemporaryExampleMongoDbRepository : ITemporaryExampleMongoDbRepository
    {
        public string HelloMessage()
        {
            return "Hello from: (SuperSportDataEngine.Repository.MongoDb.PayloadData.Repositories).TemporaryExampleMongoDbRepository";
        }
    }
}