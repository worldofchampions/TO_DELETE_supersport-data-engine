using AutoMapper;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Mappers
{
    public class LegacyGroupedLogMapperProfile : Profile
    {
        public LegacyGroupedLogMapperProfile()
        {
            CreateMap<RugbyGroupedLog, Log>()

                .ForMember(dest => dest.GroupName, expression => expression.MapFrom(
                    src => src.RugbyLogGroup.GroupName))

                .ForMember(dest => dest.GroupShortName, expression => expression.MapFrom(
                   src => src.RugbyLogGroup.GroupName))

                .ForMember(dest => dest.LeagueName, expression => expression.MapFrom(
                    src => src.RugbyTournament.Name))

                .ForMember(dest => dest.Team, expression => expression.MapFrom(
                    src => src.RugbyTeam.Name))

                .ForMember(dest => dest.TeamShortName, expression => expression.MapFrom(
                    src => src.RugbyTeam.Name))

                .ForMember(dest => dest.TeamID, expression => expression.MapFrom(
                    src => src.RugbyTeam.LegacyTeamId))

                .ForMember(dest => dest.Position, expression => expression.MapFrom(
                    src => src.LogPosition))

                .ForMember(dest => dest.Played, expression => expression.MapFrom(
                    src => src.GamesPlayed))

                .ForMember(dest => dest.Won, expression => expression.MapFrom(
                    src => src.GamesWon))

                .ForMember(dest => dest.Drew, expression => expression.MapFrom(
                    src => src.GamesDrawn))

                .ForMember(dest => dest.Lost, expression => expression.MapFrom(
                    src => src.GamesLost))

                .ForMember(dest => dest.PointsFor, expression => expression.MapFrom(
                    src => src.PointsFor))

                .ForMember(dest => dest.PointsAgainst, expression => expression.MapFrom(
                    src => src.PointsAgainst))

                .ForMember(dest => dest.BonusPoints, expression => expression.MapFrom(
                    src => src.BonusPoints))

                .ForMember(dest => dest.PointsDifference, expression => expression.MapFrom(
                    src => src.PointsDifference))

                .ForMember(dest => dest.Points, expression => expression.MapFrom(
                    src => src.TournamentPoints))

                .ForMember(dest => dest.Sport, src => src.UseValue(SportType.Rugby))

                .ForMember(dest => dest.TriesFor, expression => expression.MapFrom(
                    src => src.TriesFor))

                .ForMember(dest => dest.TriesAgainst, expression => expression.MapFrom(
                    src => src.TriesAgainst))

                .ForMember(dest => dest.TriesBonusPoints, src => src.UseValue(0))

                .ForMember(dest => dest.LossBonusPoints, src => src.UseValue(0))

                .ForAllOtherMembers(dest => dest.Ignore());
        }
    }
}