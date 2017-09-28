using AutoMapper;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyGroupedLogs;
using SuperSportDataEngine.Repository.MongoDb.PayloadData.Models.RugbyLogsGrouped;

namespace SuperSportDataEngine.Repository.MongoDb.PayloadData.MappingProfiles
{
    public class RugbyGroupedLogsMappingProfile : Profile
    {
        public RugbyGroupedLogsMappingProfile()
        {
            CreateMap<RugbyGroupedLogs, MongoRugbyGroupedLogs>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyGroupedLogs.Ladderposition, Models.RugbyLogsGrouped.Ladderposition>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyGroupedLogs.RecentFormList, Models.RugbyLogsGrouped.RecentFormList>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyGroupedLogs.OverallStandings, Models.RugbyLogsGrouped.OverallStandings>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyGroupedLogs.GroupStandings, Models.RugbyLogsGrouped.GroupStandings>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyGroupedLogs.SecondaryGroupStandings, Models.RugbyLogsGrouped.SecondaryGroupStandings>().ReverseMap();
        }
    }
}
