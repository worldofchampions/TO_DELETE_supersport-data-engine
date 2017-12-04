namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Interfaces
{
    using Models.Motor;

    public interface IStatsProzoneMotorIngestService
    {
        MotorEntitiesResponse IngestTournamentDrivers(string tournamentName);
        MotorEntitiesResponse IngestDriverStandings(string tournamentName);
        MotorEntitiesResponse IngestTeamStandings(string tournamentName);
    }
}
