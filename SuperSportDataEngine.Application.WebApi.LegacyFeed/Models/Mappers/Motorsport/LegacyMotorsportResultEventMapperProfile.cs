using AutoMapper;
using SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Motorsport;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
using SuperSportDataEngine.ApplicationLogic.Entities.Legacy.Motorsport;
using SuperSportDataEngine.Common.Helpers;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Mappers.Motorsport
{
    // This class is used by Reflection.
    public class LegacyMotorsportResultEventMapperProfile : Profile
    {
        public LegacyMotorsportResultEventMapperProfile()
        {
            CreateMap<MotorsportRaceEventResultsEntity, MotorsportResult>()
                .ForMember(dest => dest.RaceResult, expression => expression.MapFrom(
                    src => src.MotorsportRaceEventResults))

                .ForMember(dest => dest.Id, expression => expression.MapFrom(
                    src => src.MotorsportRaceEvent.LegacyRaceEventId))

                .ForMember(dest => dest.Date, expression => expression.MapFrom(
                    src => src.MotorsportRaceEvent.StartDateTimeUtc.Value.ConvertToLocalSAST().Date.ToString("s")))

                .ForMember(dest => dest.StartTime, expression => expression.MapFrom(
                    src => src.MotorsportRaceEvent.StartDateTimeUtc.Value.ConvertToLocalSAST().ToString("HH:mm")))

                .ForMember(dest => dest.EndTime, expression => expression.UseValue(""))

                .ForMember(dest => dest.Name, expression => expression.MapFrom(
                    src => src.MotorsportRaceEvent.MotorsportRace.RaceName))

                .ForMember(dest => dest.Country, expression => expression.MapFrom(
                    src => src.MotorsportRaceEvent.CountryName))

                .ForMember(dest => dest.Circuit, expression => expression.MapFrom(
                    src => src.MotorsportRaceEvent.CircuitName))

                .ForMember(dest => dest.RaceEventStatus, expression => expression.MapFrom(
                    src => FormatRaceEventStatusForResponse(src.MotorsportRaceEvent.MotorsportRaceEventStatus)))

                .ForAllOtherMembers(m => m.Ignore());
        }

        private static string FormatRaceEventStatusForResponse(MotorsportRaceEventStatus motorsportRaceEventStatus)
        {
            switch (motorsportRaceEventStatus)
            {
                case MotorsportRaceEventStatus.PreRace:
                    return "PreRace";
                case MotorsportRaceEventStatus.InProgress:
                    return "InProgress";
                case MotorsportRaceEventStatus.Result:
                    return "Results";
                case MotorsportRaceEventStatus.Postponed:
                    return "Postponed";
                case MotorsportRaceEventStatus.Suspended:
                    return "Suspended";
                case MotorsportRaceEventStatus.Delayed:
                    return "Delayed";
                case MotorsportRaceEventStatus.Cancelled:
                    return "Cancelled";
                case MotorsportRaceEventStatus.Unknown:
                    return string.Empty;
                default:
                        return string.Empty;
            }
        }
    }
}