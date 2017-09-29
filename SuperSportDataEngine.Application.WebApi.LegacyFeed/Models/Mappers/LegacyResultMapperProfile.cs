using AutoMapper;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Mappers
{
    public class LegacyResultMapperProfile : Profile
    {
        public LegacyResultMapperProfile()
        {
            CreateMap<RugbyFixture, Result>()

                .IncludeBase<RugbyFixture, MatchModel>()

                .ForMember(dest => dest.Sport, expression => expression.UseValue(SportType.Rugby))

                .ForMember(dest => dest.AwayTeamScore, expression => expression.MapFrom(
                    src => src.TeamAIsHomeTeam ? src.TeamBScore : src.TeamAScore))

                .ForMember(dest => dest.HomeTeamScore, expression => expression.MapFrom(
                    src => src.TeamAIsHomeTeam ? src.TeamAScore : src.TeamBScore))
                
                .ForMember(dest => dest.Result, expression => expression.UseValue(true))

                .ForMember(dest => dest.Sorting, expression => expression.UseValue(0))

                .ForMember(dest => dest.Channels, src => src.Ignore())

                .ForMember(dest => dest.HomeTeamScorers, src => src.Ignore())

                .ForMember(dest => dest.AwayTeamScorers, src => src.Ignore());
        }
    }
}