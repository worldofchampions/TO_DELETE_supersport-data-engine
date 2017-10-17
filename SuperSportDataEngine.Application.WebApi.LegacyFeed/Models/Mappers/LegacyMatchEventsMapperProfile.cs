using AutoMapper;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Mappers
{
    public class LegacyMatchEventsMapperProfile: Profile
    {
        public LegacyMatchEventsMapperProfile()
        {
            CreateMap<RugbyMatchEvent, MatchEvent>()

                // For Player 1 Details
                .ForMember(dest => dest.Player1DisplayName, exp => exp.MapFrom(src => src.RugbyPlayer1.FirstName))

                .ForMember(dest => dest.Player1Id, exp => exp.MapFrom(src => src.RugbyPlayer1.LegacyPlayerId))

                .ForMember(dest => dest.Player1Surname, exp => exp.MapFrom(src => src.RugbyPlayer1.LastName))

                .ForMember(dest => dest.Player1FullName, exp => exp.MapFrom( src => src.RugbyPlayer1.FullName))

                // For Player 2 Details
                .ForMember(dest => dest.Player2DisplayName, exp => exp.MapFrom(src => src.RugbyPlayer2.FirstName))

                .ForMember(dest => dest.Player2Id, exp => exp.MapFrom(src => src.RugbyPlayer2.LegacyPlayerId))

                .ForMember(dest => dest.Player2Surname, exp => exp.MapFrom(src => src.RugbyPlayer2.LastName))

                .ForMember(dest => dest.Player2FullName, exp => exp.MapFrom(src => src.RugbyPlayer2.FullName))

                // Match Related Details
                .ForMember(dest => dest.MatchId, exp => exp.MapFrom(src => src.RugbyFixture.LegacyFixtureId))

                .ForMember(dest => dest.DisplayTime, exp => exp.MapFrom(src => src.GameTimeInMinutes))

                .ForMember(dest => dest.Time, exp => exp.MapFrom(src => src.GameTimeInMinutes))

                .ForMember(dest => dest.EventName, exp => exp.MapFrom(src => src.RugbyEventType.EventName))

                .ForMember(dest => dest.TeamId, exp => exp.MapFrom(src => src.RugbyTeam.LegacyTeamId))

                .ForAllOtherMembers(dest => dest.Ignore());
        }
    }
}