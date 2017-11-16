namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Interfaces
{
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.ResponseModels;
    using System.Threading;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
    using System.Threading.Tasks;

    public interface IStatsProzoneRugbyIngestService
    {
        RugbyEntitiesResponse IngestRugbyReferenceData(CancellationToken cancellationToken);
        RugbyFixturesResponse IngestFixturesForTournament(RugbyTournament activeTournaments, int seasonId, CancellationToken cancellationToken);
        RugbySeasonResponse IngestSeasonData(CancellationToken cancellationToken, int tournamentId, int tournamentYear);
        RugbyFixturesResponse IngestFixturesForTournamentSeason(int tournamentId, int seasonId, CancellationToken cancellationToken);
        Task<RugbyMatchStatsResponse> IngestMatchStatsForFixtureAsync(CancellationToken cancellationToken, long providerFixtureId);
        RugbyFlatLogsResponse IngestFlatLogsForTournament(int competitionId, int seasonId);
        RugbyGroupedLogsResponse IngestGroupedLogsForTournament(int competitionId, int seasonId);
        Task<RugbyEventsFlowResponse> IngestEventsFlow(CancellationToken cancellationToken, long providerFixtureId);
    }
}
