namespace SuperSportDataEngine.Repository.MongoDb.PayloadData.Repositories
{
    using System.Configuration;
    using System.Threading.Tasks;
    using MongoDB.Bson;
    using MongoDB.Driver;
    using ApplicationLogic.Boundaries.Gateway.Http.Stats.Models.Motorsport;
    using ApplicationLogic.Boundaries.Repository.MongoDb.PayloadData.Interfaces;
    using Common.Logging;

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
            try
            {
                if (data == null)
                    return;

                if (_mongoClient == null)
                    return;

                // Get the Mongo DB.
                var db = _mongoClient.GetDatabase(_mongoDatabaseName);
                if (db == null)
                {
                    await _logger.Error("MongoDbIsNull", "Mongo db object is null.");
                    return;
                }

                //[TODO] Ronald: Disabling checking if mongo is live. 
                //[TODO] I'm instead going to add retryWrites=true to the connectionString for MongoDB
                //                var isMongoLive = db.RunCommandAsync((Command<BsonDocument>)"{ping:1}").Wait(1000);

                //                if (!isMongoLive)
                //                {
                //#if (!DEBUG)
                //                    await _logger.Error("MongoDbCannotConnect", "Unable to connect to MongoDB.");
                //#endif
                //                    return;
                //                }

                // Add to the collection.
                var collection = db.GetCollection<ApiResult>("motorsport_entities");
                await collection.InsertOneAsync(data.apiResults[0]);
            }
            catch (System.Exception exception)
            {
                await _logger.Warn(
                    "MongoDbSave.Motorsport",
                    "Cannot save data to MongoDB. " +
                     "Message: \n" + exception.Message +
                    "StackTrace: \n" + exception.StackTrace +
                    "Inner Exception \n" + exception.InnerException);
            }
        }

        public async Task Save(MotorsportEntitiesResponse leagues) => await Save(data: leagues);
    }
}
