
using System;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.Stats.Models.Tennis;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.Stats.Models.Tennis.TennisEventsResponse;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.Stats.Models.Tennis.TennisLeaguesResponse;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.Stats.Models.Tennis.TennisRankingsResponse;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.Stats.Models.Tennis.TennisResultsResponse;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.Stats.Models.Tennis.TennisSurfaceTypesResponse;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.Stats.Models.Tennis.TennisTournamentEventResponse;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.Stats.Models.Tennis.TennisVenuesResponse;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models.Enums;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models.Tennis;
using System.Collections.Generic;
using System.Threading.Tasks;
using MatchType = SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.Stats.Models.Tennis.TennisResultsResponse.MatchType;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces
{
    public interface ITennisStorageService
    {
        IEnumerable<TennisLeague> GetEnabledLeagues();

        TennisLeague GetTennisLeagueFor(int providerLeagueId);
        TennisLeague GetTennisLeagueFor(string providerLeagueSlug);
        TennisSeason GetCurrentTennisSeasonForLeague(string providerLeagueSlug);

        TennisTournament GetTennisTournamentFor(int providerTournamentId);

        Task AddOrUpdateTennisTournamentsFromProvider(
            List<Gateway.Http.Stats.Models.Tennis.TennisLeagueTournamentsResponse.Tournament> tournaments,
            Boundaries.Gateway.Http.Stats.Models.Tennis.TennisLeagueTournamentsResponse.League providerTennisLeague,
            TennisLeague dbTennisLeague);

        Task AddOrUpdateTennisParticipants(
            List<Gateway.Http.Stats.Models.Tennis.TennisParticipantsResponse.Player> participants);

        Task AddOrUpdateEventsFromProvider(TennisEventsResponse eventsForLeague, TennisLeague tennisLeague);
        Task AddOrUpdateTennisLeaguesFromProvider(Gateway.Http.Stats.Models.Tennis.TennisLeaguesResponse.ApiResult leagues);
        Task AddOrUpdateSeasonsFromProvider(TennisSeasonsResponse seasonsResponse, TennisLeague tennisLeague);
        Task AddOrUpdateTennisVenuesFromProvider(TennisVenuesResponse venuesFromProvider);
        Task AddOrUpdateSurfaceTypesFromProvider(TennisSurfaceTypesResponse surfaceTypesFromProvider);
        Task SetTennisTournamentTypes();
        Task AddOrUpdateRanking(TennisRankingType rankingType, List<Ranking> leaderboard, TennisLeague league, TennisSeason season, DateTimeOffset dateThroughDateTime);
        IEnumerable<TennisEvent> GetEventsForLeague(int providerLeagueId, int providerSeasonId);
        TennisEventTennisLeagues GetMappingForTennisEventTennisLeague(int providerEventId, int providerLeagueId);
        Task AddOrUpdateTennisEventSeeds(IEnumerable<Seed> seeds, TennisEvent dbTennisEvent);
        Task AddOrUpdateTennisMatch(List<Match> matches, MatchType matchType, TennisEventTennisLeagues associatedTennisEventTennisLeague);
        TennisMatchStatus GetMatchStateFromProviderState(int matchStatusMatchStatusId);

        TennisEvent GetTennisEventFor(int providerEventId);
        TennisMatch GetTennisMatchFor(int providerMatchId);
    }
}
