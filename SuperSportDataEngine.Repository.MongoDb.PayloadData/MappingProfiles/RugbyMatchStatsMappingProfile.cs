using AutoMapper;
using SuperSportDataEngine.Repository.MongoDb.PayloadData.Models.MongoRugbyMatchStats;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyMatchStats;

namespace SuperSportDataEngine.Repository.MongoDb.PayloadData.MappingProfiles
{
    // This class is used by Reflection.
    public class RugbyMatchStatsMappingProfile : Profile
    {
        public RugbyMatchStatsMappingProfile()
        {
            CreateMap<RugbyMatchStats, MongoRugbyMatchStats>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyMatchStats.GameInfo, Models.MongoRugbyMatchStats.GameInfo>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyMatchStats.Score, Models.MongoRugbyMatchStats.Score>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyMatchStats.InOut, Models.MongoRugbyMatchStats.InOut>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyMatchStats.InsAndOuts, Models.MongoRugbyMatchStats.InsAndOuts>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyMatchStats.PeriodStat, Models.MongoRugbyMatchStats.PeriodStat>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyMatchStats.Period, Models.MongoRugbyMatchStats.Period>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyMatchStats.PeriodStats, Models.MongoRugbyMatchStats.PeriodStats>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyMatchStats.MatchStats, Models.MongoRugbyMatchStats.MatchStats>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyMatchStats.MatchStat, Models.MongoRugbyMatchStats.MatchStat>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyMatchStats.PlayerStats, Models.MongoRugbyMatchStats.PlayerStats>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyMatchStats.TeamPlayer, Models.MongoRugbyMatchStats.TeamPlayer>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyMatchStats.TeamLineup, Models.MongoRugbyMatchStats.TeamLineup>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyMatchStats.TeamStats, Models.MongoRugbyMatchStats.TeamStats>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyMatchStats.TeamsMatch, Models.MongoRugbyMatchStats.TeamsMatch>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyMatchStats.Teams, Models.MongoRugbyMatchStats.Teams>().ReverseMap();
        }
    }
}
