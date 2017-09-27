using AutoMapper;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
using System;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Mappers
{
    public class LegacyResultMapperProfile : Profile
    {
        public LegacyResultMapperProfile() : base()
        {
            CreateMap<RugbyFixture, Result>()
                // For Away team 
                .ForMember(dest => dest.AwayTeam, expression => expression.MapFrom(
                    src => src.TeamAIsHomeTeam ? src.TeamB.Name : src.TeamA.Name))

                .ForMember(dest => dest.AwayTeamId, expression => expression.MapFrom(
                    src => src.TeamAIsHomeTeam ? src.TeamB.LegacyTeamId : src.TeamA.LegacyTeamId))

                .ForMember(dest => dest.AwayTeamShortName, expression => expression.MapFrom(
                    src => src.TeamAIsHomeTeam ? src.TeamB.Name : src.TeamA.Name))

                .ForMember(dest => dest.AwayTeamScore, expression => expression.MapFrom(
                    src => src.TeamAIsHomeTeam ? src.TeamBScore : src.TeamAScore))

                // For Home team 
                .ForMember(dest => dest.HomeTeam, expression => expression.MapFrom(
                    src => src.TeamAIsHomeTeam ? src.TeamA.Name : src.TeamB.Name))

                .ForMember(dest => dest.HomeTeamId, expression => expression.MapFrom(
                    src => src.TeamAIsHomeTeam ? src.TeamA.LegacyTeamId : src.TeamB.LegacyTeamId))

                .ForMember(dest => dest.HomeTeamShortName, expression => expression.MapFrom(
                    src => src.TeamAIsHomeTeam ? src.TeamA.Name : src.TeamB.Name))

                .ForMember(dest => dest.HomeTeamScore, expression => expression.MapFrom(
                    src => src.TeamAIsHomeTeam ? src.TeamAScore : src.TeamBScore))


                // For Fixture specific
                .ForMember(dest => dest.LeagueId, expression => expression.MapFrom(
                    src => src.RugbyTournament.LegacyTournamentId))

                .ForMember(dest => dest.LeagueName, expression => expression.MapFrom(
                    src => src.RugbyTournament.Name))

                .ForMember(dest => dest.LeagueUrlName, expression => expression.MapFrom(
                    src => src.RugbyTournament.Slug))

                .ForMember(dest => dest.Location, expression => expression.MapFrom(
                    src => src.RugbyVenue.Name))

                // Use sortable datetime format for legacy feed
                .ForMember(dest => dest.MatchDateTime, expression => expression.MapFrom(
                    src => Convert.ToDateTime(src.StartDateTime.UtcDateTime.ToLocalTime().ToString("s"))))

                // Use sortable datetime format for legacy feed
                .ForMember(dest => dest.MatchDateTimeString, expression => expression.MapFrom(
                    src => src.StartDateTime.UtcDateTime.ToLocalTime().ToString("s")))

                // Use sortable datetime format for legacy feed
                .ForMember(dest => dest.MatchEndDateTimeString, expression => expression.UseValue(
                   DateTime.MinValue.ToString("s")))

                .ForMember(dest => dest.MatchID, expression => expression.MapFrom(
                    src => src.LegacyFixtureId))

                .ForMember(dest => dest.Result, expression => expression.UseValue(true))

                .ForMember(dest => dest.Sport, expression => expression.UseValue(SportType.Rugby))

                // TODO: Confirm where these come from?
                .ForMember(dest => dest.International, expression => expression.UseValue(false))

                .ForMember(dest => dest.IsFeatured, expression => expression.UseValue(false))

                .ForMember(dest => dest.WalkOver, expression => expression.UseValue(false))

                //.ForMember(dest => dest.Channels, expression => expression.UseValue(des))

                // Ignore elements not used even by legacy feed
                .ForAllOtherMembers(dest => dest.Ignore());
        }
    }
}