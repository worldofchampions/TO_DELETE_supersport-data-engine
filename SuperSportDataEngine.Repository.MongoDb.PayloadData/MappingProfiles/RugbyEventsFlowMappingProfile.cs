using AutoMapper;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyEventsFlow;
using SuperSportDataEngine.Repository.MongoDb.PayloadData.Models.MongoRugbyEventsFlow;

namespace SuperSportDataEngine.Repository.MongoDb.PayloadData.MappingProfiles
{
    public class RugbyEventsFlowMappingProfile : Profile
    {
        public RugbyEventsFlowMappingProfile()
        {
            CreateMap<RugbyEventsFlow, MongoRugbyEventsFlow>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyEventsFlow.CommentaryEvent, Models.MongoRugbyEventsFlow.CommentaryEvent>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyEventsFlow.CommentaryFlow, Models.MongoRugbyEventsFlow.CommentaryFlow>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyEventsFlow.ErrorEvent, Models.MongoRugbyEventsFlow.ErrorEvent>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyEventsFlow.ErrorFlow, Models.MongoRugbyEventsFlow.ErrorFlow>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyEventsFlow.InterchangeFlow, Models.MongoRugbyEventsFlow.InterchangeFlow>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyEventsFlow.PenaltyEvent, Models.MongoRugbyEventsFlow.PenaltyEvent>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyEventsFlow.PenaltyFlow, Models.MongoRugbyEventsFlow.PenaltyFlow>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyEventsFlow.ScoreEvent, Models.MongoRugbyEventsFlow.ScoreEvent>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyEventsFlow.StatErrorEvent, Models.MongoRugbyEventsFlow.StatErrorEvent>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyEventsFlow.StatScoringEvent, Models.MongoRugbyEventsFlow.StatScoringEvent>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyEventsFlow.StatPenaltyEvent, Models.MongoRugbyEventsFlow.StatPenaltyEvent>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyEventsFlow.Team, Models.MongoRugbyEventsFlow.Team>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyEventsFlow.Team2, Models.MongoRugbyEventsFlow.Team2>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyEventsFlow.ScoreFlow, Models.MongoRugbyEventsFlow.ScoreFlow>().ReverseMap();
        }
    }
}
