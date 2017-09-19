namespace SuperSportDataEngine.ApplicationLogic.Services
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IRugbyIngestWorkerService
    {
        Task IngestReferenceData(CancellationToken cancellationToken);
        Task IngestFixturesForActiveTournaments(CancellationToken cancellationToken);
        Task IngestLogsForActiveTournaments(CancellationToken cancellationToken);
        Task IngestResultsForAllFixtures(CancellationToken cancellationToken);
        Task IngestResultsForCurrentDayFixtures(CancellationToken cancellationToken);
        Task IngestResultsForFixturesInResultsState(CancellationToken cancellationToken);
        Task IngestFixturesForTournamentSeason(CancellationToken cancellationToken, int tournamentId, int seasonId);
        Task IngestLogsForCurrentTournaments(CancellationToken cancellationToken);
        Task IngestOneMonthsFixturesForTournament(CancellationToken cancellationToken, int providerTournamentId);
        Task IngestLogsForTournamentSeason(CancellationToken cancellationToken, int providerTournamentId, int seasonId);
    }
}
