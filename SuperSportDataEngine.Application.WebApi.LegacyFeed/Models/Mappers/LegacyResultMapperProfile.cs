namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Mappers
{
    using AutoMapper;
    using SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Shared;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;

    // This class is used by Reflection.
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
                    src => src.TeamAIsHomeTeam ?
                        (src.TeamB.NameCmsOverride ?? src.TeamB.Name) : (src.TeamA.NameCmsOverride ?? src.TeamA.Name)))

                .ForMember(dest => dest.HomeTeamShortName, expression => expression.MapFrom(
                    src => src.TeamAIsHomeTeam ?
                        (src.TeamA.NameCmsOverride ?? src.TeamA.Name) : (src.TeamB.NameCmsOverride ?? src.TeamB.Name)))

                .ForMember(dest => dest.Location, expression => expression.MapFrom(
                    src => src.RugbyVenue == null ?
                        "TBC" : (src.RugbyVenue.NameCmsOverride ?? src.RugbyVenue.Name)))

                .ForMember(dest => dest.ShowFixtureMatchNumber, exp => exp.MapFrom(src => src.RugbyTournament.HasFixtureMatchNumber))

                .ForMember(dest => dest.MatchNumber, exp => exp.MapFrom(
                    src => src.RugbyTournament.HasFixtureMatchNumber ? (src.MatchNumberCmsOverride ?? src.MatchNumber) : null))

                .ForMember(dest => dest.ShowFixtureRoundName, exp => exp.MapFrom(src => src.RugbyTournament.HasFixtureRoundName))

                .ForMember(dest => dest.RoundName, exp => exp.MapFrom(
                    src => src.RugbyTournament.HasFixtureRoundName ? (src.RoundNameCmsOverride ?? src.RoundName) : null))

                .ForMember(dest => dest.Result, expression => expression.UseValue(true))

                .ForMember(dest => dest.Sorting, expression => expression.UseValue(LegacyFeedConstants.DefaultSortingValue))

                .ForMember(dest => dest.Channels, src => src.Ignore())

                .ForMember(dest => dest.Status, src => src.Ignore())

                .ForMember(dest => dest.HomeTeamScorers, src => src.Ignore())

                .ForMember(dest => dest.AwayTeamScorers, src => src.Ignore());
        }
    }
}
