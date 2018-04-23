namespace SuperSportDataEngine.Repository.MongoDb.PayloadData.Repositories
{
    using System.Configuration;
    using System.Threading.Tasks;
    using MongoDB.Bson;
    using MongoDB.Driver;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.Stats.Models.Motorsport;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.MongoDb.PayloadData.Interfaces;
    using SuperSportDataEngine.Common.Logging;

    public class MongoDbMotorsportRepository : IMongoDbMotorsportRepository
    {
        private readonly IMongoClient _mongoClient;
        private readonly ILoggingService _logger;

        private readonly string _mongoDatabaseName;

        public MongoDbMotorsportRepository(
            IMongoClient mongoClient, 
            ILoggingService logger)
        {
            _mongoClient = mongoClient;
            _logger = logger;
            _mongoDatabaseName = ConfigurationManager.AppSettings["MongoDbName"];
        }

        private async Task Save<T>(T data)
            where T : MotorsportEntitiesResponse
        {
            if (data == null)
                return;

            // Get the Mongo DB.
            var db = _mongoClient.GetDatabase(_mongoDatabaseName);
            if (db == null)
            {
                await _logger.Error("MongoDbIsNull", "Mongo db object is null.");
                return;
            }

            try
            {
                bool isMongoLive = db.RunCommandAsync((Command<BsonDocument>)"{ping:1}").Wait(1000);

                if (!isMongoLive)
                {
#if (!DEBUG)
                    _logger.Error("MongoDbCannotConnect", "Unable to connect to MongoDB.");
#endif
                    return;
                }
            }
            catch (System.Exception)
            {
            }

            // Add to the collection.
            var collection = db.GetCollection<ApiResult>("motorsport_entities");
            await collection.InsertOneAsync(data.apiResults[0]);
        }

        public async Task Save(MotorsportEntitiesResponse leagues) => await Save(data: leagues);
    }
}
