using System.Net;

namespace SuperSportDataEngine.Gateway.Http.StatsProzone.Services
{
    public interface IProviderWebRequest
    {
        WebRequest GetRequestForDrivers(string providerSlug);
        WebRequest GetRequestForTeams(string providerSlug);
        WebRequest GetRequestForStandings(string providerSlug, string standingsTypeId);
        WebRequest GetRequestForTournaments();
        WebRequest GetRequestForRaces(string providerSlug);
        WebRequest GetRequestForSchedule(string providerSlug, int providerSeasonId);
        WebRequest GetRequestRaceResults(string providerSlug, int providerSeasonId, int providerRaceId);
    }
}