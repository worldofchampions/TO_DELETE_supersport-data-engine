using AutoMapper;
using MongoDB.Driver;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.ResponseModels;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.MongoDb.PayloadData.Interfaces;
using SuperSportDataEngine.Repository.MongoDb.PayloadData.Models.RugbyEntities;
using SuperSportDataEngine.Repository.MongoDb.PayloadData.MappingProfiles;
using SuperSportDataEngine.Repository.MongoDb.PayloadData.Models.RugbyFixtures;
using SuperSportDataEngine.Repository.MongoDb.PayloadData.Models.RugbyLogs;

namespace SuperSportDataEngine.Repository.MongoDb.PayloadData.Repositories
{

    public class MongoDbRugbyRepository : IMongoDbRugbyRepository
    {
        private IMongoClient _mongoClient;

        public MongoDbRugbyRepository(IMongoClient mongoClient)
        {
            _mongoClient = mongoClient;
        }

        public void Save(RugbyEntitiesResponse entitiesResponse)
        {
            Mapper.Initialize(c => c.AddProfile<RugbyFixturesMappingProfile>());

            // Map the provider data to a type mongo understands.
            var mongoEntities = 
                Mapper.Map<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyEntities.RugbyEntities, MongoRugbyEntities>(
                    entitiesResponse.Entities);

            mongoEntities.RequestTime = entitiesResponse.RequestTime;
            mongoEntities.ResponseTime = entitiesResponse.ResponseTime;

            // Get the Mongo DB.
            var db = _mongoClient.GetDatabase("supersport-dataengine");

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
            var db = _mongoClient.GetDatabase("supersport-dataengine");

            // Add to the collection.
            var collection = db.GetCollection<MongoRugbyFixtures>("fixtures");
            collection.InsertOneAsync(mongoFixtures);
        }

        public void Save(RugbyLogsResponse logsResponse)
        {
            Mapper.Initialize(c => c.AddProfile<RugbyLogsMappingProfile>());

            // Map the provider data to a type mongo understands.
            var mongoLogs =
                Mapper.Map<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyLogs.RugbyLogs, MongoRugbyLogs>(
                    logsResponse.RugbyLogs);

            mongoLogs.RequestTime = logsResponse.RequestTime;
            mongoLogs.ResponseTime = logsResponse.ResponseTime;

            // Get the Mongo DB.
            var db = _mongoClient.GetDatabase("supersport-dataengine");

            // Add to the collection.
            var collection = db.GetCollection<MongoRugbyLogs>("logs");
            collection.InsertOneAsync(mongoLogs);
        }
    }
}