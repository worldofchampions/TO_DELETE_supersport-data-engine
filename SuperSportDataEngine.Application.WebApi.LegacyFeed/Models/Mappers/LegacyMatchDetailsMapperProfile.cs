using AutoMapper;
using SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Rugby;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Mappers
{
    public class LegacyMatchDetailsMapperProfile : Profile
    {
        public LegacyMatchDetailsMapperProfile()
        {
            CreateMap<RugbyMatchStatistics, RugbyMatchDetailsModel>()
                //TODO
                .ForAllOtherMembers(dest => dest.Ignore());
        }
    }
}