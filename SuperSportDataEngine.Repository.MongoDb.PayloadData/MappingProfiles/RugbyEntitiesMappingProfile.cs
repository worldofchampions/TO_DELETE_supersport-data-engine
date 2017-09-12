using System;
using AutoMapper;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models;
using SuperSportDataEngine.Repository.MongoDb.PayloadData.Models.RugbyEntities;

namespace SuperSportDataEngine.Repository.MongoDb.PayloadData.MappingProfiles
{
    public class RugbyEntitiesMappingProfile : Profile
    {
        public RugbyEntitiesMappingProfile()
        {
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyEntities.RugbyEntities, MongoRugbyEntities>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyEntities.GroundCondition, Models.RugbyEntities.GroundCondition>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyEntities.Competition, Models.RugbyEntities.Competition>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyEntities.Player, Models.RugbyEntities.Player>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyEntities.Position, Models.RugbyEntities.Position>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyEntities.Round, Models.RugbyEntities.Round>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyEntities.ScoringMethod, Models.RugbyEntities.ScoringMethod>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyEntities.Season, Models.RugbyEntities.Season>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyEntities.Statistic, Models.RugbyEntities.Statistic>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyEntities.Team, Models.RugbyEntities.Team>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyEntities.Venue, Models.RugbyEntities.Venue>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyEntities.WeatherCondition, Models.RugbyEntities.WeatherCondition>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyEntities.GameState, Models.RugbyEntities.GameState>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyEntities.Official, Models.RugbyEntities.Official>().ReverseMap();
        }
    }
}