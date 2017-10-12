using AutoMapper;
using SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Rugby;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Mappers
{
    public class LegacyMatchDetailsMapperProfile : Profile
    {
        public LegacyMatchDetailsMapperProfile()
        {
            CreateMap<RugbyMatchDetails, RugbyMatchDetailsModel>()

               .ForMember(dest => dest.MatchStatisticsTeamA, exp => exp.MapFrom(
                    src => src.TeamAMatchStatistics))

                .ForMember(dest => dest.MatchStatisticsTeamB, exp => exp.MapFrom(
                    src => src.TeamBMatchStatistics))

                .ForMember(dest => dest.TeamAStats, exp => exp.MapFrom(
                    src => src.TeamAMatchStatistics))

                .ForMember(dest => dest.TeamBStats, exp => exp.MapFrom(
                    src => src.TeamBMatchStatistics))

               .ForMember(dest => dest.Commentary, exp => exp.MapFrom(
                    src => src.Commentary))

               .ForAllOtherMembers(dest => dest.Ignore());
        }
    }
}