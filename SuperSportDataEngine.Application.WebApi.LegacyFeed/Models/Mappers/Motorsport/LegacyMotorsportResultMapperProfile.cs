using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.WebPages;
using AutoMapper;
using SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Motorsport;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Mappers.Motorsport
{
    // This class is used by Reflection.
    public class LegacyMotorsportResultMapperProfile : Profile
    {
        public LegacyMotorsportResultMapperProfile()
        {
            CreateMap<MotorsportRaceEventResult, ResultMotorsportModel>()
                .ForMember(dest => dest.Position, expression => expression.MapFrom(
                    src => src.GridPosition))

                .ForMember(dest => dest.PositionText, expression => expression.UseValue((string) null))

                .ForMember(dest => dest.Time, expression => expression.MapFrom(
                    src => GetTime(src)))

                .ForMember(dest => dest.Points, expression => expression.UseValue((string) null))

                .ForMember(dest => dest.Id, expression => expression.MapFrom(
                    src => src.MotorsportDriver.LegacyDriverId))

                .ForMember(dest => dest.FirstName, expression => expression.MapFrom(
                    src => src.MotorsportDriver.FirstName))
                
                .ForMember(dest => dest.LastName, expression => expression.MapFrom(
                    src => src.MotorsportDriver.LastName))

                .ForMember(dest => dest.Initials, expression => expression.MapFrom(
                    src => src.MotorsportDriver.FirstName.Substring(0, 1)))

                .ForMember(dest => dest.Abbreviation, expression => expression.UseValue(""))

                .ForMember(dest => dest.FullName, expression => expression.MapFrom(
                    src => src.MotorsportDriver.FullNameCmsOverride ?? src.MotorsportDriver.FirstName + " " + src.MotorsportDriver.LastName))

                .ForMember(dest => dest.CarNumber, expression => expression.MapFrom(
                    src => src.MotorsportDriver.CarNumber))

                .ForMember(dest => dest.Country, expression => expression.MapFrom(
                    src => src.MotorsportDriver.CountryName))

                .ForMember(dest => dest.TeamId, expression => expression.MapFrom(
                    src => src.MotorsportTeam.LegacyTeamId))

                .ForMember(dest => dest.TeamName, expression => expression.MapFrom(
                    src => src.MotorsportTeam.NameCmsOverride ?? src.MotorsportTeam.Name))

                .ForMember(dest => dest.TeamShortName, expression => expression.MapFrom(
                    src => src.MotorsportTeam.NameCmsOverride ?? src.MotorsportTeam.Name))

                .ForMember(dest => dest.TeamCompetition, expression => expression.UseValue((string) null))

                .ForMember(dest => dest.TeamCompetitionId, expression => expression.UseValue(0))

                .ForAllOtherMembers(m => m.Ignore());
        }

        private static string GetTime(MotorsportRaceEventResult grid)
        {
            var hours = grid.FinishingTimeHours == 0 ? "" : grid.FinishingTimeHours + ":";
            var minutes = grid.FinishingTimeMinutes == 0 ? "" : grid.FinishingTimeMinutes + ":";
            var seconds = grid.FinishingTimeSeconds == 0 ? "" : grid.FinishingTimeSeconds + ".";
            var milliseconds = grid.FinishingTimeMilliseconds == 0 ? "" : grid.FinishingTimeMilliseconds.ToString();

            return hours + minutes + seconds + milliseconds;
        }
    }
}