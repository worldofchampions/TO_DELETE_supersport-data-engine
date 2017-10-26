using AutoMapper;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Mappers
{
    public class LegacyMatchEventsMapperProfile : Profile
    {
        public LegacyMatchEventsMapperProfile()
        {
            CreateMap<RugbyMatchEvent, MatchEvent>()

                // For Player 1 Details
                .ForMember(dest => dest.Player1Id, exp => exp.MapFrom(src => src.RugbyPlayer1.LegacyPlayerId))

                .ForMember(dest => dest.Player1DisplayName, exp => exp.MapFrom(
                    src => src.RugbyPlayer1.FirstName ?? src.RugbyPlayer1.FullName ?? LegacyFeedConstants.EmptyPlayerName))

                .ForMember(dest => dest.Player1Surname, exp => exp.MapFrom(
                       src => src.RugbyPlayer1.LastName ?? LegacyFeedConstants.EmptyPlayerName))

                .ForMember(dest => dest.Player1FullName, exp => exp.MapFrom(
                    src => src.RugbyPlayer1.FullName ?? LegacyFeedConstants.EmptyPlayerName))

                // For Player 2 Details
                .ForMember(dest => dest.Player2Id, exp => exp.MapFrom(src => src.RugbyPlayer2.LegacyPlayerId))

                .ForMember(dest => dest.Player2DisplayName, exp => exp.MapFrom(
                    src => src.RugbyPlayer2.FirstName ?? src.RugbyPlayer2.FullName ?? LegacyFeedConstants.EmptyPlayerName))

                .ForMember(dest => dest.Player2Surname, exp => exp.MapFrom(
                    src => src.RugbyPlayer2.LastName ?? LegacyFeedConstants.EmptyPlayerName))

                .ForMember(dest => dest.Player2FullName, exp => exp.MapFrom(
                    src => src.RugbyPlayer2.FullName ?? LegacyFeedConstants.EmptyPlayerName))

                // Match Related Details
                .ForMember(dest => dest.MatchId, exp => exp.MapFrom(src => src.RugbyFixture.LegacyFixtureId))

                .ForMember(dest => dest.DisplayTime, exp => exp.MapFrom(src => src.GameTimeInMinutes))

                .ForMember(dest => dest.Time, exp => exp.MapFrom(src => src.GameTimeInMinutes))

                .ForMember(dest => dest.TeamId, exp => exp.MapFrom(src => src.RugbyTeam.LegacyTeamId))

                .ForMember(dest => dest.EventName, exp => exp.MapFrom(
                    src => src.RugbyEventType.EventName ?? LegacyFeedConstants.EmptyEventName))
                
                .ForMember(dest => dest.Comments, exp => exp.UseValue(LegacyFeedConstants.EmptyEventComment))

                .ForMember(dest => dest.EventId, exp => exp.MapFrom(src => src.RugbyEventType.EventCode))

                .ForAllOtherMembers(dest => dest.Ignore());
        }
    }
}