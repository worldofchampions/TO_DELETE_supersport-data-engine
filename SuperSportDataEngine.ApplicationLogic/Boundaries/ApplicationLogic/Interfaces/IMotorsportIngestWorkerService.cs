namespace SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces
{
    using System.Threading;
    using System.Threading.Tasks;
    using Gateway.Http.StatsProzone.Models.RequestModels;

    public interface IMotorsportIngestWorkerService
    {
        Task IngestDriversForActiveLeagues(CancellationToken cancellationToken);
        Task IngestTeamsForActiveLeagues(CancellationToken cancellationToken);
        Task IngestOwnersForActiveLeagues(CancellationToken cancellationToken);
        Task IngestDriverStandingsForActiveLeagues(CancellationToken cancellationToken);
        Task IngestTeamStandingsForActiveLeagues(CancellationToken cancellationToken);
        Task IngestRacesForActiveLeagues(CancellationToken cancellationToken);
        Task IngestRaceResults(MotorResultRequestParams requestParams, CancellationToken cancellationToken);
        Task IngestRaceGrid(MotorResultRequestParams requestParams, CancellationToken cancellationToken);
        Task IngestLeagues(CancellationToken cancellationToken);
    }
}