using AutoMapper;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Mappers
{
    using SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Rugby;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;

    // This class is used by Reflection.
    public class LegacyPointScorerMapperProfile : Profile
    {
        public LegacyPointScorerMapperProfile()
        {
            CreateMap<RugbyPlayerStatistics, RugbyPointsScorerModel>()

                .ForMember(dest => dest.Tries, exp => exp.MapFrom(src => src.TriesScored))

                .ForMember(dest => dest.Conversions, exp => exp.MapFrom(src => src.ConversionsScored))

                .ForMember(dest => dest.DropGoals, exp => exp.MapFrom(src => src.DropGoalsScored))

                .ForMember(dest => dest.Penalties, exp => exp.MapFrom(src => src.PenaltiesScored))

                .ForMember(dest => dest.Rank, exp => exp.MapFrom(src => src.Rank))

                .ForMember(dest => dest.TotalPoints, exp => exp.MapFrom(src => src.TotalPoints))

                .ForMember(dest => dest.LeagueName, exp => exp.MapFrom(
                    src => src.RugbyTournament.NameCmsOverride ?? src.RugbyTournament.Name))

                .ForMember(dest => dest.LeagueUrlName, exp => exp.MapFrom(src => src.RugbyTournament.Slug))

                .ForMember(dest => dest.TeamId, exp => exp.MapFrom(src => src.RugbyTeam.LegacyTeamId))

                .ForMember(dest => dest.TeamName, exp => exp.MapFrom(
                    src => src.RugbyTeam.NameCmsOverride ?? src.RugbyTeam.Name))

                .ForMember(dest => dest.Name, exp => exp.MapFrom(src => src.RugbyPlayer.FirstName))

                .ForMember(dest => dest.Surname, exp => exp.MapFrom(src => src.RugbyPlayer.LastName))

                .ForMember(dest => dest.CombinedName, exp => exp.MapFrom(
                    src => src.RugbyPlayer.DisplayNameCmsOverride ?? src.RugbyPlayer.FullName ?? src.RugbyPlayer.FirstName))

                .ForMember(dest => dest.DisplayName, exp => exp.MapFrom(
                    src => src.RugbyPlayer.DisplayNameCmsOverride ?? src.RugbyPlayer.FullName ?? src.RugbyPlayer.FirstName))

                .ForMember(dest => dest.PersonId, exp => exp.MapFrom(src => src.RugbyPlayer.LegacyPlayerId))

                .ForAllOtherMembers(dest => dest.Ignore());
        }
    }
}