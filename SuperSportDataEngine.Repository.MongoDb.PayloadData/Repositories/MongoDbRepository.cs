using AutoMapper;
using MongoDB.Driver;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.ResponseModels;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.MongoDb.PayloadData.Interfaces;
using SuperSportDataEngine.Gateway.Http.StatsProzone.Models;
using SuperSportDataEngine.Repository.MongoDb.PayloadData.Models;

namespace SuperSportDataEngine.Repository.MongoDb.PayloadData.Repositories
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            Mapper.CreateMap<Entities, MongoEntities>().ReverseMap();
            Mapper.CreateMap<Gateway.Http.StatsProzone.Models.GroundCondition, Models.GroundCondition>().ReverseMap();
            Mapper.CreateMap<Gateway.Http.StatsProzone.Models.Competition, Models.Competition>().ReverseMap();
            Mapper.CreateMap<Gateway.Http.StatsProzone.Models.Player, Models.Player>().ReverseMap();
            Mapper.CreateMap<Gateway.Http.StatsProzone.Models.Position, Models.Position>().ReverseMap();
            Mapper.CreateMap<Gateway.Http.StatsProzone.Models.Round, Models.Round>().ReverseMap();
            Mapper.CreateMap<Gateway.Http.StatsProzone.Models.ScoringMethod, Models.ScoringMethod>().ReverseMap();
            Mapper.CreateMap<Gateway.Http.StatsProzone.Models.Season, Models.Season>().ReverseMap();
            Mapper.CreateMap<Gateway.Http.StatsProzone.Models.Statistic, Models.Statistic>().ReverseMap();
            Mapper.CreateMap<Gateway.Http.StatsProzone.Models.Team, Models.Team>().ReverseMap();
            Mapper.CreateMap<Gateway.Http.StatsProzone.Models.Venue, Models.Venue>().ReverseMap();
            Mapper.CreateMap<Gateway.Http.StatsProzone.Models.WeatherCondition, Models.WeatherCondition>().ReverseMap();
            Mapper.CreateMap<Gateway.Http.StatsProzone.Models.GameState, Models.GameState>().ReverseMap();
            Mapper.CreateMap<Gateway.Http.StatsProzone.Models.Official, Models.Official>().ReverseMap();
        }
    }

    public class MongoDbRepository : IMongoDbRepository
    {
        public void Save(EntitiesResponse entitiesResponse)
        {
            Mapper.Initialize(c => c.AddProfile<MappingProfile>());

            // Map the provider data to a type mongo understands.
            var mongoEntities = Mapper.Map<Entities, MongoEntities>(entitiesResponse.Entities);
            mongoEntities.RequestTime = entitiesResponse.RequestTime;

            // Get the Mongo DB.
            var client = new MongoClient("mongodb://RND-MDODS-QA.dstvo.local:27017");
            var db = client.GetDatabase("supersport-dataengine");

            // Add to the collection.
            var collection = db.GetCollection<MongoEntities>("entities");
            collection.InsertOneAsync(mongoEntities);
        }
    }
}
