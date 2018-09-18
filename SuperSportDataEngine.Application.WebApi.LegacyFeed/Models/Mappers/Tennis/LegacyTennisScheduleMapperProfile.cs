using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models.Tennis;
using SuperSportDataEngine.ApplicationLogic.Entities.Legacy.Tennis;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Mappers.Tennis
{
    public class LegacyTennisScheduleMapperProfile : Profile
    {
        public LegacyTennisScheduleMapperProfile()
        {
            CreateMap<TennisEventEntity, Models.Tennis.TennisTournament>()
                .ForMember(dest => dest.Id, expression => expression.MapFrom(
                    src => src.TennisEvent.LegacyEventId))

                .ForMember(dest => dest.Title, expression => expression.MapFrom(
                    src => src.TennisEvent.EventName))

                .ForMember(dest => dest.Location, expression => expression.MapFrom(
                    src => string.Join(", ", src.TennisEvent.TennisVenue.City, src.TennisEvent.TennisVenue.TennisCountry.Country)))

                .ForMember(dest => dest.Type, expression => expression.MapFrom(
                    src => src.TennisEvent.TennisTournament.TennisTournamentType.ToString()))

                .ForMember(dest => dest.Surface, expression => expression.MapFrom(
                    src => src.TennisEvent.TennisSurfaceType.NameCmsOverride ?? src.TennisEvent.TennisSurfaceType.Name))

                .ForMember(dest => dest.StartDate, expression => expression.MapFrom(
                    src => src.TennisEvent.StartDateUtc.ToString("s")))

                .ForMember(dest => dest.EndDate, expression => expression.MapFrom(
                    src => src.TennisEvent.EndDateUtc.ToString("s")))

                .ForMember(dest => dest.Currency, expression => expression.MapFrom(
                    src => string.Empty))

                .ForMember(dest => dest.PrizeMoney, expression => expression.MapFrom(
                    src => src.TennisEventTennisLeague.PrizeCmsOverride ?? src.TennisEventTennisLeague.Prize ?? ""))

                .ForAllOtherMembers(m => m.Ignore());
        }
    }
}