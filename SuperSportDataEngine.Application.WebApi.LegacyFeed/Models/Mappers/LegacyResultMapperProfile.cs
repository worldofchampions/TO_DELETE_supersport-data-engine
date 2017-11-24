namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Mappers
{
    using AutoMapper;
    using SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Shared;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;

    public class LegacyResultMapperProfile : Profile
    {
        public LegacyResultMapperProfile()
        {
            CreateMap<RugbyFixture, Result>()

                .IncludeBase<RugbyFixture, Match>()

                .ForMember(dest => dest.Sport, expression => expression.UseValue(SportType.Rugby))

                .ForMember(dest => dest.LeagueUrlName, expression => expression.MapFrom(
                    src => src.RugbyTournament.Slug))

                .ForMember(dest => dest.HomeTeamScore, expression => expression.MapFrom(
                    src => (src.TeamAIsHomeTeam ? src.TeamAScore : src.TeamBScore) ?? LegacyFeedConstants.DefaultScoreForStartedGame))

                .ForMember(dest => dest.AwayTeamScore, expression => expression.MapFrom(
                   src => (src.TeamAIsHomeTeam ? src.TeamBScore : src.TeamAScore) ?? LegacyFeedConstants.DefaultScoreForStartedGame))

                .ForMember(dest => dest.AwayTeamShortName, expression => expression.MapFrom(
                    src => src.TeamAIsHomeTeam ? src.TeamB.Name : src.TeamA.Name))

                .ForMember(dest => dest.HomeTeamShortName, expression => expression.MapFrom(
                   src => src.TeamAIsHomeTeam ? src.TeamA.Name : src.TeamB.Name))

                .ForMember(dest => dest.Location, expression => expression.MapFrom(
                    src => src.RugbyVenue.Name))

                .ForMember(dest => dest.Result, expression => expression.UseValue(true))

                .ForMember(dest => dest.Sorting, expression => expression.UseValue(LegacyFeedConstants.DefaultSortingValue))

                .ForMember(dest => dest.Channels, src => src.Ignore())

                .ForMember(dest => dest.Status, src => src.Ignore())

                .ForMember(dest => dest.HomeTeamScorers, src => src.Ignore())

                .ForMember(dest => dest.AwayTeamScorers, src => src.Ignore());
        }
    }
}