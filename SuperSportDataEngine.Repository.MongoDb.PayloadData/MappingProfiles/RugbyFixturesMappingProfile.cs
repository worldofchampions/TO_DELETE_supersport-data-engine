using AutoMapper;
using SuperSportDataEngine.Repository.MongoDb.PayloadData.Models.RugbyFixtures;

namespace SuperSportDataEngine.Repository.MongoDb.PayloadData.MappingProfiles
{
    public class RugbyFixturesMappingProfile : Profile
    {
        public RugbyFixturesMappingProfile()
        {
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyFixtures.RugbyFixtures, MongoRugbyFixtures>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyFixtures.Team, Team>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyFixtures.GameFixture, GameFixture>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyFixtures.RoundFixture, RoundFixture>().ReverseMap();
        }
    }
}