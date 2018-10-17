using SuperSportDataEngine.Common.Helpers;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Mappers
{
    using AutoMapper;
    using SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Rugby;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
    using SuperSportDataEngine.ApplicationLogic.Entities.Legacy;

    // This class is used by Reflection.
    public class LegacyMatchDetailsMapperProfile : Profile
    {
        public LegacyMatchDetailsMapperProfile()
        {
            CreateMap<RugbyMatchDetailsEntity, RugbyMatchDetails>()

                // Team A Details
                .ForMember(dest => dest.teamAStats, exp => exp.MapFrom(src => src.TeamAMatchStatistics))

                .ForMember(dest => dest.MatchStatisticsTeamA, exp => exp.MapFrom(src => src.TeamAMatchStatistics))

                .ForMember(dest => dest.TeamAName, exp => exp.MapFrom(
                    src => GetTeamAName(src)))

                .ForMember(dest => dest.IsPlaceholderTeamA, exp => exp.MapFrom(
                    src => GetTeamAName(src) == "TBC"))

                .ForMember(dest => dest.TeamAShortName, src => src.UseValue(LegacyFeedConstants.EmptyTeamName))

                .ForMember(dest => dest.TeamAScore, exp => exp.MapFrom(src => src.RugbyFixture.TeamAScore))

                .ForMember(dest => dest.TeamAId, exp => exp.MapFrom(src => src.RugbyFixture.TeamA.LegacyTeamId))

                .ForMember(dest => dest.TeamATeamsheet, exp => exp.MapFrom(src => src.TeamALineup))

                .ForMember(dest => dest.TeamAScorers, exp => exp.MapFrom(src => src.TeamAScorers))

                .ForMember(dest => dest.TeamASubstitutes, src => src.UseValue(LegacyFeedConstants.EmptyTeamSubstitutes))

                .ForMember(dest => dest.TeamACards, src => src.UseValue(LegacyFeedConstants.EmptyTeamCardsList))

                // Team B Details
                .ForMember(dest => dest.teamBStats, exp => exp.MapFrom(src => src.TeamBMatchStatistics))

                .ForMember(dest => dest.MatchStatisticsTeamB, exp => exp.MapFrom(src => src.TeamBMatchStatistics))

                .ForMember(dest => dest.TeamBName, exp => exp.MapFrom(
                    src => GetTeamBName(src)))

                .ForMember(dest => dest.IsPlaceholderTeamB, exp => exp.MapFrom(
                    src => GetTeamBName(src) == "TBC"))

                 .ForMember(dest => dest.TeamBShortName, src => src.UseValue(LegacyFeedConstants.EmptyTeamName))

                .ForMember(dest => dest.TeamBScore, exp => exp.MapFrom(src => src.RugbyFixture.TeamBScore))

                .ForMember(dest => dest.TeamBId, exp => exp.MapFrom(src => src.RugbyFixture.TeamB.LegacyTeamId))

                .ForMember(dest => dest.TeamBTeamsheet, exp => exp.MapFrom(src => src.TeamBLineup))

                .ForMember(dest => dest.TeamBScorers, exp => exp.MapFrom(src => src.TeamBScorers))

                .ForMember(dest => dest.TeamBSubstitutes, src => src.UseValue(LegacyFeedConstants.EmptyTeamSubstitutes))

                .ForMember(dest => dest.TeamBCards, src => src.UseValue(LegacyFeedConstants.EmptyTeamCardsList))

                // Fixture Specific Details
                .ForMember(dest => dest.Teamsheet, exp => exp.MapFrom(src => src.TeamsLineups))

                .ForMember(dest => dest.Events, exp => exp.MapFrom(src => src.MatchEvents))

                .ForMember(dest => dest.KickoffDateTime, exp => exp.MapFrom(
                    src => src.RugbyFixture.StartDateTime.UtcDateTime.FromUtcToSastDateTime().ToString("s")))

                .ForMember(dest => dest.KickoffDateTimeString, exp => exp.MapFrom(
                    src => src.RugbyFixture.StartDateTime.UtcDateTime.FromUtcToSastDateTime().ToString("s")))

                .ForMember(dest => dest.LeagueId, exp => exp.MapFrom(src => src.RugbyFixture.RugbyTournament.LegacyTournamentId))

                .ForMember(dest => dest.LeagueName, exp => exp.MapFrom(
                    src => src.RugbyFixture.RugbyTournament.NameCmsOverride ?? src.RugbyFixture.RugbyTournament.Name))

                .ForMember(dest => dest.LeagueUrlName, exp => exp.MapFrom(src => src.RugbyFixture.RugbyTournament.Slug))

                .ForMember(dest => dest.Location, expression => expression.MapFrom(
                    src => src.RugbyFixture.RugbyVenue == null ?
                        "TBC" : (src.RugbyFixture.RugbyVenue.NameCmsOverride ?? src.RugbyFixture.RugbyVenue.Name)))

                .ForMember(dest => dest.MatchCompleted, exp => exp.MapFrom(
                    src => src.RugbyFixture.RugbyFixtureStatus == RugbyFixtureStatus.Result))

                .ForMember(dest => dest.Status, exp => exp.MapFrom(
                    src => LegacyFeedConstants.GetFixtureStatusDescription(src.RugbyFixture.RugbyFixtureStatus)))

                .ForMember(dest => dest.StatusId, exp => exp.MapFrom(
                    src => LegacyFeedConstants.GetFixtureStatusId(src.RugbyFixture.RugbyFixtureStatus)))

                .ForMember(dest => dest.MatchID, exp => exp.MapFrom(src => src.RugbyFixture.LegacyFixtureId))

                .ForMember(dest => dest.MatchTime, exp => exp.MapFrom(
                    src => (src.RugbyFixture.StartDateTime.UtcDateTime.FromUtcToSastDateTime().AddSeconds(
                        src.RugbyFixture.GameTimeInSeconds)).ToString("s")))

                .ForMember(dest => dest.Officials, src => src.UseValue(LegacyFeedConstants.EmptyMatchOfficialsList))

                .ForMember(dest => dest.isScoredLive, exp => exp.MapFrom(src => src.RugbyFixture.IsLiveScored))

                .ForMember(dest => dest.Videos, src => src.UseValue(LegacyFeedConstants.EmptyVideosList))

                .ForMember(dest => dest.LiveVideos, src => src.UseValue(LegacyFeedConstants.EmptyLiveVideosList))

                .ForMember(dest => dest.Attendance, src => src.UseValue(LegacyFeedConstants.DefaultAttendanceValue))

                .ForMember(dest => dest.Preview, src => src.UseValue(string.Empty))

                .ForAllOtherMembers(dest => dest.Ignore());
        }

        private static string GetTeamBName(RugbyMatchDetailsEntity src)
        {
            return src.RugbyFixture.TeamB.NameCmsOverride ?? src.RugbyFixture.TeamB.Name;
        }

        private static string GetTeamAName(RugbyMatchDetailsEntity src)
        {
            return src.RugbyFixture.TeamA.NameCmsOverride ?? src.RugbyFixture.TeamA.Name;
        }
    }
}
