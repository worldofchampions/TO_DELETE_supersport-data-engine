using AutoMapper;
using SuperSportDataEngine.Repository.MongoDb.PayloadData.Models.RugbyLogs;

namespace SuperSportDataEngine.Repository.MongoDb.PayloadData.MappingProfiles
{
    public class RugbyLogsMappingProfile : Profile
    {
        public RugbyLogsMappingProfile()
        {
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyLogs.RugbyLogs, MongoRugbyLogs>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyLogs.RecentFormList, RecentFormList>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyLogs.Ladderposition, Ladderposition>().ReverseMap();
        }
    }
}
