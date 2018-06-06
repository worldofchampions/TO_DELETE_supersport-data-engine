namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Mappers.Motorsport
{
    using AutoMapper;
    using SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Motorsport;
    using SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Shared;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
    using System;
    using System.Collections.Generic;
    using System.Web.WebPages;

    // TODO: Refactor implementation to use common mapping logic for LegacyMotorsportScheduleMapperProfile + LegacyMotorsportScheduleWithResultsMapperProfile classes.
    // This class is used by Reflection.
    public class LegacyMotorsportScheduleMapperProfile : Profile
    {
        public LegacyMotorsportScheduleMapperProfile()
        {
            CreateMap<MotorsportRaceEvent, Races>()
                .ForMember(dest => dest.Id, expression => expression.MapFrom(
                    src => src.LegacyRaceEventId))

                .ForMember(dest => dest.Category, expression => expression.MapFrom(
                    src => src.MotorsportSeason.MotorsportLeague.Slug))

                .ForMember(dest => dest.TournamentId, expression => expression.MapFrom(
                    src => src.MotorsportRace.LegacyRaceId))

                .ForMember(dest => dest.ResultId, expression => expression.UseValue(0))

                .ForMember(dest => dest.EventId, expression => expression.MapFrom(
                    src => src.LegacyRaceEventId))

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
                        ? ((DateTimeOffset)src.StartDateTimeUtc).ToLocalTime().ToString("s")
                        : ""))

                // [TODO] This value is set to the minimum date time because we do not get it from the provider.
                .ForMember(dest => dest.EndDate, expression => expression.UseValue(DateTime.MinValue.ToString("s")))

                .ForMember(dest => dest.winner, expression => expression.MapFrom(src =>
                    src.RaceEventWinner != null && src.RaceEventWinner.FullNameCmsOverride != null ? src.RaceEventWinner.FullNameCmsOverride :
                    GetFullName(src.RaceEventWinner != null ? src.RaceEventWinner.FirstName : null,
                                src.RaceEventWinner != null ? src.RaceEventWinner.LastName : null)))

                .ForMember(dest => dest.Videos, expression => expression.UseValue(new List<MatchVideoModel>()))
                .ForMember(dest => dest.LiveVideos, expression => expression.UseValue(new List<MatchLiveVideoModel>()))

                .ForAllOtherMembers(m => m.Ignore());
        }

        private static string GetFullName(string firstName, string lastName)
        {
            var name = firstName ?? "";
            var surname = lastName ?? "";

            var fullName = (name + " " + surname).Trim();
            return fullName.IsEmpty() ? null : fullName;
        }
    }
}
