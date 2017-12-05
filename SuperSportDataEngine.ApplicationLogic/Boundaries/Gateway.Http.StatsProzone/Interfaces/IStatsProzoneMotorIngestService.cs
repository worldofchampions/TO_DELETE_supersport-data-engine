namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Interfaces
{
    using Models.Motor;

    public interface IStatsProzoneMotorIngestService
    {
        MotorEntitiesResponse IngestTournamentDrivers(string providerTournamentSlug);
        MotorEntitiesResponse IngestTournamentTeams(string providerTournamentSlug);
        MotorEntitiesResponse IngestDriverStandings(string providerTournamentSlug);
        MotorEntitiesResponse IngestTeamStandings(string providerTournamentSlug);
        MotorEntitiesResponse IngestTournaments();
        MotorEntitiesResponse IngestTournamentRaces(string providerTournamentSlug);
    }
}