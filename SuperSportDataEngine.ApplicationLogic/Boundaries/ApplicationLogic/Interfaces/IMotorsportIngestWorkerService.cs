namespace SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces
{
    using System.Threading;
    using System.Threading.Tasks;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;

    public interface IMotorsportIngestWorkerService
    {
        Task IngestLeagues(CancellationToken cancellationToken);
        Task IngestSeasons(CancellationToken cancellationToken);
        Task IngestDriversForActiveLeagues(CancellationToken cancellationToken);
        Task IngestTeamsForActiveLeagues(CancellationToken cancellationToken);
        Task IngestDriverStandingsForRecentlyEndedRaces(int numberOfDaysEnded);
        Task IngestTeamStandingsForRecentlyEndedRaces(int numberOfDaysEnded);
        Task IngestRacesForActiveLeagues(CancellationToken cancellationToken);
        Task IngestRacesEvents(CancellationToken cancellationToken);
        Task IngestResultsForRecentlyEndendRaces(int numberOfDaysEnded);
        Task IngestRacesEventGridForRecentlyEndedRaces(int numberOfDaysEnded);
        Task IngestHistoricRaces(CancellationToken cancellationToken);
        Task IngestHistoricRaceEvents(CancellationToken cancellationToken);
        Task IngestHistoricEventsGrids(CancellationToken cancellationToken);
        Task IngestHistoricEventsResults(CancellationToken cancellationToken);
        Task IngestHistoricDriverStandings(CancellationToken cancellationToken);
        Task IngestHistoricTeamStandings(CancellationToken cancellationToken);
        Task IngestLiveRaceEventData(MotorsportRaceEvent raceEvent, int threadSleepInSeconds, CancellationToken cancellationToken);
        Task IngestTeamStandingsForLeague(MotorsportRaceEvent motorsportRaceEvent, int threadSleepInSeconds, int pollingDurationInMinutes);
        Task IngestDriverStandingsForLeague(MotorsportRaceEvent motorsportRaceEvent, int threadSleepInSeconds, int pollingDurationInMinutes);
        Task IngestResultsForRaceEvent(MotorsportRaceEvent motorsportRaceEvent, int threadSleepInSeconds, int pollingDurationInMinutes);
        Task IngestRaceEventsForLeague(MotorsportRaceEvent motorsportRaceEvent, int threadSleepInSeconds, int pollingDurationInMinutes);
        Task IngestRaceEventGrids(MotorsportRaceEvent motorsportRaceEvent, int ingestSleepInSeconds, int pollingDurationInMinutes);
    }
}