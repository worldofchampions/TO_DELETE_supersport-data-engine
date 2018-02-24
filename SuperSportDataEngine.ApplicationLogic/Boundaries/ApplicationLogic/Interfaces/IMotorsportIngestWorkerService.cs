namespace SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IMotorsportIngestWorkerService
    {
        Task IngestLeagues(CancellationToken cancellationToken);
        Task IngestSeasons(CancellationToken cancellationToken);
        Task IngestDriversForActiveLeagues(CancellationToken cancellationToken);
        Task IngestTeamsForActiveLeagues(CancellationToken cancellationToken);
        Task IngestDriverStandingsForActiveLeagues(CancellationToken cancellationToken);
        Task IngestTeamStandingsForActiveLeagues(CancellationToken cancellationToken);
        Task IngestRacesForActiveLeagues(CancellationToken cancellationToken);
        Task IngestRaceGridsForPastSeasons(CancellationToken cancellationToken);
        Task IngestRaceResults(CancellationToken cancellationToken);
    }
}