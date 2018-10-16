using SuperSportDataEngine.Common.Helpers;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Mappers
{
    using AutoMapper;
    using SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Shared;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
    using System;

    // This class is used by Reflection.
    public class LegacyMatchModelMapperProfile : Profile
    {
        public LegacyMatchModelMapperProfile()
        {
            CreateMap<RugbyFixture, Match>()

                // For Away team
                .ForMember(dest => dest.AwayTeam, expression => expression.MapFrom(
                    src => GetAwayTeam(src)))

                .ForMember(dest => dest.IsPlaceholderAwayTeam, expression => expression.MapFrom(
                    src => GetAwayTeam(src) == "TBC"))

                .ForMember(dest => dest.AwayTeamId, expression => expression.MapFrom(
                    src => src.TeamAIsHomeTeam ? src.TeamB.LegacyTeamId : src.TeamA.LegacyTeamId))

                .ForMember(dest => dest.AwayTeamScore, expression => expression.MapFrom(
                   src => (src.TeamAIsHomeTeam ? src.TeamBScore : src.TeamAScore) ?? LegacyFeedConstants.DefaultScoreForStartedGame))

                // For Home team
                .ForMember(dest => dest.HomeTeam, expression => expression.MapFrom(
                    src => GeHomeTeam(src)))

                .ForMember(dest => dest.HomeTeamId, expression => expression.MapFrom(
                    src => src.TeamAIsHomeTeam ? src.TeamA.LegacyTeamId : src.TeamB.LegacyTeamId))

                .ForMember(dest => dest.IsPlaceholderHomeTeam, expression => expression.MapFrom(
                    src => GeHomeTeam(src) == "TBC"))

                .ForMember(dest => dest.HomeTeamScore, expression => expression.MapFrom(
                    src => (src.TeamAIsHomeTeam ? src.TeamAScore : src.TeamBScore) ?? LegacyFeedConstants.DefaultScoreForStartedGame))

                // For Fixture specific
                .ForMember(dest => dest.LeagueName, expression => expression.MapFrom(
                    src => src.RugbyTournament.NameCmsOverride ?? src.RugbyTournament.Name))

                .ForMember(dest => dest.LeagueUrlName, expression => expression.MapFrom(
                    src => src.RugbyTournament.Slug))

                .ForMember(dest => dest.LeagueId, expression => expression.MapFrom(
                    src => src.RugbyTournament.LegacyTournamentId))

                .ForMember(dest => dest.MatchID, expression => expression.MapFrom(
                    src => src.LegacyFixtureId))

                // Use sortable datetime format for legacy feed
                .ForMember(dest => dest.MatchDateTime, expression => expression.MapFrom(
                    src => Convert.ToDateTime(src.StartDateTime.UtcDateTime.ConvertToLocalSAST().ToString("s"))))

                // Use sortable datetime format for legacy feed
                .ForMember(dest => dest.MatchDateTimeString, expression => expression.MapFrom(
                    src => src.StartDateTime.UtcDateTime.ConvertToLocalSAST().ToString("s")))

                // Use sortable datetime format for legacy feed
                .ForMember(dest => dest.MatchEndDateTimeString, expression => expression.UseValue(
                   DateTime.MinValue.ToString("s")))

                // TODO: Confirm where these come from?
                .ForMember(dest => dest.International, expression => expression.UseValue(false))

                .ForMember(dest => dest.IsFeatured, expression => expression.UseValue(false))

                .ForMember(dest => dest.WalkOver, expression => expression.UseValue(false))

                .ForMember(dest => dest.Status, expression => expression.UseValue(string.Empty))

                // Ignore elements not used even by legacy feed
                .ForAllOtherMembers(dest => dest.Ignore());
        }

        private static string GeHomeTeam(RugbyFixture fixture)
        {
            return fixture.TeamAIsHomeTeam
                ? (fixture.TeamA.NameCmsOverride ?? fixture.TeamA.Name)
                : (fixture.TeamB.NameCmsOverride ?? fixture.TeamB.Name);
        }

        private static string GetAwayTeam(RugbyFixture fixture)
        {
            return fixture.TeamAIsHomeTeam
                ? (fixture.TeamB.NameCmsOverride ?? fixture.TeamB.Name)
                : (fixture.TeamA.NameCmsOverride ?? fixture.TeamA.Name);
        }
    }
}
