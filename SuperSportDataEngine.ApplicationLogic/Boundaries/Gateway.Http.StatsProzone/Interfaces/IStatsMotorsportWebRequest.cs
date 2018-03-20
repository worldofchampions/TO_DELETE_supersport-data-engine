namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Interfaces
{
    using System.Net;

    public interface IStatsMotorsportWebRequest
    {
        WebRequest GetRequestForLeagues();
        WebRequest GetRequestForSeasons(string providerSlug);
        WebRequest GetRequestForDrivers(string providerSlug, int? providerDriverId = null);
        WebRequest GetRequestForTeams(string providerSlug);
        WebRequest GetRequestForOwners(string providerSlug);
        WebRequest GetRequestForRaces(string providerSlug, int providerSeasonId);
        WebRequest GetRequestForRaceEvents(string providerSlug, int providerSeasonId, int providerRaceId);
        WebRequest GetRequestForRaceGrid(string providerSlug, int providerSeasonId, int providerRaceId);
        WebRequest GetRequestForRaceResults(string providerSlug, int providerSeasonId, int providerRaceId);
        WebRequest GetRequestForStandings(string providerSlug, string standingsTypeId, int providerSeasonId);
    }
}