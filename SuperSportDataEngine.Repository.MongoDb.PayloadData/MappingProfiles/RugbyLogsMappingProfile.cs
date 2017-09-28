using AutoMapper;
using SuperSportDataEngine.Repository.MongoDb.PayloadData.Models.RugbyLogs;

namespace SuperSportDataEngine.Repository.MongoDb.PayloadData.MappingProfiles
{
    public class RugbyLogsMappingProfile : Profile
    {
        public RugbyLogsMappingProfile()
        {
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyFlatLogs.RugbyFlatLogs, MongoRugbyLogs>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyFlatLogs.RecentFormList, RecentFormList>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyFlatLogs.Ladderposition, Ladderposition>().ReverseMap();
        }
    }
}
