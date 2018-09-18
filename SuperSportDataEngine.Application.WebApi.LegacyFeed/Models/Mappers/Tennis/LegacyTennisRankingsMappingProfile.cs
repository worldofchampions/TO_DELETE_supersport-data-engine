using System;
using AutoMapper;
using SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Tennis;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models.Tennis;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Mappers.Tennis
{
    public class LegacyTennisRankingsMappingProfile : Profile
    {
        public LegacyTennisRankingsMappingProfile()
        {
            CreateMap<TennisRanking, TennisRankings>()

                .ForMember(dest => dest.PersonId, expression => expression.MapFrom(
                    src => src.TennisPlayer.LegacyPlayerId))

                .ForMember(dest => dest.Surname, expression => expression.MapFrom(
                    src => src.TennisPlayer.LastNameCmsOverride ?? src.TennisPlayer.LastName))

                .ForMember(dest => dest.Name, expression => expression.MapFrom(
                    src => src.TennisPlayer.FirstNameCmsOverride ?? src.TennisPlayer.FirstName))

                .ForMember(dest => dest.Rank, expression => expression.MapFrom(
                    src => src.Rank))

                .ForMember(dest => dest.Points, expression => expression.MapFrom(
                    src => src.Points))

                .ForMember(dest => dest.Movement, expression => expression.MapFrom(
                    src => src.Movement ?? 0))

                .ForMember(dest => dest.TimeStamp, expression => expression.MapFrom(
                    src => src.RankingValidLastAt.DateTime.Date.ToString("s")))

                .ForMember(dest => dest.LeagueName, expression => expression.MapFrom(
                    src => src.TennisLeague.Abbreviation))

                .ForMember(dest => dest.LeagueURLName, expression => expression.MapFrom(
                    src => src.TennisLeague.Slug))

                .ForAllOtherMembers(m => m.Ignore());
        }
    }
}