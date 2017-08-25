using AutoMapper;
using MongoDB.Driver;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.ResponseModels;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.MongoDb.PayloadData.Interfaces;
using SuperSportDataEngine.Repository.MongoDb.PayloadData.Models;

namespace SuperSportDataEngine.Repository.MongoDb.PayloadData.Repositories
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RugbyEntities, MongoRugbyEntities>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.GroundCondition, Models.GroundCondition>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Competition, Models.Competition>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Player, Models.Player>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Position, Models.Position>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Round, Models.Round>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.ScoringMethod, Models.ScoringMethod>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Season, Models.Season>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Statistic, Models.Statistic>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Team, Models.Team>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Venue, Models.Venue>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.WeatherCondition, Models.WeatherCondition>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.GameState, Models.GameState>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Official, Models.Official>().ReverseMap();
        }
    }

    public class MongoDbRugbyRepository : IMongoDbRugbyRepository
    {
        private IMongoClient _mongoClient;

        public MongoDbRugbyRepository(IMongoClient mongoClient)
        {
            _mongoClient = mongoClient;
        }

        public void Save(RugbyEntitiesResponse entitiesResponse)
        {
            Mapper.Initialize(c => c.AddProfile<MappingProfile>());

            // Map the provider data to a type mongo understands.
            var mongoEntities = Mapper.Map<RugbyEntities, MongoRugbyEntities>(entitiesResponse.Entities);
            mongoEntities.RequestTime = entitiesResponse.RequestTime;
            mongoEntities.ResponseTime = entitiesResponse.ResponseTime;

            // Get the Mongo DB.
            var db = _mongoClient.GetDatabase("supersport-dataengine");

            // Add to the collection.
            var collection = db.GetCollection<MongoRugbyEntities>("entities");
            collection.InsertOneAsync(mongoEntities);
        }
    }
}