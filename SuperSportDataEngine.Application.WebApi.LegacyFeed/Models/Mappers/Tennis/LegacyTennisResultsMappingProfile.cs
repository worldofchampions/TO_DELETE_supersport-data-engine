using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models.Enums;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models.Tennis;
using SuperSportDataEngine.ApplicationLogic.Entities.Legacy.Tennis;
using WebGrease.Css.Extensions;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Mappers.Tennis
{
    public class TennisSideDto
    {
        public TennisSide TennisSide { get; set; }
        public List<TennisEventSeed> TennisSeeds { get; set; }
        public TennisEvent TennisEvent { get; set; }
        public TennisMatch TennisMatch { get; set; }
    }

    public class TennisPlayerDto
    {
        public TennisPlayer TennisPlayer { get; set; }
        public List<TennisEventSeed> TennisSeeds { get; set; }
        public TennisEvent TennisEvent { get; set; }
    }

    public class TennisSetDto
    {
        public TennisSet TennisSet { get; set; }
        public int MatchId { get; set; }
        public int SideNumber { get; set; }
    }

    public class LegacyTennisResultsMappingProfile : Profile
    {
        private const string FinalResult = "FinalResult";
        private const string Suspended = "Suspended";
        private const string ScoreUpdate = "ScoreUpdate";

        public LegacyTennisResultsMappingProfile()
        {
            CreateMap<TennisMatchEntity, Models.Tennis.TennisMatch>()

                .ForMember(dest => dest.TournamentId, expression => expression.MapFrom(
                    src => src.TennisMatch.AssociatedTennisEventTennisLeague.TennisEvent.LegacyEventId))

                .ForMember(dest => dest.TournamentName, expression => expression.MapFrom(
                    src => src.TennisMatch.AssociatedTennisEventTennisLeague.TennisEvent.EventNameCmsOverride 
                 ?? src.TennisMatch.AssociatedTennisEventTennisLeague.TennisEvent.EventName))

                 .ForMember(dest => dest.Type, expression => expression.MapFrom(
                     src => GetTournamentTypeString(src)))

                 .ForMember(dest => dest.RoundType, expression => expression.MapFrom(
                     src => src.TennisMatch.RoundType))

                .ForMember(dest => dest.Sequence, expression => expression.UseValue(0))

                .ForMember(dest => dest.DrawOrder, expression => expression.MapFrom(
                    src => src.TennisMatch.DrawNumber))

                .ForMember(dest => dest.RoundOrder, expression => expression.MapFrom(
                    src => src.TennisMatch.RoundNumber))

                .ForMember(dest => dest.Id, expression => expression.MapFrom(
                    src => src.TennisMatch.LegacyMatchId))

                .ForMember(dest => dest.StatusText, expression => expression.UseValue(""))

                .ForMember(dest => dest.Started, expression => expression.MapFrom(
                    src => src.TennisMatch.TennisMatchStatus == TennisMatchStatus.InProgress))

                .ForMember(dest => dest.Completed, expression => expression.MapFrom(
                    src => src.TennisMatch.TennisMatchStatus == TennisMatchStatus.Final))

                .ForMember(dest => dest.Date, expression => expression.MapFrom(
                    src => (src.TennisMatch.MakeupDateTimeUtc ?? src.TennisMatch.StartDateTimeUtc).Date.ToString("s")))

                .ForMember(dest => dest.Comments, expression => expression.UseValue(""))

                .ForMember(dest => dest.Status, expression => expression.MapFrom(
                    src => GetStatus(src.TennisMatch.TennisMatchStatus)))

                .ForMember(dest => dest.Court, expression => expression.MapFrom(
                    src => src.TennisMatch.CourtName ?? ""))

                .ForMember(dest => dest.Sets, expression => expression.MapFrom(
                    src => GeNumberOfTennisSets(src)))

                .ForMember(dest => dest.Sides, expression => expression.MapFrom(
                    src => new List<TennisSideDto>
                    {
                        new TennisSideDto { TennisSide = src.TennisMatch.TennisSideA, TennisSeeds = src.TennisSeeds, TennisEvent = src.TennisMatch.AssociatedTennisEventTennisLeague.TennisEvent, TennisMatch = src.TennisMatch },
                        new TennisSideDto { TennisSide = src.TennisMatch.TennisSideB, TennisSeeds = src.TennisSeeds, TennisEvent = src.TennisMatch.AssociatedTennisEventTennisLeague.TennisEvent, TennisMatch = src.TennisMatch }
                    }))

                .ForMember(dest => dest.WinningSide, expression => expression.MapFrom(
                    src => GetWinningSide(src.TennisMatch)))

                .ForAllOtherMembers(m => m.Ignore());

            CreateMap<TennisSideDto, Models.Tennis.TennisSide>()

                .ForMember(dest => dest.Number, expression => expression.MapFrom(
                    src => src.TennisSide.SideNumber))

                .ForMember(dest => dest.Sets, expression => expression.MapFrom(
                    src => GetTennisSets(src)))

                .ForMember(dest => dest.Players, expression => expression.MapFrom(
                    src => new List<TennisPlayerDto>
                    {
                        new TennisPlayerDto { TennisPlayer = src.TennisSide.TennisPlayerA, TennisSeeds = src.TennisSeeds, TennisEvent = src.TennisEvent },
                        new TennisPlayerDto { TennisPlayer = src.TennisSide.TennisPlayerB, TennisSeeds = src.TennisSeeds, TennisEvent = src.TennisEvent },
                    }))

                .ForAllOtherMembers(m => m.Ignore());

            CreateMap<TennisSetDto, Models.Tennis.TennisSet>()
                
                .ForMember(dest => dest.MatchId, expression => expression.MapFrom(
                    src => src.MatchId))

                .ForMember(dest => dest.Side, expression => expression.MapFrom(
                    src => src.SideNumber))

                .ForMember(dest => dest.Number, expression => expression.MapFrom(
                    src => src.TennisSet.SetNumber))

                .ForMember(dest => dest.Games, expression => expression.MapFrom(
                    src => src.SideNumber == 1 ? src.TennisSet.SideAGamesWon : src.TennisSet.SideBGamesWon))

                .ForMember(dest => dest.TieBreak, expression => expression.MapFrom(
                    src => src.SideNumber == 1 ? src.TennisSet.SideATieBreakerPoints ?? -1 : src.TennisSet.SideBTieBreakerPoints ?? -1))

                .ForAllOtherMembers(m => m.Ignore());

            CreateMap<TennisPlayerDto, Models.Tennis.TennisPlayer>()

                .ForMember(dest => dest.PersonId, expression => expression.MapFrom(
                    src => GetPlayerId(src)))

                .ForMember(dest => dest.Country, expression => expression.MapFrom(
                    src => src.TennisPlayer.TennisCountry.CountryAbbreviation))

                .ForMember(dest => dest.Seed, expression => expression.MapFrom(
                    src => GetPlayerSeed(src)))

                .ForMember(dest => dest.Name, expression => expression.MapFrom(
                    src => GetPlayerName(src)))

                .ForMember(dest => dest.Surname, expression => expression.MapFrom(
                    src => GetPlayerSurname(src)))

                .ForMember(dest => dest.NickName, expression => expression.UseValue((string) null))
                .ForMember(dest => dest.DisplayName, expression => expression.UseValue((string) null))

                .ForMember(dest => dest.CombinedName, expression => expression.MapFrom(
                    src => GetCombinedNameForPlayer(src.TennisPlayer)))

                .ForAllOtherMembers(m => m.Ignore());
        }

        private string GetStatus(TennisMatchStatus tennisMatchTennisMatchStatus)
        {
            if (tennisMatchTennisMatchStatus == TennisMatchStatus.Final)
                return FinalResult;

            if (tennisMatchTennisMatchStatus == TennisMatchStatus.InProgress)
                return ScoreUpdate;

            if (tennisMatchTennisMatchStatus == TennisMatchStatus.Suspended)
                return Suspended;

            return "";
        }

        private static string GetCombinedNameForPlayer(TennisPlayer srcTennisPlayer)
        {
            return (srcTennisPlayer?.FirstNameCmsOverride ?? srcTennisPlayer?.FirstName ?? "") + " " +
                   (srcTennisPlayer?.LastNameCmsOverride ?? srcTennisPlayer?.LastName ?? "");
        }

        private static List<TennisSetDto> GetTennisSets(TennisSideDto src)
        {
            var dtoSets = new List<TennisSetDto>();
            src.TennisMatch.TennisSets.ForEach(
                s => dtoSets.Add(
                    new TennisSetDto
                    {
                        TennisSet = s,
                        MatchId = src.TennisMatch.LegacyMatchId,
                        SideNumber = src.TennisSide.SideNumber
                    }));

            return dtoSets;
        }

        private int GeNumberOfTennisSets(TennisMatchEntity src)
        {
            return src.TennisMatch.AssociatedTennisEventTennisLeague.TennisEvent.TennisTournament.TennisTournamentType ==
                TennisTournamentType.GS
                    ? 5
                    : 3;
        }

        private string GetTournamentTypeString(TennisMatchEntity src)
        {
            var leagueGender = src.TennisMatch.AssociatedTennisEventTennisLeague.TennisLeague.Gender;
            switch(leagueGender)
            {
                case TennisGender.Male:
                    return "Men's Singles";
                case TennisGender.Female:
                    return "Women's Singles";
            }

            return "";
        }

        private string GetPlayerName(TennisPlayerDto src)
        {
            return src.TennisPlayer?.FirstNameCmsOverride ?? src.TennisPlayer?.FirstName ?? "";
        }

        private string GetPlayerSurname(TennisPlayerDto src)
        {
            return src.TennisPlayer?.LastNameCmsOverride ?? src.TennisPlayer?.LastName ?? "";
        }

        private int GetPlayerId(TennisPlayerDto src)
        {
            return src.TennisPlayer?.LegacyPlayerId ?? -1;
        }

        private int GetPlayerSeed(TennisPlayerDto src)
        {
            var seed =
                src.TennisSeeds.FirstOrDefault(s =>
                    s.TennisEvent.LegacyEventId == src.TennisEvent.LegacyEventId &&
                    s.TennisPlayer.LegacyPlayerId == src.TennisPlayer?.LegacyPlayerId);

            return seed?.SeedValue ?? 1000;
        }

        private int GetWinningSide(TennisMatch match)
        {
            var sideOneHasWon = match.TennisSideA.HasSideWon;
            var sideTwoHasWon = match.TennisSideB.HasSideWon;

            if (sideOneHasWon)
                return 1;

            if (sideTwoHasWon)
                return 2;

            return 0;
        }
    }
}