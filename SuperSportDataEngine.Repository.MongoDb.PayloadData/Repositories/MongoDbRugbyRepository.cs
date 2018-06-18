using MongoDB.Driver;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.ResponseModels;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.MongoDb.PayloadData.Interfaces;
using System.Configuration;
using MongoDB.Bson;
using SuperSportDataEngine.Common.Logging;

namespace SuperSportDataEngine.Repository.MongoDb.PayloadData.Repositories
{
    public class MongoDbRugbyRepository : IMongoDbRugbyRepository
    {
        private readonly IMongoClient _mongoClient;
        private readonly ILoggingService _logger;

        private readonly string _mongoDatabaseName;

        public MongoDbRugbyRepository(
            IMongoClient mongoClient,
            ILoggingService logger)
        {
            _mongoClient = mongoClient;
            _logger = logger;
            _mongoDatabaseName = ConfigurationManager.AppSettings["MongoDbName"];

        }

        private async void Save<T>(T data, string collectionName)
        {
            // Get the Mongo DB.
            var db = _mongoClient.GetDatabase(_mongoDatabaseName);
            if (db == null)
            {
                await _logger.Error("MongoDbIsNull", "Mongo db object is null.");
                return;
            }

            try
            {
                var isMongoLive = db.RunCommandAsync((Command<BsonDocument>)"{ping:1}").Wait(1000);

                if (!isMongoLive)
                {
#if (!DEBUG)
                    _logger.Error("MongoDbCannotConnect", "Unable to connect to MongoDB.");
#endif
                    return;
                }

                // Add to the collection.
                var collection = db.GetCollection<T>(collectionName);
                await collection.InsertOneAsync(data);
            }
            catch (System.Exception exception)
            {
                await _logger.Warn(
                    "MongoDbSave.Rugby", 
                    "Cannot save data to MongoDB. " +
                     "Message: \n" + exception.Message +
                    "StackTrace: \n" + exception.StackTrace +
                    "Inner Exception \n" + exception.InnerException);
            }
        }

        public void SaveEntities(RugbyEntitiesResponse entitiesResponse) => Save(entitiesResponse, "entities");

        public void Save(RugbyFixturesResponse fixturesResponse) => Save(fixturesResponse, "fixtures");

        public void Save(RugbyFlatLogsResponse logsResponse) => Save(logsResponse, "logs");

        public void Save(RugbyGroupedLogsResponse logsResponse) => Save(logsResponse, "logs");

        public void Save(RugbyMatchStatsResponse matchStatsResponse) => Save(matchStatsResponse, "matchStats");

        public void Save(RugbyEventsFlowResponse eventsFlowResponse) => Save(eventsFlowResponse, "eventsFlow");
    }
}