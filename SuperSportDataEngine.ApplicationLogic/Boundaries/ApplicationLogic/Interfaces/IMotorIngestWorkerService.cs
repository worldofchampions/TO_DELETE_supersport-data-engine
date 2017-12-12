namespace SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces
{
    using System.Threading;
    using System.Threading.Tasks;
    using Gateway.Http.StatsProzone.Models.RequestModels;
    public interface IMotorIngestWorkerService
    {
        Task IngestDriversForActiveTournaments(CancellationToken cancellationToken);
        Task IngestTeamsForActiveTournaments(CancellationToken cancellationToken);
        Task IngestDriverStandingsForActiveTournaments(CancellationToken cancellationToken);
        Task IngestTeamStandingsForActiveTournaments(CancellationToken cancellationToken);
        Task IngestTournaments(CancellationToken cancellationToken);
        Task IngestRacesForActiveTournaments(CancellationToken cancellationToken);
        Task IngestSchedulesForActiveTournaments(CancellationToken cancellationToken);
        Task IngestTournamentResults(MotorResultRequestEntity motorResultRequestEntity, CancellationToken cancellationToken);
        Task IngestTournamentGrid(MotorResultRequestEntity motorResultRequestEntity, CancellationToken cancellationToken);
    }
}