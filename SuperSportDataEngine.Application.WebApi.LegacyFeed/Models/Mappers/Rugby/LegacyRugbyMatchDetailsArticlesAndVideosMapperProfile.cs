using SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Rugby;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Mappers.Rugby
{
    using AutoMapper;
    using Shared;
    using ApplicationLogic.Boundaries.Gateway.Http.DeprecatedFeed.ResponseModels;
    using ApplicationLogic.Entities.Legacy;

    public class LegacyRugbyMatchDetailsArticlesAndVideosMapperProfile : Profile
    {
        public LegacyRugbyMatchDetailsArticlesAndVideosMapperProfile()
        {
            CreateMap<LiveVideosResponse, MatchLiveVideo>();

            CreateMap<HighlightVideosResponse, MatchVideo>();

            CreateMap<DeprecatedArticlesAndVideosEntity, RugbyMatchDetails>()
                .ForMember(dest => dest.MatchDayBlogId, exp => exp.MapFrom(src => src.MatchDayBlogId))
                .ForMember(dest => dest.PreviewId, exp => exp.MapFrom(src => src.MatchPreviewId))
                .ForMember(dest => dest.ReportId, exp => exp.MapFrom(src => src.MatchReportId))
                .ForMember(dest => dest.LiveVideos, exp => exp.MapFrom(src => src.LiveVideosResponse))
                .ForMember(dest => dest.Videos, exp => exp.MapFrom(src => src.HighlightVideosResponse))
                .ForAllOtherMembers(dest => dest.Ignore());
        }
    }
}
