namespace SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces
{
    using System.Threading;
    using System.Threading.Tasks;
    using Gateway.Http.StatsProzone.Models.RequestModels;

    public interface IMotorIngestWorkerService
    {
        Task IngestDriversForActiveTournaments(MotorDriverRequestEntity motorDriverRequestEntity, CancellationToken none);
        Task IngestTeamsForActiveTournaments(CancellationToken cancellationToken);
        Task IngestOwnersForActiveTournaments(CancellationToken cancellationToken);
        Task IngestDriverStandingsForActiveTournaments(CancellationToken cancellationToken);
        Task IngestTeamStandingsForActiveTournaments(CancellationToken cancellationToken);
        Task IngestTournaments(CancellationToken cancellationToken);
        Task IngestRacesForActiveTournaments(CancellationToken cancellationToken);
        Task IngestSchedulesForActiveTournaments(CancellationToken cancellationToken);
        Task IngestTournamentResults(MotorResultRequestParams requestParams, CancellationToken cancellationToken);
        Task IngestTournamentGrid(MotorResultRequestParams requestParams, CancellationToken cancellationToken);
    }
}