using System;
using System.Threading;

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
        private readonly int _maxRetryCount;
        private readonly int _maxMillisecondsWaitBeforeMongoDbNextRetry;

        public MongoDbMotorsportRepository(
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

        private async Task Save<T>(T data)
            where T : MotorsportEntitiesResponse
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
                var collection = db.GetCollection<ApiResult>("motorsport_entities");
                await collection.InsertOneAsync(data.apiResults[0]);
            }
            catch (MongoConnectionException connectionException)
            {
                Thread.Sleep(_maxMillisecondsWaitBeforeMongoDbNextRetry);
                persistAttemptsCount++;
                if (persistAttemptsCount >= _maxRetryCount)
                {
                    await _logger.Warn(
                        "MongoDbSave.Motorsport",
                        connectionException,
                        $"Cannot save data to MongoDB. " +
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
                    "MongoDbSave.Motorsport",
                    exception,
                    $"Cannot save data to MongoDB." +
                    $"Message: {exception.Message}\n" +
                    $"StackTrace: {exception.StackTrace}\n" +
                    $"Inner Exception {exception.InnerException}\n");
            }
        }

        public async Task Save(MotorsportEntitiesResponse leagues) => await Save(data: leagues);
    }
}
