using AutoMapper;
using SuperSportDataEngine.ApplicationLogic.Entities.Legacy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Mappers
{
    public class LegacyScorerModelMapperProfile: Profile
    {
        public LegacyScorerModelMapperProfile()
        {
            CreateMap<LegacyRugbyScorerEntity, Scorer>()

                .ForMember(dest => dest.CombinedName, exp => exp.MapFrom(src => src.CombinedName))

                .ForMember(dest => dest.DisplayName, exp => exp.MapFrom(src => src.DisplayName))

                .ForMember(dest => dest.EventId, exp => exp.MapFrom(src => src.EventId))

                .ForMember(dest => dest.Name, exp => exp.MapFrom(src => src.Name))
                
                .ForMember(dest => dest.NickName, exp => exp.MapFrom(src => src.NickName))

                .ForMember(dest => dest.PersonId, exp => exp.MapFrom(src => src.PersonId))

                .ForMember(dest => dest.Surname, exp => exp.MapFrom(src => src.Surname))

                .ForMember(dest => dest.Time, exp => exp.MapFrom(src => src.Time))

                .ForMember(dest => dest.Type, exp => exp.MapFrom(src => src.Type))

                .ForAllOtherMembers(dest => dest.Ignore());
        }
    }
}