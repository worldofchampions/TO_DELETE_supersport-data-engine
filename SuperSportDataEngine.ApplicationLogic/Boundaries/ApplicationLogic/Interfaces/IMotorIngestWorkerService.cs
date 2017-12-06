namespace SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IMotorIngestWorkerService
    {
        Task IngestDriversForActiveTournaments(CancellationToken cancellationToken);
        Task IngestTeamsForActiveTournaments(CancellationToken cancellationToken);
        Task IngestDriverStandingsForActiveTournaments(CancellationToken cancellationToken);
        Task IngestTeamStandingsForActiveTournaments(CancellationToken cancellationToken);
        Task IngestTournaments(CancellationToken cancellationToken);
        Task IngestRacesForActiveTournaments(CancellationToken cancellationToken);
        Task IngestSchedulesForActiveTournaments(CancellationToken cancellationToken);
        Task IngestTournamentResults(string providerSlug, int providerSeasonId, int providerRaceId, CancellationToken cancellationToken);
        Task IngestTournamentGrid(string providerSlug, int providerSeasonId, int providerRaceId, CancellationToken cancellationToken);
    }
}