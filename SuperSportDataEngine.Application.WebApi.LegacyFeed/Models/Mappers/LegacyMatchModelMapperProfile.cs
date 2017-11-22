namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Mappers
{
    using System;
    using AutoMapper;
    using SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Shared;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;

    public class LegacyMatchModelMapperProfile : Profile
    {
        public LegacyMatchModelMapperProfile()
        {
            CreateMap<RugbyFixture, Match>()

                // For Away team
                .ForMember(dest => dest.AwayTeam, expression => expression.MapFrom(
                    src => src.TeamAIsHomeTeam ?
                    (src.TeamB.NameCmsOverride ?? src.TeamB.Name) : (src.TeamA.NameCmsOverride ?? src.TeamA.Name)))

                .ForMember(dest => dest.AwayTeamId, expression => expression.MapFrom(
                    src => src.TeamAIsHomeTeam ? src.TeamB.LegacyTeamId : src.TeamA.LegacyTeamId))

                .ForMember(dest => dest.AwayTeamScore, expression => expression.MapFrom(
                   src => (src.TeamAIsHomeTeam ? src.TeamBScore : src.TeamAScore) ?? LegacyFeedConstants.DefaultScoreForStartedGame))

                // For Home team
                .ForMember(dest => dest.HomeTeam, expression => expression.MapFrom(
                    src => src.TeamAIsHomeTeam ?
                    (src.TeamA.NameCmsOverride ?? src.TeamA.Name) : (src.TeamB.NameCmsOverride ?? src.TeamB.Name)))

                .ForMember(dest => dest.HomeTeamId, expression => expression.MapFrom(
                    src => src.TeamAIsHomeTeam ? src.TeamA.LegacyTeamId : src.TeamB.LegacyTeamId))

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
                    src => Convert.ToDateTime(src.StartDateTime.UtcDateTime.ToLocalTime().ToString("s"))))

                // Use sortable datetime format for legacy feed
                .ForMember(dest => dest.MatchDateTimeString, expression => expression.MapFrom(
                    src => src.StartDateTime.UtcDateTime.ToLocalTime().ToString("s")))

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
    }
}