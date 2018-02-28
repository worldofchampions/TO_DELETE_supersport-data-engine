using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IMotorsportIngestWorkerService
    {
        Task IngestLeagues(CancellationToken cancellationToken);
        Task IngestSeasons(CancellationToken cancellationToken);
        Task IngestDriversForActiveLeagues(CancellationToken cancellationToken);
        Task IngestTeamsForActiveLeagues(CancellationToken cancellationToken);
        Task IngestDriverStandingsForActiveLeagues(CancellationToken cancellationToken);
        Task IngestTeamStandingsForActiveLeagues(CancellationToken cancellationToken);
        Task IngestRacesForActiveLeagues(CancellationToken cancellationToken);
        Task IngestResultsForActiveLeagues(CancellationToken cancellationToken);
        Task IngestLeagueCalendarForPastSeasons(CancellationToken cancellationToken);
        Task IngestCalendarsForActiveLeagues(CancellationToken cancellationToken);
        Task IngestRaceGridsForActiveLeagues(CancellationToken cancellationToken);
        Task IngestHistoricRaces(CancellationToken cancellationToken);
        Task IngestHistoricGrids(CancellationToken cancellationToken);
        Task IngestHistoricResults(CancellationToken cancellationToken);
        Task IngestHistoricTeamStandings(CancellationToken cancellationToken);
        Task IngestHistoricDriverStandings(CancellationToken cancellationToken);
        Task IngestLiveRaceData(MotorsportRace race, CancellationToken cancellationToken);
    }
}