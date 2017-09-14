namespace SuperSportDataEngine.ApplicationLogic.Services
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IRugbyIngestWorkerService
    {
        Task IngestRugbyReferenceData(CancellationToken cancellationToken);
        Task IngestFixturesForActiveTournaments(CancellationToken cancellationToken);
        Task IngestLogsForActiveTournaments(CancellationToken cancellationToken);
        Task IngestRugbyResultsForAllFixtures(CancellationToken cancellationToken);
        Task IngestRugbyResultsForCurrentDayFixtures(CancellationToken cancellationToken);
        Task IngestRugbyResultsForFixturesInResultsState(CancellationToken cancellationToken);
        Task IngestFixturesForTournamentSeason(CancellationToken cancellationToken, int tournamentId, int seasonId);
        Task IngestLogsForCurrentTournaments(CancellationToken cancellationToken);
    }
}
