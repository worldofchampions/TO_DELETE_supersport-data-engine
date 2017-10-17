using AutoMapper;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
using System;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Mappers
{
    public class LegacyCommentaryMapperProfile: Profile
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

                .ForAllOtherMembers(dest => dest.Ignore());
        }
    }
}