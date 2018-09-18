using System.Collections.Generic;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.Stats.Models.Tennis.TennisLeagueTournamentsResponse;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.Stats.Models.Tennis.TennisParticipantsResponse;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.Stats.Models.Tennis.TennisRankingsResponse;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.Stats.Models.Tennis.TennisResultsResponse;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.Stats.Models.Tennis.TennisSurfaceTypesResponse;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.Stats.Models.Tennis.TennisVenuesResponse;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models.Tennis;

namespace SuperSportDataEngine.Repository.MongoDb.PayloadData.Repositories
{
    using System.Configuration;
    using System.Threading.Tasks;
    using MongoDB.Bson;
    using MongoDB.Driver;
    using ApplicationLogic.Boundaries.Gateway.Http.Stats.Models.Tennis;
    using ApplicationLogic.Boundaries.Repository.MongoDb.PayloadData.Interfaces;
    using ApplicationLogic.Boundaries.Gateway.Http.Stats.Models.Tennis.TennisLeaguesResponse;
    using Common.Logging;

    public class MongoDbTennisRepository : IMongoDbTennisRepository
    {
        private readonly IMongoClient _mongoClient;
        private readonly ILoggingService _logger;

        private readonly string _mongoDatabaseName;

        public MongoDbTennisRepository(
            IMongoClient mongoClient, 
            ILoggingService logger)
        {
            _mongoClient = mongoClient;
            _logger = logger;
            _mongoDatabaseName = ConfigurationManager.AppSettings["MongoDbName"];
        }

        public async Task Save(TennisLeaguesResponse data)
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

                // Add to the collection.
                var collection = db.GetCollection<ApplicationLogic.Boundaries.Gateway.Http.Stats.Models.Tennis.TennisLeaguesResponse.ApiResult>("tennis_leagues");
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

        public async Task Save(TennisLeagueTournamentsResponse data)
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

                // Add to the collection.
                var collection = db.GetCollection<ApplicationLogic.Boundaries.Gateway.Http.Stats.Models.Tennis.TennisLeagueTournamentsResponse.ApiResult>("tennis_tournaments");
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

        public async Task Save(TennisRankingsResponse data)
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

                // Add to the collection.
                var collection = db.GetCollection<ApplicationLogic.Boundaries.Gateway.Http.Stats.Models.Tennis.TennisRankingsResponse.ApiResult>("tennis_rankings");
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

        public async Task Save(TennisSeasonsResponse data)
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

                // Add to the collection.
                var collection = db.GetCollection<ApplicationLogic.Boundaries.Gateway.Http.Stats.Models.Tennis.ApiResult>("tennis_seasons");
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

        public async Task Save(TennisVenuesResponse data)
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

                // Add to the collection.
                var collection = db.GetCollection<ApplicationLogic.Boundaries.Gateway.Http.Stats.Models.Tennis.TennisVenuesResponse.ApiResult>("tennis_venues");
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

        public async Task Save(TennisSurfaceTypesResponse data)
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

                // Add to the collection.
                var collection = db.GetCollection<ApplicationLogic.Boundaries.Gateway.Http.Stats.Models.Tennis.TennisSurfaceTypesResponse.ApiResults>("tennis_surfacetypes");
                await collection.InsertOneAsync(data.apiResults);
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

        public async Task Save(TennisParticipantsResponse data)
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

                // Add to the collection.
                var collection = db.GetCollection<ApplicationLogic.Boundaries.Gateway.Http.Stats.Models.Tennis.TennisParticipantsResponse.ApiResult>("tennis_participants");
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

        public async Task Save(TennisResultsResponse data)
        {
            try
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

                bool isMongoLive = db.RunCommandAsync((Command<BsonDocument>)"{ping:1}").Wait(1000);

                if (!isMongoLive)
                {
#if (!DEBUG)
                    _logger.Error("MongoDbCannotConnect", "Unable to connect to MongoDB.");
#endif
                    return;
                }

                // Add to the collection.
                var collection = db.GetCollection<ApplicationLogic.Boundaries.Gateway.Http.Stats.Models.Tennis.TennisResultsResponse.ApiResult>("tennis_results");
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
    }
}
