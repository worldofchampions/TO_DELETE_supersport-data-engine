namespace SuperSportDataEngine.Gateway.Http.StatsProzone.Services
{
    using System.Net;
    public interface IProviderWebRequest
    {
        WebRequest GetRequestForDrivers(string providerSlug, int? seasonId = null);
        WebRequest GetRequestForTeams(string providerSlug);
        WebRequest GetRequestForStandings(string providerSlug, string standingsTypeId, int providerSeasonId);
        WebRequest GetRequestForTournaments();
        WebRequest GetRequestForRaces(string providerSlug);
        WebRequest GetRequestForSchedule(string providerSlug, int providerSeasonId);
        WebRequest GetRequestForOwners(string providerSlug);
        WebRequest GetRequestForRaceResults(string providerSlug, int providerSeasonId, int providerRaceId);
    }
}