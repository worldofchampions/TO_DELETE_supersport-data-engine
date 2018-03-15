namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Mappers
{
    using AutoMapper;
    using SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Shared;
    using SuperSportDataEngine.ApplicationLogic.Entities.Legacy;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;

    
    // This class is used by Reflection.
    public class LegacyCommentaryAsEventMapperProfile : Profile
    {
        public LegacyCommentaryAsEventMapperProfile()
        {
            CreateMap<LegacyRugbyMatchEventEntity, MatchEvent>()
                .IncludeBase<RugbyMatchEvent, MatchEvent>()

                .ForMember(dest => dest.Comments, exp => exp.MapFrom(src => src.Comments));
        }
    }
}