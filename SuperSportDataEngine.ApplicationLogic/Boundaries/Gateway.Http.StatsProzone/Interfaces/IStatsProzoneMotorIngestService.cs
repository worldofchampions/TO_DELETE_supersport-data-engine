namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Interfaces
{
    using Models.Motor;
    using Models.RequestModels;
    public interface IStatsProzoneMotorIngestService
    {
        MotorEntitiesResponse IngestLeagueDrivers(string providerSlug, int providerSeasonId);
        MotorEntitiesResponse IngestTournamentTeams(string providerSlug);
        MotorEntitiesResponse IngestTournamentOwners(string providerSlug);
        MotorEntitiesResponse IngestDriverStandings(string providerSlug);
        MotorEntitiesResponse IngestTeamStandings(string providerSlug);
        MotorEntitiesResponse IngestLeagues();
        MotorEntitiesResponse IngestTournamentRaces(string providerSlug);
        MotorEntitiesResponse IngestLeagueCalendar(string providerSlug, int providerSeasonId);
        MotorEntitiesResponse IngestRaceResults(string providerSlug, int providerSeasonId, int providerRaceId);
        MotorEntitiesResponse IngestLeagueGrid(string providerSlug, int providerSeasonId);
    }
}