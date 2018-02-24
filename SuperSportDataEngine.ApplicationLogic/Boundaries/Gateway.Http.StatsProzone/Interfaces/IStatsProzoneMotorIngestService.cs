namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Interfaces
{
    using Models.Motor;
    public interface IStatsProzoneMotorIngestService
    {
        MotorEntitiesResponse IngestLeagues();
        MotorEntitiesResponse IngestDriversForLeague(string providerSlug, int providerSeasonId);
        MotorEntitiesResponse IngestTeamsForLeague(string providerSlug);
        MotorEntitiesResponse IngestDriverStandings(string providerSlug, int providerSeasonId);
        MotorEntitiesResponse IngestTeamStandings(string providerSlug, int providerSeasonId);
        MotorEntitiesResponse IngestTournamentRaces(string providerSlug);
        MotorEntitiesResponse IngestLeagueCalendar(string providerSlug, int providerSeasonId);
        MotorEntitiesResponse IngestRaceResults(string providerSlug, int providerSeasonId, int providerRaceId);
        MotorEntitiesResponse IngestRaceGrid(string providerSlug, int providerSeasonId, int providerRaceId);
    }
}