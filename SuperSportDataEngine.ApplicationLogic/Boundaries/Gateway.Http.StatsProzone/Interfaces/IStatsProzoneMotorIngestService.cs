namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Interfaces
{
    using Models.Motor;

    public interface IStatsProzoneMotorIngestService
    {
        MotorEntitiesResponse IngestTournamentDrivers(string providerSlug);
        MotorEntitiesResponse IngestTournamentTeams(string providerSlug);
        MotorEntitiesResponse IngestDriverStandings(string providerSlug);
        MotorEntitiesResponse IngestTeamStandings(string providerSlug);
        MotorEntitiesResponse IngestTournaments();
        MotorEntitiesResponse IngestTournamentRaces(string providerSlug);
        MotorEntitiesResponse IngestTournamentSchedule(string providerSlug, int providerSeasonId);
        MotorEntitiesResponse IngestTournamentResults(string providerSlug, int providerSeasonId, int providerRaceId);
    }
}