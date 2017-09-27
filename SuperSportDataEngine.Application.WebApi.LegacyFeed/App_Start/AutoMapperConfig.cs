using AutoMapper;
using SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Mappers;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models.Legacy;
using SuperSportDataEngine.ApplicationLogic.Entities.Legacy.Mappers;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.App_Start
{
    public class AutoMapperConfig
    {
        public static void InitializeMappings()
        {

            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<LogModelMapperProfile>();
                cfg.AddProfile<LogEntityMapperProfile>();
                cfg.AddProfile<LegacyAuthMappingProfile>();
                cfg.AddProfile<LegacyFixtureMapperProfile>();
                cfg.AddProfile<LegacyResultMapperProfile>();
            });
#if DEBUG
            Mapper.AssertConfigurationIsValid();
#endif
        }
    }
}