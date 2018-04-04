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

        public void SaveEntities(RugbyEntitiesResponse entitiesResponse)
        {
            // Get the Mongo DB.
            var db = _mongoClient.GetDatabase(_mongoDatabaseName);
            if (db == null)
            {
                _logger.Error("MongoDbIsNull", "Mongo db object is null.");
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
            var collection = db.GetCollection<RugbyEntitiesResponse>("entities");
            collection.InsertOneAsync(entitiesResponse);
        }

        public void Save(RugbyFixturesResponse fixturesResponse)
        {
            // Get the Mongo DB.
            var db = _mongoClient.GetDatabase(_mongoDatabaseName);
            if (db == null)
            {
                _logger.Error("MongoDbIsNull", "Mongo db object is null.");
                return;
            }

            try
            {
                bool isMongoLive = db.RunCommandAsync((Command<BsonDocument>)"{ping:1}").Wait(1000);

                if (!isMongoLive)
                {
#if(!DEBUG)
                    _logger.Error("MongoDbCannotConnect" ,"Unable to connect to MongoDB.");
#endif
                    return;
                }
            }
            catch (System.Exception e)
            {
            }

            // Add to the collection.
            var collection = db.GetCollection<RugbyFixturesResponse>("fixtures");
            collection.InsertOneAsync(fixturesResponse);
        }

        public void Save(RugbyFlatLogsResponse logsResponse)
        {
            // Get the Mongo DB.
            var db = _mongoClient.GetDatabase(_mongoDatabaseName);
            if (db == null)
            {
                _logger.Error("MongoDbIsNull", "Mongo db object is null.");
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
            catch (System.Exception e)
            {
            }

            // Add to the collection.
            var collection = db.GetCollection<RugbyFlatLogsResponse>("logs");
            collection.InsertOneAsync(logsResponse);
        }

        public void Save(RugbyGroupedLogsResponse logsResponse)
        {
            // Get the Mongo DB.
            var db = _mongoClient.GetDatabase(_mongoDatabaseName);
            if (db == null)
            {
                _logger.Error("MongoDbIsNull", "Mongo db object is null.");
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
            catch (System.Exception e)
            {
            }

            // Add to the collection.
            var collection = db.GetCollection<RugbyGroupedLogsResponse>("logs");
            collection.InsertOneAsync(logsResponse);
        }

        public void Save(RugbyMatchStatsResponse matchStatsResponse)
        {
            // Get the Mongo DB.
            var db = _mongoClient.GetDatabase(_mongoDatabaseName);
            if(db == null)
            {
                _logger.Error("MongoDbIsNull", "Mongo db object is null.");
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
            catch (System.Exception e)
            {
            }

            // Add to the collection.
            var collection = db.GetCollection<RugbyMatchStatsResponse>("matchStats");
            collection.InsertOneAsync(matchStatsResponse);
        }

        public void Save(RugbyEventsFlowResponse eventsFlowResponse)
        {
            // Get the Mongo DB.
            var db = _mongoClient.GetDatabase(_mongoDatabaseName);
            if (db == null)
            {
                _logger.Error("MongoDbIsNull", "Mongo db object is null.");
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
            catch (System.Exception e)
            {
            }

            // Add to the collection.
            var collection = db.GetCollection<RugbyEventsFlowResponse>("eventsFlow");
            collection.InsertOneAsync(eventsFlowResponse);
        }
    }
}