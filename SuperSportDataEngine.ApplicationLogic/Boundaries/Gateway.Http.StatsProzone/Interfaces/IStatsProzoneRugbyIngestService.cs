namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Interfaces
{
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.ResponseModels;
    using System.Threading;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
    using System.Threading.Tasks;

    public interface IStatsProzoneRugbyIngestService
    {
        RugbyEntitiesResponse IngestRugbyReferenceData(CancellationToken cancellationToken);
        RugbyFixturesResponse IngestFixturesForTournament(RugbyTournament activeTournaments, CancellationToken cancellationToken);
        RugbyLogsResponse IngestLogsForTournament(RugbyTournament activeTournaments, CancellationToken cancellationToken);
        RugbySeasonResponse IngestSeasonData(CancellationToken cancellationToken, int tournamentId, int tournamentYear);
        // TODO: Ingest
        RugbyFixturesResponse IngestFixturesForTournamentSeason(int tournamentId, int seasonId, CancellationToken cancellationToken);
        Task<RugbyFixturesResponse> IngestFixtureResults(int competionId, int seasonId, int roundId);
        Task<RugbyMatchStatsResponse> IngestMatchStatsForFixtureAsync(CancellationToken cancellationToken, long providerFixtureId);
    }
}
