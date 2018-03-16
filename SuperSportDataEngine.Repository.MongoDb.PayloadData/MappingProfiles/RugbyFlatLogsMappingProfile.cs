using AutoMapper;
using SuperSportDataEngine.Repository.MongoDb.PayloadData.Models.RugbyLogsFlat;

namespace SuperSportDataEngine.Repository.MongoDb.PayloadData.MappingProfiles
{
    // This class is used by Reflection.
    public class RugbyFlatLogsMappingProfile : Profile
    {
        public RugbyFlatLogsMappingProfile()
        {
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyFlatLogs.RugbyFlatLogs, MongoRugbyFlatLogs>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyFlatLogs.RecentFormList, RecentFormList>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyFlatLogs.Ladderposition, Ladderposition>().ReverseMap();
        }
    }
}
