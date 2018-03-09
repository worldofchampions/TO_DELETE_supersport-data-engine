using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
                .ForAllOtherMembers(m => m.Ignore());
        }
    }
}