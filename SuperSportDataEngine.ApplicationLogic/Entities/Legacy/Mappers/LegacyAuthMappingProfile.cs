using AutoMapper;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models;

namespace SuperSportDataEngine.ApplicationLogic.Entities.Legacy.Mappers
{
    public class LegacyAuthMappingProfile : Profile
    {
        public LegacyAuthMappingProfile()
        {
            CreateMap<LegacyZoneSiteEntity, LegacyZoneSite>();
        }
    }
}