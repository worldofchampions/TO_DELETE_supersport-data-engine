using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Motorsport;
using SuperSportDataEngine.Repository.MongoDb.PayloadData.Models;

namespace SuperSportDataEngine.Repository.MongoDb.PayloadData.MappingProfiles
{
    public class MotorsportEntitiesMappingProfile : Profile
    {
        public MotorsportEntitiesMappingProfile()
        {
            CreateMap<MotorsportEntitiesResponse, MongoMotorsportEntitiesResponse>().ReverseMap();
            CreateMap<ApiResult, MongoMotorsportApiResult>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Motorsport.UriPath, Models.UriPath>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Motorsport.League2, Models.League2>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Motorsport.UriPath2, Models.UriPath2>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Motorsport.League, Models.League>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Motorsport.Season, Models.Season>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Motorsport.EventType, Models.EventType>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Motorsport.EndDate, Models.EndDate>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Motorsport.Event, Models.Event>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Motorsport.Venue, Models.Venue>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Motorsport.TvStation, Models.TvStation>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Motorsport.NetworkType, Models.NetworkType>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Motorsport.CompletedDistance, Models.CompletedDistance>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Motorsport.Cautions, Models.Cautions>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Motorsport.FastestSpeed, Models.FastestSpeed>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Motorsport.FastestTime, Models.FastestTime>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Motorsport.PracticeEvent, Models.PracticeEvent>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Motorsport.CoverageLevel, Models.CoverageLevel>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Motorsport.TimeOfRace, Models.TimeOfRace>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Motorsport.VictoryMargin, Models.VictoryMargin>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Motorsport.PoleTime, Models.PoleTime>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Motorsport.Result, Models.Result>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Motorsport.Points, Models.Points>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Motorsport.Champion, Models.Champion>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Motorsport.ScheduledDistance, Models.ScheduledDistance>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Motorsport.Driver, Models.Driver>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Motorsport.FinishingTime, Models.FinishingTime>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Motorsport.Boxscore, Models.Boxscore>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Motorsport.EventRound, Models.EventRound>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Motorsport.Race, Models.Race>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Motorsport.IsDataConfirmed, Models.IsDataConfirmed>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Motorsport.EventStatus, Models.EventStatus>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Motorsport.SubLeague, Models.SubLeague>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Motorsport.Player, Models.Player>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Motorsport.Birth, Models.Birth>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Motorsport.BirthDate, Models.BirthDate>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Motorsport.Country, Models.Country>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Motorsport.State, Models.State>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Motorsport.Weight, Models.Weight>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Motorsport.Height, Models.Height>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Motorsport.Tire, Models.Tire>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Motorsport.Engine, Models.Engine>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Motorsport.Chassis, Models.Chassis>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Motorsport.Qualifying, Models.Qualifying>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Motorsport.QualifyingEvent, Models.QualifyingEvent>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Motorsport.QualifyingRun, Models.QualifyingRun>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Motorsport.CarDetails, Models.CarDetails>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Motorsport.StartDate, Models.StartDate>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Motorsport.Exemption, Models.Exemption>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Motorsport.LeadChange, Models.LeadChange>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Motorsport.AverageSpeed, Models.AverageSpeed>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Motorsport.Time, Models.Time>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Motorsport.Car, Models.Car>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Motorsport.Make, Models.Make>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Motorsport.Laps, Models.Laps>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Motorsport.CarPosition, Models.CarPosition>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Motorsport.CarStatus, Models.CarStatus>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Motorsport.Finishes, Models.Finishes>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Motorsport.Owner, Models.Owner>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Motorsport.DateThrough, Models.DateThrough>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Motorsport.LastEvent, Models.LastEvent>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Motorsport.Team, Models.Team>().ReverseMap();
            CreateMap<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Motorsport.Standings, Models.Standings>().ReverseMap();

        }
    }
}
