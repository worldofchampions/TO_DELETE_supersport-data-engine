namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Interfaces
{
    using Models.Motor;
    public interface IStatsMotorsportIngestService
    {
        MotorsportEntitiesResponse IngestLeagues();
        MotorsportEntitiesResponse IngestLeagueSeason(string providerSlug);
        MotorsportEntitiesResponse IngestDriversForLeague(string providerSlug, int? providerDriverId = null);
        MotorsportEntitiesResponse IngestTeamsForLeague(string providerSlug);
        MotorsportEntitiesResponse IngestDriverStandings(string providerSlug, int providerSeasonId);
        MotorsportEntitiesResponse IngestTeamStandings(string providerSlug, int providerSeasonId);
        MotorsportEntitiesResponse IngestLeagueRaces(string providerSlug, int providerSeasonId);
        MotorsportEntitiesResponse IngestRaceEventsForLeague(string providerSlug, int providerSeasonId, int providerRaceId);
        MotorsportEntitiesResponse IngestRaceResults(string providerSlug, int providerSeasonId, int providerRaceId);
        MotorsportEntitiesResponse IngestRaceGrid(string providerSlug, int providerSeasonId, int providerRaceId);
    }
}