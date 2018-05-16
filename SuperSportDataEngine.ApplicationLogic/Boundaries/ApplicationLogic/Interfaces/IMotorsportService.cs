namespace SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models.Enums;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;

    public interface IMotorsportService
    {
        Task<IEnumerable<MotorsportLeague>> GetActiveLeagues();
        Task<int> GetProviderSeasonIdForLeague(Guid leagueId, CancellationToken cancellationToken);
        Task<SchedulerStateForManagerJobPolling> GetSchedulerStateForManagerJobPolling(Guid leagueId);
        Task<IEnumerable<MotorsportRace>> GetRacesForLeague(Guid leagueId);
        Task<MotorsportSeason> GetCurrentSeasonForLeague(Guid leagueId, CancellationToken cancellationToken);
        Task<IEnumerable<MotorsportSeason>> GetHistoricSeasonsForLeague(Guid leagueId, bool includeCurrentSeason = false);
        Task<MotorsportRaceEvent> GetLiveEventForLeague(Guid leagueId);
        Task<IEnumerable<MotorsportRaceEvent>> GetEndedRaceEventsForLeague(Guid leagueId);
        Task<IEnumerable<MotorsportRaceEvent>> GetEventsForRace(Guid raceId, Guid seasonId);
        Task<IEnumerable<MotorsportSeason>> GetCurrentAndFutureSeasonsForLeague(Guid leagueId);
        Task SetCurrentRaceEvents();
        Task<SchedulerTrackingMotorsportRaceEvent> GetSchedulerTrackingEvent(MotorsportRaceEvent raceEvent);
        Task<IEnumerable<MotorsportRaceEvent>> GetPreLiveEventsForActiveLeagues(int numberOfHoursBeforeEventStarts);
    }
}