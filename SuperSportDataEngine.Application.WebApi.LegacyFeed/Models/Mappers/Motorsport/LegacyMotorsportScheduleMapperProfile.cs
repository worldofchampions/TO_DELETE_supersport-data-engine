using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Shared;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Mappers.Motorsport
{
    public class LegacyMotorsportScheduleMapperProfile : Profile
    {
        public LegacyMotorsportScheduleMapperProfile()
        {
            CreateMap<MotorsportRaceEvent, RacesModel>()
                .ForMember(dest => dest.Id, expression => expression.MapFrom(
                    src => src.LegacyRaceEventId))

                .ForMember(dest => dest.Category, expression => expression.MapFrom(
                    src => src.MotorsportSeason.MotorsportLeague.Name))

                .ForMember(dest => dest.TournamentId, expression => expression.MapFrom(
                    src => src.MotorsportRace.LegacyRaceId))

                .ForMember(dest => dest.ResultId, expression => expression.UseValue(0))

                .ForMember(dest => dest.EventId, expression => expression.UseValue(0))

                .ForMember(dest => dest.Name, expression => expression.MapFrom(
                    src => src.MotorsportRace.RaceNameCmsOverride ?? src.MotorsportRace.RaceName))

                .ForMember(dest => dest.ShortName, expression => expression.MapFrom(
                    src => src.MotorsportRace.RaceNameAbbreviationCmsOverride ??
                           src.MotorsportRace.RaceNameAbbreviation))

                .ForMember(dest => dest.Venue, expression => expression.MapFrom(
                    src => src.CircuitName))

                .ForMember(dest => dest.City, expression => expression.MapFrom(
                    src => src.CityName))

                .ForMember(dest => dest.Country, expression => expression.MapFrom(
                    src => src.CountryName))

                .ForMember(dest => dest.Date, expression => expression.MapFrom(
                    src => src.StartDateTimeUtc != null
                        ? ((DateTimeOffset) src.StartDateTimeUtc).ToLocalTime().ToString()
                        : ""))
                .ForAllOtherMembers(m => m.Ignore());

                
        }
    }
}