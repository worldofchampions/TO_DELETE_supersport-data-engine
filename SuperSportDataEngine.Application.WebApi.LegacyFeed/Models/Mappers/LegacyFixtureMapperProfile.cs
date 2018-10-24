namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Mappers
{
    using AutoMapper;
    using SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Shared;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;

    // This class is used by Reflection.
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
                    src => src.RugbyVenue == null ? "TBC" : (src.RugbyVenue.NameCmsOverride ?? src.RugbyVenue.Name)))

                .ForMember(dest => dest.ShowFixtureMatchNumber, exp => exp.MapFrom(src => src.RugbyTournament.HasFixtureMatchNumber))

                .ForMember(dest => dest.MatchNumber, exp => exp.MapFrom(
                    src => src.RugbyTournament.HasFixtureMatchNumber ? (src.MatchNumberCmsOverride ?? src.MatchNumber) : null))

                .ForMember(dest => dest.ShowFixtureRoundName, exp => exp.MapFrom(src => src.RugbyTournament.HasFixtureRoundName))

                .ForMember(dest => dest.RoundName, exp => exp.MapFrom(
                    src => src.RugbyTournament.HasFixtureRoundName ? (src.RoundNameCmsOverride ?? src.RoundName) : null))

                .ForMember(dest => dest.Preview, src => src.Ignore())

                .ForMember(dest => dest.Sorting, expression => expression.UseValue(LegacyFeedConstants.DefaultSortingValue))

                .ForMember(dest => dest.Channels, expression => expression.UseValue(LegacyFeedConstants.EmptyChannelsList))

                .ForMember(dest => dest.Status, exp => exp.MapFrom(src => LegacyFeedConstants.GetFixtureStatusDescription(src.RugbyFixtureStatus)))

                .ForMember(dest => dest.video, src => src.Ignore());
        }
    }
}
