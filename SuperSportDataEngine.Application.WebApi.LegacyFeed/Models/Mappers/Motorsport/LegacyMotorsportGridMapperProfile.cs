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
    public class LegacyMotorsportGridMapperProfile : Profile
    {
        public LegacyMotorsportGridMapperProfile()
        {
            CreateMap<MotorsportRaceEventGrid, GridModel>()
                .ForMember(dest => dest.StartInPit, expression => expression.UseValue(false))
                .ForMember(dest => dest.Position, expression => expression.MapFrom(
                    src => src.GridPosition))
                .ForMember(dest => dest.PositionText, expression => expression.UseValue((string) null))
                .ForMember(dest => dest.Time, expression => expression.MapFrom(
                    src => GetTime(src)))
                .ForAllOtherMembers(m => m.Ignore());
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