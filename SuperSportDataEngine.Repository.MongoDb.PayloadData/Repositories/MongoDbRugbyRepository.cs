using AutoMapper;
using MongoDB.Driver;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.ResponseModels;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.MongoDb.PayloadData.Interfaces;
using SuperSportDataEngine.Repository.MongoDb.PayloadData.Models.RugbyEntities;
using SuperSportDataEngine.Repository.MongoDb.PayloadData.MappingProfiles;
using SuperSportDataEngine.Repository.MongoDb.PayloadData.Models.RugbyFixtures;
using SuperSportDataEngine.Repository.MongoDb.PayloadData.Models.RugbyLogsFlat;
using SuperSportDataEngine.Repository.MongoDb.PayloadData.Models.RugbyLogsGrouped;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyMatchStats;
using SuperSportDataEngine.Repository.MongoDb.PayloadData.Models.MongoRugbyMatchStats;
using System.Configuration;
using System.Linq;
using System.Reflection;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyEventsFlow;
using SuperSportDataEngine.Repository.MongoDb.PayloadData.Models.MongoRugbyEventsFlow;
using MongoDB.Bson;
using SuperSportDataEngine.Common.Logging;

namespace SuperSportDataEngine.Repository.MongoDb.PayloadData.Repositories
{
    public class MongoDbRugbyRepository : IMongoDbRugbyRepository
    {
        private IMongoClient _mongoClient;
        private ILoggingService _logger;

        private string _mongoDatabaseName;

        public MongoDbRugbyRepository(
            IMongoClient mongoClient,
            ILoggingService logger)
        {
            _mongoClient = mongoClient;
            _logger = logger;
            _mongoDatabaseName = ConfigurationManager.AppSettings["MongoDbName"];

            InitialiseMappings();
        }

        private void InitialiseMappings()
        {
            // Get all the mapping profiles from the current assembly.
            var types = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t =>
                    t.BaseType == typeof(Profile));

            // Add all the mapping 
            // profiles to Automapper.
            Mapper.Initialize(cfg =>
            {
                foreach (var type in types)
                    cfg.AddProfile(type);
            });
        }

        public void SaveEntities(RugbyEntitiesResponse entitiesResponse)
        {
            // Map the provider data to a type mongo understands.
            var mongoEntities = 
                Mapper.Map<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyEntities.RugbyEntities, MongoRugbyEntities>(
                    entitiesResponse.Entities);

            mongoEntities.RequestTime = entitiesResponse.RequestTime;
            mongoEntities.ResponseTime = entitiesResponse.ResponseTime;

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
            var collection = db.GetCollection<MongoRugbyEntities>("entities");
            collection.InsertOneAsync(mongoEntities);
        }

        public void Save(RugbyFixturesResponse fixturesResponse)
        {
            // Map the provider data to a type mongo understands.
            var mongoFixtures = 
                Mapper.Map<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyFixtures.RugbyFixtures, MongoRugbyFixtures>(
                    fixturesResponse.Fixtures);

            mongoFixtures.RequestTime = fixturesResponse.RequestTime;
            mongoFixtures.ResponseTime = fixturesResponse.ResponseTime;

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
                //_logger.Error("Cannot ping MongoDB. " + e.StackTrace);
                //return;
            }

            // Add to the collection.
            var collection = db.GetCollection<MongoRugbyFixtures>("fixtures");
            collection.InsertOneAsync(mongoFixtures);
        }

        public void Save(RugbyFlatLogsResponse logsResponse)
        {
            // Map the provider data to a type mongo understands.
            var mongoLogs =
                Mapper.Map<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyFlatLogs.RugbyFlatLogs, MongoRugbyFlatLogs>(
                    logsResponse.RugbyFlatLogs);

            mongoLogs.RequestTime = logsResponse.RequestTime;
            mongoLogs.ResponseTime = logsResponse.ResponseTime;

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
                //_logger.Error("Cannot ping MongoDB. " + e.StackTrace);
                //return;
            }

            // Add to the collection.
            var collection = db.GetCollection<MongoRugbyFlatLogs>("logs");
            collection.InsertOneAsync(mongoLogs);
        }

        public void Save(RugbyGroupedLogsResponse logsResponse)
        {
            // Map the provider data to a type mongo understands.
            var mongoLogsGrouped =
                Mapper.Map<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyGroupedLogs.RugbyGroupedLogs, MongoRugbyGroupedLogs>(
                    logsResponse.RugbyGroupedLogs);

            mongoLogsGrouped.RequestTime = logsResponse.RequestTime;
            mongoLogsGrouped.ResponseTime = logsResponse.ResponseTime;

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
                //_logger.Error("Cannot ping MongoDB. " + e.StackTrace);
                //return;
            }

            // Add to the collection.
            var collection = db.GetCollection<MongoRugbyGroupedLogs>("logs");
            collection.InsertOneAsync(mongoLogsGrouped);
        }

        public void Save(RugbyMatchStatsResponse matchStatsResponse)
        {
            // Map the provider data to a type mongo understands.
            var matchStats =
                Mapper.Map<RugbyMatchStats, MongoRugbyMatchStats>(
                    matchStatsResponse.RugbyMatchStats);

            matchStats.RequestTime = matchStatsResponse.RequestTime;
            matchStats.ResponseTime = matchStatsResponse.ResponseTime;

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
                //_logger.Error("Cannot ping MongoDB. " + e.StackTrace);
                //return;
            }

            // Add to the collection.
            var collection = db.GetCollection<MongoRugbyMatchStats>("matchStats");
            collection.InsertOneAsync(matchStats);
        }

        public void Save(RugbyEventsFlowResponse eventsFlowResponse)
        {
            // Map the provider data to a type mongo understands.
            var eventsFlow =
                Mapper.Map<RugbyEventsFlow, MongoRugbyEventsFlow>(
                    eventsFlowResponse.RugbyEventsFlow);

            eventsFlow.RequestTime = eventsFlowResponse.RequestTime;
            eventsFlow.ResponseTime = eventsFlowResponse.ResponseTime;

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
                //_logger.Error("Cannot ping MongoDB. " + e.StackTrace);
                //return;
            }

            // Add to the collection.
            var collection = db.GetCollection<MongoRugbyEventsFlow>("eventsFlow");
            collection.InsertOneAsync(eventsFlow);
        }
    }
}