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

namespace SuperSportDataEngine.Repository.MongoDb.PayloadData.Repositories
{

    public class MongoDbRugbyRepository : IMongoDbRugbyRepository
    {
        private IMongoClient _mongoClient;
        private string _mongoDatabaseName;

        public MongoDbRugbyRepository(IMongoClient mongoClient)
        {
            _mongoClient = mongoClient;
            _mongoDatabaseName = ConfigurationManager.AppSettings["MongoDbName"];
        }

        public void Save(RugbyEntitiesResponse entitiesResponse)
        {
            Mapper.Initialize(c => c.AddProfile<RugbyEntitiesMappingProfile>());

            // Map the provider data to a type mongo understands.
            var mongoEntities = 
                Mapper.Map<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyEntities.RugbyEntities, MongoRugbyEntities>(
                    entitiesResponse.Entities);

            mongoEntities.RequestTime = entitiesResponse.RequestTime;
            mongoEntities.ResponseTime = entitiesResponse.ResponseTime;

            // Get the Mongo DB.
            var db = _mongoClient.GetDatabase(_mongoDatabaseName);

            // Add to the collection.
            var collection = db.GetCollection<MongoRugbyEntities>("entities");
            collection.InsertOneAsync(mongoEntities);
        }

        public void Save(RugbyFixturesResponse fixturesResponse)
        {
            Mapper.Initialize(c => c.AddProfile<RugbyFixturesMappingProfile>());

            // Map the provider data to a type mongo understands.
            var mongoFixtures = 
                Mapper.Map<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyFixtures.RugbyFixtures, MongoRugbyFixtures>(
                    fixturesResponse.Fixtures);

            mongoFixtures.RequestTime = fixturesResponse.RequestTime;
            mongoFixtures.ResponseTime = fixturesResponse.ResponseTime;

            // Get the Mongo DB.
            var db = _mongoClient.GetDatabase(_mongoDatabaseName);

            // Add to the collection.
            var collection = db.GetCollection<MongoRugbyFixtures>("fixtures");
            collection.InsertOneAsync(mongoFixtures);
        }

        public void Save(RugbyFlatLogsResponse logsResponse)
        {
            Mapper.Initialize(c => c.AddProfile<RugbyFlatLogsMappingProfile>());

            // Map the provider data to a type mongo understands.
            var mongoLogs =
                Mapper.Map<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyFlatLogs.RugbyFlatLogs, MongoRugbyFlatLogs>(
                    logsResponse.RugbyFlatLogs);

            mongoLogs.RequestTime = logsResponse.RequestTime;
            mongoLogs.ResponseTime = logsResponse.ResponseTime;

            // Get the Mongo DB.
            var db = _mongoClient.GetDatabase(_mongoDatabaseName);

            // Add to the collection.
            var collection = db.GetCollection<MongoRugbyFlatLogs>("logs");
            collection.InsertOneAsync(mongoLogs);
        }

        public void Save(RugbyGroupedLogsResponse logsResponse)
        {
            Mapper.Initialize(c => c.AddProfile<RugbyGroupedLogsMappingProfile>());

            // Map the provider data to a type mongo understands.
            var mongoLogsGrouped =
                Mapper.Map<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyGroupedLogs.RugbyGroupedLogs, MongoRugbyGroupedLogs>(
                    logsResponse.RugbyGroupedLogs);

            mongoLogsGrouped.RequestTime = logsResponse.RequestTime;
            mongoLogsGrouped.ResponseTime = logsResponse.ResponseTime;

            // Get the Mongo DB.
            var db = _mongoClient.GetDatabase(_mongoDatabaseName);

            // Add to the collection.
            var collection = db.GetCollection<MongoRugbyGroupedLogs>("logs");
            collection.InsertOneAsync(mongoLogsGrouped);
        }

        public void Save(RugbyMatchStatsResponse matchStatsResponse)
        {
            Mapper.Initialize(c => c.AddProfile<RugbyMatchStatsMappingProfile>());

            // Map the provider data to a type mongo understands.
            var matchStats =
                Mapper.Map<RugbyMatchStats, MongoRugbyMatchStats>(
                    matchStatsResponse.RugbyMatchStats);

            matchStats.RequestTime = matchStatsResponse.RequestTime;
            matchStats.ResponseTime = matchStatsResponse.ResponseTime;

            // Get the Mongo DB.
            var db = _mongoClient.GetDatabase(_mongoDatabaseName);

            // Add to the collection.
            var collection = db.GetCollection<MongoRugbyMatchStats>("matchStats");
            collection.InsertOneAsync(matchStats);
        }
    }
}