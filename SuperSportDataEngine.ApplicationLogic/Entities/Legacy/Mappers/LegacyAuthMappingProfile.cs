using AutoMapper;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models;

namespace SuperSportDataEngine.ApplicationLogic.Entities.Legacy.Mappers
{
    // This class is used by Reflection.
    public class LegacyAuthMappingProfile : Profile
    {
        public LegacyAuthMappingProfile()
        {
            CreateMap<LegacyZoneSiteEntity, LegacyZoneSite>()
                .ForAllOtherMembers(dest => dest.Ignore());
        }
    }
}