using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.Stats.Models.Tennis;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.Stats.Models.Tennis.TennisEventsResponse;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.Stats.Models.Tennis.TennisLeaguesResponse;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.Stats.Models.Tennis.TennisLeagueTournamentsResponse;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.Stats.Models.Tennis.TennisParticipantsResponse;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.Stats.Models.Tennis.TennisRankingsResponse;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.Stats.Models.Tennis.TennisResultsResponse;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.Stats.Models.Tennis.TennisSurfaceTypesResponse;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.Stats.Models.Tennis.TennisTournamentEventResponse;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.Stats.Models.Tennis.TennisVenuesResponse;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Interfaces
{
    public interface IStatsTennisIngestService
    {
        TennisLeaguesResponse GetLeagues();
        TennisLeagueTournamentsResponse GetTournamentsForLeague(string leagueProviderSlug);
        TennisSeasonsResponse GetSeasonForLeague(string leagueProviderName);
        TennisTournamentEventResponse GetCalendarEventForTournament(string leagueProviderName, int providerTournamentId, int seasonId);
        TennisVenuesResponse GetVenuesForLeague(string leagueProviderSlug);
        TennisSurfaceTypesResponse GetSurfaceTypes(string leagueProviderSlug);
        TennisParticipantsResponse GetParticipants(string leagueProviderSlug);
        TennisEventsResponse GetLeagueEvents(string providerSlug, int year);
        TennisRankingsResponse GetRankRankings(string providerSlug, int year);
        TennisRankingsResponse GetRaceRankings(string providerSlug, int year);
        TennisResultsResponse GetResultsForEvent(string providerSlug, int eventId, int providerSeasonId);
        TennisResultsResponse GetResultsForMatch(string providerSlug, int providerEventId, int providerMatchId, int seasonProviderSeasonId);
    }
}
