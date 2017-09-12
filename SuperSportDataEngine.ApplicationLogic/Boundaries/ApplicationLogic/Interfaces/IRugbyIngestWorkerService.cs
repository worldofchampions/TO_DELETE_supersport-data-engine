namespace SuperSportDataEngine.ApplicationLogic.Services
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IRugbyIngestWorkerService
    {
        Task IngestRugbyReferenceData(CancellationToken cancellationToken);
        Task IngestFixturesForActiveTournaments(CancellationToken cancellationToken);
        Task IngestLogsForActiveTournaments(CancellationToken cancellationToken);
    }
}
