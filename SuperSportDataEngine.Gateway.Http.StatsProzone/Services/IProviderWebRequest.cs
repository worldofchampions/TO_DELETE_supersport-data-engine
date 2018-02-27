namespace SuperSportDataEngine.Gateway.Http.StatsProzone.Services
{
    using System.Net;
    public interface IProviderWebRequest
    {
        WebRequest GetRequestForDrivers(string providerSlug, int? seasonId = null);
        WebRequest GetRequestForTeams(string providerSlug);
        WebRequest GetRequestForStandings(string providerSlug, string standingsTypeId, int providerSeasonId);
        WebRequest GetRequestForLeagues();
        WebRequest GetRequestForRaces(string providerSlug, int providerSeasonId);
        WebRequest GetRequestForCalendar(string providerSlug, int providerSeasonId, int providerRaceId);
        WebRequest GetRequestForOwners(string providerSlug);
        WebRequest GetRequestForRaceResults(string providerSlug, int providerSeasonId, int providerRaceId);
        WebRequest GetRequestForSeasons(string providerSlug);
        WebRequest GetRequestForRaceGrid(string providerSlug, int providerSeasonId, int providerRaceId);
    }
}