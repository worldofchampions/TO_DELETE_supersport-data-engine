using AutoMapper;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;

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