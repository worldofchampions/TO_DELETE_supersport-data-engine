using AutoMapper;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Mappers
{
    public class LegacyTeamsheetMapperProfile: Profile
    {
        public LegacyTeamsheetMapperProfile()
        {
            CreateMap<RugbyPlayerLineup, Player>()

                .ForMember(dest => dest.IsSubstitute, exp => exp.MapFrom(src => src.IsSubstitute))

                .ForMember(dest => dest.TeamId, exp => exp.MapFrom(src => src.RugbyTeam.LegacyTeamId))

                .ForMember(dest => dest.Name, exp => exp.MapFrom(src => src.RugbyPlayer.FirstName))
                
                .ForMember(dest => dest.Surname, exp => exp.MapFrom(src => src.RugbyPlayer.LastName))

                .ForMember(dest => dest.CombinedName, exp => exp.MapFrom(src => src.RugbyPlayer.FullName))

                .ForMember(dest => dest.DisplayName, exp => exp.MapFrom(src => src.RugbyPlayer.FullName))

                .ForMember(dest => dest.IsCaptain, exp => exp.MapFrom(src => src.IsCaptain))

                .ForMember(dest => dest.PositionName, exp => exp.MapFrom(src => src.PositionName))

                .ForMember(dest => dest.PersonId, exp => exp.MapFrom(src => src.RugbyPlayer.LegacyPlayerId))

                .ForMember(dest => dest.Number, exp => exp.MapFrom(src => src.JerseyNumber))

                .ForAllOtherMembers(dest => dest.Ignore());
        }
    }
}