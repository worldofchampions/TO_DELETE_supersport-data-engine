using System.Net;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.Stats.Interfaces
{
    public interface IStatsTennisProviderWebRequest
    {
        WebRequest GetRequestForLeagues();
        WebRequest GetWebRequestForTournamentDecode(string leagueName);
        WebRequest GetWebRequestForSeasonsDecode(string leagueProviderSlug);
        WebRequest GetWebRequestForVenuesDecode(string leagueProviderSlug);
        WebRequest GetWebRequestForSurfaceTypesDecode(string leagueProviderSlug);
        WebRequest GetWebRequestForParticipants(string leagueProviderSlug);
        WebRequest GetWebRequestForTournamentEvent(string leagueProviderSlug, int providerTournamentId, int seasonId);
        WebRequest GetWebRequestForEvents(string leagueProviderSlug, int year);
        WebRequest GetWebRequestForRankings(string leagueProviderSlug, int rankingType, string providerRankingQuery, int year);
        WebRequest GetWebRequestForResults(string leagueProviderSlug, int eventId, int providerSeasonId);
        WebRequest GetWebRequestForResults(string leagueProviderSlug, int eventId, int matchId, int providerSeasonId);
    }
}