namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Mappers
{
    using AutoMapper;
    using SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Shared;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;

    public class LegacyMatchEventsMapperProfile : Profile
    {
        public LegacyMatchEventsMapperProfile()
        {
            CreateMap<RugbyMatchEvent, MatchEvent>()

                // For Player 1 Details
                .ForMember(dest => dest.Player1Id, exp => exp.MapFrom(src => src.RugbyPlayer1.LegacyPlayerId))

                .ForMember(dest => dest.Player1DisplayName, exp => exp.MapFrom(
                    src => src.RugbyPlayer1 == null ? LegacyFeedConstants.EmptyPlayerName :
                    (src.RugbyPlayer1.DisplayNameCmsOverride ?? src.RugbyPlayer1.FullName ?? src.RugbyPlayer1.FirstName ?? LegacyFeedConstants.EmptyPlayerName)))

                .ForMember(dest => dest.Player1Surname, exp => exp.MapFrom(
                       src => src.RugbyPlayer1 == null ? LegacyFeedConstants.EmptyPlayerName :
                       (src.RugbyPlayer1.LastName ?? LegacyFeedConstants.EmptyPlayerName)))

                .ForMember(dest => dest.Player1FullName, exp => exp.MapFrom(
                    src => src.RugbyPlayer1 == null ? LegacyFeedConstants.EmptyPlayerName :
                    (src.RugbyPlayer1.FullName ?? LegacyFeedConstants.EmptyPlayerName)))

                // For Player 2 Details
                .ForMember(dest => dest.Player2Id, exp => exp.MapFrom(src => src.RugbyPlayer2.LegacyPlayerId))

                .ForMember(dest => dest.Player2DisplayName, exp => exp.MapFrom(
                    src => src.RugbyPlayer2 == null ? LegacyFeedConstants.EmptyPlayerName :
                    (src.RugbyPlayer2.DisplayNameCmsOverride ?? src.RugbyPlayer2.FullName ?? src.RugbyPlayer1.FirstName ?? LegacyFeedConstants.EmptyPlayerName)))

                .ForMember(dest => dest.Player2Surname, exp => exp.MapFrom(
                    src => src.RugbyPlayer2 == null ? LegacyFeedConstants.EmptyPlayerName :
                    (src.RugbyPlayer2.LastName ?? LegacyFeedConstants.EmptyPlayerName)))

                .ForMember(dest => dest.Player2FullName, exp => exp.MapFrom(
                    src => src.RugbyPlayer2 == null ? LegacyFeedConstants.EmptyPlayerName :
                    (src.RugbyPlayer2.FullName ?? LegacyFeedConstants.EmptyPlayerName)))

                // Match Related Details
                .ForMember(dest => dest.MatchId, exp => exp.MapFrom(src => src.RugbyFixture.LegacyFixtureId))

                .ForMember(dest => dest.DisplayTime, exp => exp.MapFrom(src => src.GameTimeInMinutes))

                .ForMember(dest => dest.Time, exp => exp.MapFrom(src => src.GameTimeInMinutes))

                .ForMember(dest => dest.TeamId, exp => exp.MapFrom(src => src.RugbyTeam.LegacyTeamId))

                .ForMember(dest => dest.EventName, exp => exp.MapFrom(
                    src => src.RugbyEventType == null ? LegacyFeedConstants.EmptyEventName :
                    (src.RugbyEventType.EventName ?? LegacyFeedConstants.EmptyEventName)))

                .ForMember(dest => dest.Comments, exp => exp.UseValue(LegacyFeedConstants.EmptyEventComment))

                .ForMember(dest => dest.EventId, exp => exp.MapFrom(src => src.RugbyEventType.EventCode))

                .ForAllOtherMembers(dest => dest.Ignore());
        }
    }
}