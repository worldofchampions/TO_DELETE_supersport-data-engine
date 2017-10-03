using AutoMapper;
using SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Mappers;
using SuperSportDataEngine.ApplicationLogic.Entities.Legacy.Mappers;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.App_Start
{
    public class AutoMapperConfig
    {
        public static void InitializeMappings()
        {

            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<LegacyLogMapperProfile>();
                cfg.AddProfile<LegacyAuthMappingProfile>();
                cfg.AddProfile<LegacyMatchModelMapperProfile>();
                cfg.AddProfile<LegacyFixtureMapperProfile>();
                cfg.AddProfile<LegacyResultMapperProfile>();
            });
#if DEBUG
            Mapper.AssertConfigurationIsValid();
#endif
        }
    }
}