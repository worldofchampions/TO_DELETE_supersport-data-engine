namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Mappers
{
    using AutoMapper;
    using SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Shared;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
    
    // This class is used by Reflection.
    public class LegacyCommentaryMapperProfile : Profile
    {
        public LegacyCommentaryMapperProfile()
        {
            CreateMap<RugbyCommentary, MatchEvent>()

                .ForMember(dest => dest.Comments, exp => exp.MapFrom(
                    src => src.CommentaryText))

                .ForMember(dest => dest.DisplayTime, exp => exp.MapFrom(
                    src => src.GameTimeRawMinutes))

                .ForMember(dest => dest.Time, exp => exp.MapFrom(
                    src => src.GameTimeRawMinutes))

                .ForMember(dest => dest.MatchId, exp => exp.MapFrom(
                    src => src.RugbyFixture.LegacyFixtureId))

                .ForMember(dest => dest.EventId, src => src.UseValue(LegacyFeedConstants.CommentaryEventId))

                .ForAllOtherMembers(dest => dest.Ignore());
        }
    }
}