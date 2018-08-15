using System;
using MongoDB.Driver;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.ResponseModels;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.MongoDb.PayloadData.Interfaces;
using System.Configuration;
using System.Threading;
using MongoDB.Bson;
using SuperSportDataEngine.Common.Logging;

namespace SuperSportDataEngine.Repository.MongoDb.PayloadData.Repositories
{
    public class MongoDbRugbyRepository : IMongoDbRugbyRepository
    {
        private readonly IMongoClient _mongoClient;
        private readonly ILoggingService _logger;

        private readonly string _mongoDatabaseName;
        private readonly int _maxRetryCount;
        private readonly int _maxMillisecondsWaitBeforeMongoDbNextRetry;

        public MongoDbRugbyRepository(
            IMongoClient mongoClient,
            ILoggingService logger)
        {
            _mongoClient = mongoClient;
            _logger = logger;
            _mongoDatabaseName = ConfigurationManager.AppSettings["MongoDbName"];
            _maxRetryCount = int.Parse(ConfigurationManager.AppSettings["MongoDBRetryCount"]);
            _maxMillisecondsWaitBeforeMongoDbNextRetry =
                int.Parse(ConfigurationManager.AppSettings["MongoDBWaitTimeInMillisecondsBeforeNextRetry"]);
        }

        private async void Save<T>(T data, string collectionName)
        {
            var persistAttemptsCount = 0;
            PersistIntoMongo:

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
                    await _logger.Error("MongoDbIsNull",
                        null, "Mongo db object is null.");
                    return;
                }

                // Add to the collection.
                var collection = db.GetCollection<T>(collectionName);
                await collection.InsertOneAsync(data);
            }
            catch (MongoConnectionException connectionException)
            {
                Thread.Sleep(_maxMillisecondsWaitBeforeMongoDbNextRetry);
                persistAttemptsCount++;
                if (persistAttemptsCount >= _maxRetryCount)
                {
                    await _logger.Warn(
                        "MongoDbSave.Rugby",
                        connectionException,
                        "Cannot save data to MongoDB." +
                        $"Message: {connectionException.Message}\n" +
                        $"StackTrace: {connectionException.StackTrace}\n" +
                        $"Inner Exception {connectionException.InnerException}\n");

                    return;
                }

                goto PersistIntoMongo;
            }
            catch (System.Exception exception)
            {
                await _logger.Warn(
                    "MongoDbSave.Rugby", 
                    exception,
                    "Cannot save data to MongoDB. " +
                    $"Message: {exception.Message}\n" +
                    $"StackTrace: {exception.StackTrace}\n" +
                    $"Inner Exception {exception.InnerException}\n");
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