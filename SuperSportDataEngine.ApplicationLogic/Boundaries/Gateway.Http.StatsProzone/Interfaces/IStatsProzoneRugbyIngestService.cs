namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Interfaces
{
    using ResponseModels;
    using System.Threading;
    using Repository.EntityFramework.PublicSportData.Models;
    using System.Threading.Tasks;

    public interface IStatsProzoneRugbyIngestService
    {
        Task<RugbyEntitiesResponse> IngestRugbyReferenceData(CancellationToken cancellationToken);
        Task<RugbyFixturesResponse> IngestFixturesForTournament(RugbyTournament activeTournaments, int seasonId, CancellationToken cancellationToken);
        Task<RugbySeasonResponse> IngestSeasonData(CancellationToken cancellationToken, int tournamentId, int tournamentYear);
        Task<RugbyFixturesResponse> IngestFixturesForTournamentSeason(int tournamentId, int seasonId, CancellationToken cancellationToken);
        Task<RugbyMatchStatsResponse> IngestMatchStatsForFixtureAsync(CancellationToken cancellationToken, long providerFixtureId);
        Task<RugbyFlatLogsResponse> IngestFlatLogsForTournament(int competitionId, int seasonId, int roundNumber);
        Task<RugbyGroupedLogsResponse> IngestGroupedLogsForTournament(int competitionId, int seasonId, int numberOfRounds);
        Task<RugbyEventsFlowResponse> IngestEventsFlow(CancellationToken cancellationToken, long providerFixtureId);
    }
}
