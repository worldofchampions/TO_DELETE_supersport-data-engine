using AutoMapper;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Mappers
{
    public class LegacyFixtureMapperProfile : Profile
    {
        public LegacyFixtureMapperProfile()
        {
            CreateMap<RugbyFixture, FixtureModel>()

                // For Away team 
                .ForMember(dest => dest.AwayTeam, expression => expression.MapFrom(
                    src => src.TeamAIsHomeTeam ? src.TeamB.Name : src.TeamA.Name))

                .ForMember(dest => dest.AwayTeamScore, expression => expression.MapFrom(
                    src => src.TeamAIsHomeTeam ? src.TeamBScore.ToString() : src.TeamAScore.ToString()))

                .ForMember(dest => dest.AwayTeamId, expression => expression.MapFrom(
                    src => src.TeamAIsHomeTeam ? src.TeamB.LegacyTeamId : src.TeamA.LegacyTeamId))

                .ForMember(dest => dest.AwayTeamShortName, expression => expression.MapFrom(
                    src => src.TeamAIsHomeTeam ? src.TeamB.Abbreviation : src.TeamA.Abbreviation))

                // For Home team 
                .ForMember(dest => dest.HomeTeam, expression => expression.MapFrom(
                    src => src.TeamAIsHomeTeam ? src.TeamA.Name : src.TeamB.Name))

                .ForMember(dest => dest.HomeTeamScore, expression => expression.MapFrom(
                   src => src.TeamAIsHomeTeam ? src.TeamAScore.ToString() : src.TeamBScore.ToString()))

                .ForMember(dest => dest.HomeTeamId, expression => expression.MapFrom(
                    src => src.TeamAIsHomeTeam ? src.TeamA.LegacyTeamId : src.TeamB.LegacyTeamId))

                .ForMember(dest => dest.HomeTeamShortName, expression => expression.MapFrom(
                    src => src.TeamAIsHomeTeam ? src.TeamA.Abbreviation : src.TeamB.Abbreviation))


                // For Fixture specific
                .ForMember(dest => dest.LeagueId, expression => expression.MapFrom(src => src.RugbyTournament.LegacyTournamentId))

                .ForMember(dest => dest.LeagueName, expression => expression.MapFrom(src => src.RugbyTournament.Name))

                .ForMember(dest => dest.LeagueShortName, expression => expression.MapFrom(src => src.RugbyTournament.Abbreviation))

                .ForMember(dest => dest.LeagueUrlName, expression => expression.MapFrom(src => src.RugbyTournament.LogoUrl))

                .ForMember(dest => dest.Location, expression => expression.MapFrom(src => src.RugbyVenue))

                .ForMember(dest => dest.MatchDateTime, expression => expression.MapFrom(src => src.StartDateTime.UtcDateTime))

                .ForMember(dest => dest.MatchDateTimeString, expression => expression.MapFrom(src => src.StartDateTime.UtcDateTime.ToLongDateString()))
                
                .ForMember(dest => dest.MatchID, expression => expression.MapFrom(src => src.LegacyFixtureId))

                .ForMember(dest => dest.Result, expression => expression.MapFrom(src => src.RugbyFixtureStatus == RugbyFixtureStatus.GameEnd))

                .ForMember(dest => dest.Sport, expression => expression.UseValue(SportType.Rugby))

                // TODO: Confirm where these come from?
                .ForMember(dest => dest.International, expression => expression.UseValue(false))

                .ForMember(dest => dest.IsFeatured, expression => expression.UseValue(false))

                .ForMember(dest => dest.WalkOver, expression => expression.UseValue(false))

                // For data exclusive to old feed
                .ForAllOtherMembers(dest => dest.Ignore());
        }
    }
}