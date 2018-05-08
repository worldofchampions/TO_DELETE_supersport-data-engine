namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Mappers.Motorsport
{
    using AutoMapper;
    using SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Shared;
    using SuperSportDataEngine.ApplicationLogic.Entities.Legacy;

    public class LegacyMotorsportScheduleArticlesAndVideosMapperProfile : Profile
    {
        public LegacyMotorsportScheduleArticlesAndVideosMapperProfile()
        {
            CreateMap<DeprecatedArticlesAndVideosEntity, Races>()
                .ForMember(dest => dest.LiveVideos, exp => exp.MapFrom(src => src.LiveVideosResponse))
                .ForMember(dest => dest.Videos, exp => exp.MapFrom(src => src.HighlightVideosResponse))
                .ForAllOtherMembers(dest => dest.Ignore());
        }
    }
}
