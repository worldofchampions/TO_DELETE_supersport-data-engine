using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MongoDB.Bson;
using MongoDB.Driver;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Motorsport;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.MongoDb.PayloadData.Interfaces;
using SuperSportDataEngine.Common.Logging;
using SuperSportDataEngine.Repository.MongoDb.PayloadData.Models;
using SuperSportDataEngine.Repository.MongoDb.PayloadData.Models.RugbyEntities;

namespace SuperSportDataEngine.Repository.MongoDb.PayloadData.Repositories
{
    public class MongoDbMotorsportRepository : IMongoDbMotorsportRepository
    {
        private IMongoClient _mongoClient;
        private ILoggingService _logger;

        private string _mongoDatabaseName;

        public MongoDbMotorsportRepository(
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

        public async Task Save(MotorsportEntitiesResponse leagues)
        {
            if (leagues == null)
                return;

            // Map the provider data to a type mongo understands.
            var mongoEntities =
                Mapper.Map<ApiResult, MongoMotorsportApiResult>(leagues.apiResults[0]);

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
            var collection = db.GetCollection<MongoMotorsportApiResult>("motorsport_entities");
            await collection.InsertOneAsync(mongoEntities);
        }
    }
}
