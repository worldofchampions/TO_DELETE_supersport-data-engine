using AutoMapper;
using SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Rugby;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Mappers
{
    // This class is used by Reflection.
    public class LegacyMatchStatsMapperProfile: Profile
    {
        public LegacyMatchStatsMapperProfile()
        {
            CreateMap<RugbyMatchStatistics, RugbyMatchStatisticsModel>()

                .ForMember(src => src.CarriesCrossedGainLine, exp => exp.MapFrom(
                   dest => dest.CarriesCrossedGainLine))

                .ForMember(src => src.YellowCards, exp => exp.MapFrom(
                   dest => dest.YellowCards))

                .ForMember(src => src.RedCards, exp => exp.MapFrom(
                   dest => dest.RedCards))

               .ForMember(src => src.Tries, exp => exp.MapFrom(
                   dest => dest.Tries))

                .ForMember(src => src.Conversions, exp => exp.MapFrom(
                   dest => dest.Conversions))

               .ForMember(src => src.Penalties, exp => exp.MapFrom(
                   dest => dest.Penalties))

                .ForMember(src => src.PenaltiesMissed, exp => exp.MapFrom(
                   dest => dest.PenaltiesMissed))

                .ForMember(src => src.DropGoals, exp => exp.MapFrom(
                   dest => dest.DropGoals))

                .ForMember(src => src.DropGoalsMissed, exp => exp.MapFrom(
                   dest => dest.DropGoalsMissed))

                 .ForMember(src => src.ConversionAttempts, exp => exp.MapFrom(
                   dest => dest.ConversionAttempts))

                .ForMember(src => src.PenaltyAttempts, exp => exp.MapFrom(
                   dest => dest.PenaltyAttempts))

                .ForMember(src => src.DropGoalAttempts, exp => exp.MapFrom(
                   dest => dest.DropGoalAttempts))

                .ForMember(src => src.DefendersBeaten, exp => exp.MapFrom(
                   dest => dest.DefendersBeaten))

                .ForMember(src => src.Offloads, exp => exp.MapFrom(
                   dest => dest.Offloads))

                .ForMember(src => src.LineOutsWon, exp => exp.MapFrom(
                   dest => dest.LineOutsWon))

                .ForMember(src => src.LineOutsLost, exp => exp.MapFrom(
                   dest => dest.LineOutsLost))

                .ForMember(src => src.PenaltiesConceded, exp => exp.MapFrom(
                   dest => dest.PenaltiesConceded))

                .ForMember(src => src.Tackles, exp => exp.MapFrom(
                   dest => dest.Tackles))

                .ForMember(src => src.Territory, exp => exp.MapFrom(
                   dest => dest.Territory))

                .ForMember(src => src.Passes, exp => exp.MapFrom(
                   dest => dest.Passes))

                .ForMember(src => src.CleanBreaks, exp => exp.MapFrom(
                   dest => dest.CleanBreaks))

                 .ForMember(src => src.ScrumsWon, exp => exp.MapFrom(
                   dest => dest.ScrumsWon))

                 .ForMember(src => src.MissedTackles, exp => exp.MapFrom(
                   dest => dest.TacklesMissed))

                 .ForMember(src => src.Possession, exp => exp.MapFrom(
                   dest => dest.Possession))

                 .ForMember(src => src.ScrumsLost, exp => exp.MapFrom(
                   dest => dest.ScrumsLost))

                 .ForMember(src => src.PenaltyTries, exp => exp.MapFrom(
                   dest => dest.PenaltyTries))

                .ForAllOtherMembers(dest => dest.Ignore());
        }
    }
}