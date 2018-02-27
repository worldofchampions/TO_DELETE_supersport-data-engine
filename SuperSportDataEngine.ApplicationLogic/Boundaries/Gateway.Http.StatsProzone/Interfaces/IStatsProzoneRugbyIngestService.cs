namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Interfaces
{
    using ResponseModels;
    using System.Threading;
    using Repository.EntityFramework.PublicSportData.Models;
    using System.Threading.Tasks;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.ResponseModels.RugbyRoundFixturesResponse;

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
        Task<RugbyPlayerStatsResponse> IngestPlayerStatsForTournament(int providerTournamentId, int providerSeasonId, CancellationToken cancellationToken);
        Task<RugbyRoundFixturesResponse> IngestRoundFixturesForTournament(int providerTournamentId, int providerSeasonId, int roundNumber);
    }
}
