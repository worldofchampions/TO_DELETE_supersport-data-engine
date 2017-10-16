namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Mappers
{
    using AutoMapper;
    using SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Rugby;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
    using SuperSportDataEngine.ApplicationLogic.Entities.Legacy;

    public class LegacyMatchDetailsMapperProfile : Profile
    {
        public LegacyMatchDetailsMapperProfile()
        {
            CreateMap<RugbyMatchDetailsEntity, RugbyMatchDetails>()

                // Team A Details
                .ForMember(dest => dest.TeamAStats, exp => exp.MapFrom(src => src.TeamAMatchStatistics))

                .ForMember(dest => dest.MatchStatisticsTeamA, exp => exp.MapFrom(src => src.TeamAMatchStatistics))

                .ForMember(dest => dest.TeamAName, exp => exp.MapFrom(src => src.RugbyFixture.TeamA.Name))

                .ForMember(dest => dest.TeamAShortName, src =>src.UseValue(string.Empty))

                .ForMember(dest => dest.TeamAScore, exp => exp.MapFrom(src => src.RugbyFixture.TeamAScore))
                
                .ForMember(dest => dest.TeamAId, exp => exp.MapFrom(src => src.RugbyFixture.TeamA.LegacyTeamId))

                // Team B Details
                .ForMember(dest => dest.TeamBStats, exp => exp.MapFrom(src => src.TeamBMatchStatistics))

                .ForMember(dest => dest.MatchStatisticsTeamB, exp => exp.MapFrom(src => src.TeamBMatchStatistics))

                .ForMember(dest => dest.TeamBName, exp => exp.MapFrom(src => src.RugbyFixture.TeamB.Name))

                 .ForMember(dest => dest.TeamBShortName, src => src.UseValue(string.Empty))

                .ForMember(dest => dest.TeamBScore, exp => exp.MapFrom(src => src.RugbyFixture.TeamBScore))

                .ForMember(dest => dest.TeamBId, exp => exp.MapFrom(src => src.RugbyFixture.TeamB.LegacyTeamId))

                // Fixture Specific Details
                .ForMember(dest => dest.Events, exp => exp.MapFrom(src => src.Commentary))

                .ForMember(dest => dest.KickoffDateTime, exp => exp.MapFrom(
                    src => src.RugbyFixture.StartDateTime.UtcDateTime.ToLocalTime().ToString("s")))

                .ForMember(dest => dest.KickoffDateTimeString, exp => exp.MapFrom(
                    src => src.RugbyFixture.StartDateTime.UtcDateTime.ToLocalTime().ToString("s")))

                .ForMember(dest => dest.LeagueId, exp => exp.MapFrom(src => src.RugbyFixture.RugbyTournament.LegacyTournamentId))

                .ForMember(dest => dest.LeagueName, exp => exp.MapFrom(src => src.RugbyFixture.RugbyTournament.Name))

                .ForMember(dest => dest.LeagueUrlName, exp => exp.MapFrom(src => src.RugbyFixture.RugbyTournament.Slug))

                .ForMember(dest => dest.Location, exp => exp.MapFrom(src => src.RugbyFixture.RugbyVenue.Name))

                .ForMember(dest => dest.MatchCompleted, exp => exp.MapFrom(
                    src => src.RugbyFixture.RugbyFixtureStatus == RugbyFixtureStatus.Result ||
                           src.RugbyFixture.RugbyFixtureStatus == RugbyFixtureStatus.PostMatch ? true : false))

                .ForMember(dest => dest.MatchID, exp => exp.MapFrom(src => src.RugbyFixture.LegacyFixtureId))

                .ForMember(dest => dest.MatchTime, exp => exp.MapFrom(
                    src => src.RugbyFixture.StartDateTime.UtcDateTime.ToLocalTime().ToString("s")))
               
                .ForAllOtherMembers(dest => dest.Ignore());
        }
    }
}