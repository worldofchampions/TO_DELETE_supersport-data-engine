using AutoMapper;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Mappers
{
    public class LegacyFixtureMapperProfile : Profile
    {
        public LegacyFixtureMapperProfile()
        {
            CreateMap<RugbyFixture, Fixture>()

                .IncludeBase<RugbyFixture, MatchModel>()

                .ForMember(dest => dest.Sport, expression => expression.UseValue(SportType.Rugby))

                .ForMember(dest => dest.Preview, src => src.Ignore())

                .ForMember(dest => dest.Sorting, expression => expression.UseValue(0))

                .ForMember(dest => dest.video, src => src.Ignore());
        }
    }
}