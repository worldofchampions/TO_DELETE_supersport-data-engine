namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Mappers
{
    using AutoMapper;
    using SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Shared;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;

    public class LegacyFixtureMapperProfile : Profile
    {
        public LegacyFixtureMapperProfile()
        {
            CreateMap<RugbyFixture, Fixture>()

                .IncludeBase<RugbyFixture, Match>()

                .ForMember(dest => dest.Sport, expression => expression.UseValue(SportType.Rugby))

                .ForMember(dest => dest.AwayTeamShortName, expression => expression.MapFrom(
                    src => src.TeamAIsHomeTeam ?
                    (src.TeamB.NameCmsOverride ?? src.TeamB.Name) : (src.TeamA.NameCmsOverride ?? src.TeamA.Name)))

                .ForMember(dest => dest.HomeTeamShortName, expression => expression.MapFrom(
                   src => src.TeamAIsHomeTeam ?
                   (src.TeamA.NameCmsOverride ?? src.TeamA.Name) : (src.TeamB.NameCmsOverride ?? src.TeamB.Name)))

                .ForMember(dest => dest.HomeTeamScore, expression => expression.MapFrom(
                    src => (src.TeamAIsHomeTeam ? src.TeamAScore : src.TeamBScore)))

                .ForMember(dest => dest.AwayTeamScore, expression => expression.MapFrom(
                    src => (src.TeamAIsHomeTeam ? src.TeamBScore : src.TeamAScore)))

                .ForMember(dest => dest.Location, expression => expression.MapFrom(
                    src => src.RugbyVenue != null ? src.RugbyVenue.Name : "TBC"))

                .ForMember(dest => dest.Preview, src => src.Ignore())

                .ForMember(dest => dest.Sorting, expression => expression.UseValue(LegacyFeedConstants.DefaultSortingValue))

                .ForMember(dest => dest.Channels, expression => expression.UseValue(LegacyFeedConstants.EmptyChannelsList))

                .ForMember(dest => dest.Status, exp => exp.MapFrom(
                        src => LegacyFeedConstants.GetFixtureStatusDescription(src.RugbyFixtureStatus)))

                .ForMember(dest => dest.video, src => src.Ignore());
        }
    }
}