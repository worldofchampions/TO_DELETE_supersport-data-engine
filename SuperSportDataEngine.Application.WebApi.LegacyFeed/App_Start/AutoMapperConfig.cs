using AutoMapper;
using SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Mappers;
using SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Mappers.Motorsport;
using SuperSportDataEngine.ApplicationLogic.Entities.Legacy.Mappers;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed
{
    public class AutoMapperConfig
    {
        public static void InitializeMappings()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<LegacyLogMapperProfile>();
                cfg.AddProfile<LegacyGroupedLogMapperProfile>();
                cfg.AddProfile<LegacyAuthMappingProfile>();
                cfg.AddProfile<LegacyMatchModelMapperProfile>();
                cfg.AddProfile<LegacyFixtureMapperProfile>();
                cfg.AddProfile<LegacyResultMapperProfile>();
                cfg.AddProfile<LegacyMatchStatsMapperProfile>();
                cfg.AddProfile<LegacyMatchDetailsMapperProfile>();
                cfg.AddProfile<LegacyMatchDetailsArticlesAndVideosMapperProfile>();
                cfg.AddProfile<LegacyCommentaryMapperProfile>();
                cfg.AddProfile<LegacyMatchEventsMapperProfile>();
                cfg.AddProfile<LegacyCommentaryAsEventMapperProfile>();
                cfg.AddProfile<LegacyTeamsheetMapperProfile>();
                cfg.AddProfile<LegacyScorerModelMapperProfile>();
                cfg.AddProfile<LegacyPointScorerMapperProfile>();

                cfg.AddProfile<LegacyMotorsportScheduleMapperProfile>();
            });
#if DEBUG
            Mapper.AssertConfigurationIsValid();
#endif
        }
    }
}