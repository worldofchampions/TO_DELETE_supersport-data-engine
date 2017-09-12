namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Interfaces
{
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.ResponseModels;
    using System.Threading;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models;

    public interface IStatsProzoneRugbyIngestService
    {
        RugbyEntitiesResponse IngestRugbyReferenceData(CancellationToken cancellationToken);
        RugbyFixturesResponse IngestFixturesForTournament(SportTournament activeTournaments, CancellationToken cancellationToken);
        RugbyLogsResponse IngestLogsForTournament(SportTournament activeTournaments, CancellationToken cancellationToken);
    }
}
