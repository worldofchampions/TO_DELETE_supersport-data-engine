using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.WebPages;
using AutoMapper;
using SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Motorsport;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
using SuperSportDataEngine.ApplicationLogic.Entities.Legacy.Motorsport;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Mappers.Motorsport
{
    public class LegacyMotorsportResultEventMapperProfile : Profile
    {
        public LegacyMotorsportResultEventMapperProfile()
        {
            //CreateMap<MotorsportRaceEventResultsEntity, ResultMotorsportModel>()
            //    //.ForMember(dest => dest.Race, expression => expression.MapFrom(
            //    //    src => src.MotorsportRaceEventGrids))

            //    //.ForMember(dest => dest.Id, expression => expression.MapFrom(
            //    //    src => src.MotorsportRaceEvent.LegacyRaceEventId))

            //    //.ForMember(dest => dest.Date, expression => expression.MapFrom(
            //    //    src => src.MotorsportRaceEvent.StartDateTimeUtc.Value.LocalDateTime.Date.ToString("s")))

            //    //.ForMember(dest => dest.StartTime, expression => expression.MapFrom(
            //    //    src => src.MotorsportRaceEvent.StartDateTimeUtc.Value.LocalDateTime.ToString("HH:mm")))

            //    //.ForMember(dest => dest.EndTime, expression => expression.UseValue(""))

            //    //.ForMember(dest => dest.Name, expression => expression.MapFrom(
            //    //    src => src.MotorsportRaceEvent.MotorsportRace.RaceName))

            //    //.ForMember(dest => dest.Country, expression => expression.MapFrom(
            //    //    src => src.MotorsportRaceEvent.CountryName))

            //    //.ForMember(dest => dest.Circuit, expression => expression.MapFrom(
            //    //    src => src.MotorsportRaceEvent.CircuitName))

            //    .ForAllOtherMembers(m => m.Ignore());
        }

        private static string GetTime(MotorsportRaceEventGrid grid)
        {
            var hours = grid.QualifyingTimeHours == 0 ? "" : grid.QualifyingTimeHours + ":";
            var minutes = grid.QualifyingTimeMinutes == 0 ? "" : grid.QualifyingTimeMinutes + ":";
            var seconds = grid.QualifyingTimeSeconds == 0 ? "" : grid.QualifyingTimeSeconds + ".";
            var milliseconds = grid.QualifyingTimeMilliseconds == 0 ? "" : grid.QualifyingTimeMilliseconds.ToString();

            return hours + minutes + seconds + milliseconds;
        }
    }
}