namespace SuperSportDataEngine.ApplicationLogic.Services
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IRugbyIngestWorkerService
    {
        Task IngestRugbyReferenceData(CancellationToken cancellationToken);
        void IngestFixturesForActiveTournaments(CancellationToken cancellationToken);
        void IngestLogsForActiveTournaments(CancellationToken cancellationToken);
        void IngestRugbyResultsForAllFixtures(CancellationToken cancellationToken);
    }
}
