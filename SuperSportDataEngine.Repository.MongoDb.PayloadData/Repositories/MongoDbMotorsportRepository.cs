using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Motorsport;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.MongoDb.PayloadData.Interfaces;
using SuperSportDataEngine.Common.Logging;

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
        }

        public async Task Save(MotorsportEntitiesResponse leagues)
        {
            if (leagues == null)
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
            }
            catch (System.Exception)
            {
            }

            // Add to the collection.
            var collection = db.GetCollection<ApiResult>("motorsport_entities");
            await collection.InsertOneAsync(leagues.apiResults[0]);
        }
    }
}
