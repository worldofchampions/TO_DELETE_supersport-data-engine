using AutoMapper;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
using System.Collections.Generic;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Mappers
{
    public class LegacyFixtureMapperProfile : Profile
    {
        public LegacyFixtureMapperProfile()
        {
            CreateMap<RugbyFixture, Fixture>()

                .IncludeBase<RugbyFixture, Match>()

                .ForMember(dest => dest.Sport, expression => expression.UseValue(SportType.Rugby))

                .ForMember(dest => dest.AwayTeamShortName, expression => expression.MapFrom(
                    src => src.TeamAIsHomeTeam ? src.TeamB.Name : src.TeamA.Name))
                
                .ForMember(dest => dest.HomeTeamShortName, expression => expression.MapFrom(
                   src => src.TeamAIsHomeTeam ? src.TeamA.Name : src.TeamB.Name))

                .ForMember(dest => dest.Location, expression => expression.MapFrom(
                    src => src.RugbyVenue.Name))

                .ForMember(dest => dest.Preview, src => src.Ignore())

                .ForMember(dest => dest.Sorting, expression => expression.UseValue(LegacyFeedConstants.DefaultSortingValue))

                .ForMember(dest => dest.Channels, expression => expression.UseValue(LegacyFeedConstants.EmptyChannelsList))

                .ForMember(dest => dest.Status, src => src.Ignore())

                .ForMember(dest => dest.video, src => src.Ignore());
        }
    }
}