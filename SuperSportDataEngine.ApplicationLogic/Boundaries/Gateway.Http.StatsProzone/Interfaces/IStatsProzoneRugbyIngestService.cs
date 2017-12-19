namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Interfaces
{
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.ResponseModels;
    using System.Threading;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
    using System.Threading.Tasks;

    public interface IStatsProzoneRugbyIngestService
    {
        Task<RugbyEntitiesResponse> IngestRugbyReferenceData(CancellationToken cancellationToken);
        Task<RugbyFixturesResponse> IngestFixturesForTournament(RugbyTournament activeTournaments, int seasonId, CancellationToken cancellationToken);
        Task<RugbySeasonResponse> IngestSeasonData(CancellationToken cancellationToken, int tournamentId, int tournamentYear);
        Task<RugbyFixturesResponse> IngestFixturesForTournamentSeason(int tournamentId, int seasonId, CancellationToken cancellationToken);
        Task<RugbyMatchStatsResponse> IngestMatchStatsForFixtureAsync(CancellationToken cancellationToken, long providerFixtureId);
        Task<RugbyFlatLogsResponse> IngestFlatLogsForTournament(int competitionId, int seasonId);
        Task<RugbyGroupedLogsResponse> IngestGroupedLogsForTournament(int competitionId, int seasonId);
        Task<RugbyEventsFlowResponse> IngestEventsFlow(CancellationToken cancellationToken, long providerFixtureId);
    }
}
