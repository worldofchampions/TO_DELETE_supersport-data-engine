namespace SuperSportDataEngine.ApplicationLogic.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.UnitOfWork;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models.Enums;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.UnitOfWork;

    public class MotorsportService : IMotorsportService
    {
        private readonly IPublicSportDataUnitOfWork _publicSportDataUnitOfWork;
        private readonly ISystemSportDataUnitOfWork _systemSportDataUnitOfWork;

        public MotorsportService(IPublicSportDataUnitOfWork publicSportDataUnitOfWork, ISystemSportDataUnitOfWork systemSportDataUnitOfWork)
        {
            _publicSportDataUnitOfWork = publicSportDataUnitOfWork;
            _systemSportDataUnitOfWork = systemSportDataUnitOfWork;
        }

        public async Task<IEnumerable<MotorsportLeague>> GetActiveLeagues()
        {
            var activeLeagues = await _publicSportDataUnitOfWork.MotorsportLeagues.WhereAsync(l => l.IsEnabled);

            return activeLeagues;
        }

        public async Task<int> GetProviderSeasonIdForLeague(Guid leagueId, CancellationToken cancellationToken)
        {
            var season = _publicSportDataUnitOfWork.MotorsportSeasons.FirstOrDefault(s =>
                s.MotorsportLeague.Id == leagueId);

            return await Task.FromResult(season.ProviderSeasonId);
        }

        public async Task<SchedulerStateForManagerJobPolling> GetSchedulerStateForManagerJobPolling(Guid leagueId)
        {
            return await Task.FromResult(SchedulerStateForManagerJobPolling.NotRunning);
        }

        public async Task<IEnumerable<MotorsportRace>> GetRacesForLeague(Guid leagueId)
        {
            var races =
                _publicSportDataUnitOfWork.MotorsportRaces.Where(r => r.MotorsportLeague.Id == leagueId).ToList();

            return await Task.FromResult(races);
        }

        public async Task<MotorsportSeason> GetCurrentSeasonForLeague(Guid leagueId, CancellationToken cancellationToken)
        {
            var season =
                 _publicSportDataUnitOfWork.MotorsportSeasons.FirstOrDefault(s =>
                    s.IsCurrent && s.MotorsportLeague.Id == leagueId);

            return await Task.FromResult(season);
        }

        public async Task<IEnumerable<MotorsportSeason>> GetCurrentAndFutureSeasonsForLeague(Guid leagueId)
        {
            var currentSeason =
                _publicSportDataUnitOfWork.MotorsportSeasons.FirstOrDefault(s =>
                s.MotorsportLeague.Id == leagueId && s.IsCurrent);

            if (currentSeason == null) return null;

            var result =
                _publicSportDataUnitOfWork.MotorsportSeasons.Where(s =>
                    s.MotorsportLeague.Id == leagueId &&
                    s.ProviderSeasonId >= currentSeason.ProviderSeasonId).ToList();

            return await Task.FromResult(result);
        }

        public async Task<IEnumerable<MotorsportSeason>> GetHistoricSeasonsForLeague(Guid leagueId, bool includeCurrentSeason)
        {
            const int numberOfPastSeasons = 2;

            var currentSeason =
                _publicSportDataUnitOfWork.MotorsportSeasons.FirstOrDefault(s =>
                s.IsCurrent && s.MotorsportLeague.Id == leagueId);

            if (includeCurrentSeason && currentSeason != null)
            {
                var providerCurentSeasonId = currentSeason.ProviderSeasonId;

                return await _publicSportDataUnitOfWork.MotorsportSeasons.WhereAsync(s =>
                    s.ProviderSeasonId >= providerCurentSeasonId - numberOfPastSeasons
                    && s.ProviderSeasonId <= providerCurentSeasonId
                    && s.MotorsportLeague.Id == leagueId);
            }

            return await _publicSportDataUnitOfWork.MotorsportSeasons.WhereAsync(s =>
                s.ProviderSeasonId >= DateTime.UtcNow.Year - numberOfPastSeasons &&
                s.ProviderSeasonId <= DateTime.UtcNow.Year
                && s.MotorsportLeague.Id == leagueId);
        }

        public async Task<MotorsportRaceEvent> GetLiveEventForLeague(Guid leagueId)
        {
            var raceEvent = _publicSportDataUnitOfWork.MotorsportRaceEvents
                .Where(e => e.MotorsportRace.MotorsportLeague.Id == leagueId)
                .FirstOrDefault(IsEventLive);

            return await Task.FromResult(raceEvent);
        }

        public async Task<MotorsportRaceEvent> GetEndedRaceEventForLeague(Guid leagueId)
        {
            var raceEvent =
                _publicSportDataUnitOfWork.MotorsportRaceEvents.FirstOrDefault(e =>
                    e.MotorsportRace.MotorsportLeague.Id == leagueId
                    && e.MotorsportRaceEventStatus == MotorsportRaceEventStatus.Result
                    && e.MotorsportSeason.IsCurrent);

            return await Task.FromResult(raceEvent);
        }

        private static bool IsEventLive(MotorsportRaceEvent raceEvent)
        {
            if (raceEvent.StartDateTimeUtc is null) return false;

            var minutesBeforeEventStarts =
                Math.Round(raceEvent.StartDateTimeUtc.Value.Subtract(DateTime.UtcNow).TotalMinutes, MidpointRounding.AwayFromZero);

            var maxRaceEventMinutes = GetEstimatedRaceEventTimeInMinutes(raceEvent.MotorsportRace.MotorsportLeague.MotorsportSportType);

            const int minRaceEventMinutes = 0;

            return (int)minutesBeforeEventStarts < maxRaceEventMinutes && (int)minutesBeforeEventStarts > minRaceEventMinutes;
        }

        private static int GetEstimatedRaceEventTimeInMinutes(MotorsportSportType motorsportSportType)
        {
            switch (motorsportSportType)
            {
                case MotorsportSportType.FormulaOne:
                    return 135;
                case MotorsportSportType.Superbike:
                case MotorsportSportType.MotoGp:
                default:
                    return 75;
            }
        }

        public async Task<IEnumerable<MotorsportRaceEvent>> GetEventsForRace(Guid raceId, Guid seasonId)
        {
            var raceEvents =
                _publicSportDataUnitOfWork.MotorsportRaceEvents.Where(e =>
                e.MotorsportRace.Id == raceId && e.MotorsportSeason.Id == seasonId).ToList();

            return await Task.FromResult(raceEvents);
        }

        public async Task SetCurrentRaceEvents()
        {
            var motorsportLeagues = await GetActiveLeagues();

            if (motorsportLeagues == null) return;

            foreach (var league in motorsportLeagues)
            {
                var currentSeason = await GetCurrentSeasonForLeague(league.Id, CancellationToken.None);

                var raceEvents = GetRaceEventsForLeague(league, currentSeason).ToList();

                if (league.MotorsportSportType == MotorsportSportType.Superbike)
                {
                    await SetSuperbikeCurrentRaceEvents(raceEvents);
                }
                else
                {
                    await SetOtherLeagueCurrentEvent(raceEvents);
                }
            }
        }

        private async Task SetOtherLeagueCurrentEvent(ICollection<MotorsportRaceEvent> raceEvents)
        {
            var previousRaceEvent = raceEvents.FirstOrDefault(e => e.IsCurrent);

            raceEvents.Remove(previousRaceEvent);

            foreach (var raceEvent in raceEvents)
            {
                if (!ShouldSetRaceEventAsCurrent(raceEvent)) continue;

                if (previousRaceEvent != null)
                {
                    previousRaceEvent.IsCurrent = false;

                    _publicSportDataUnitOfWork.MotorsportRaceEvents.Update(previousRaceEvent);
                }

                raceEvent.IsCurrent = true;

                _publicSportDataUnitOfWork.MotorsportRaceEvents.Update(raceEvent);

                await _publicSportDataUnitOfWork.SaveChangesAsync();

                break;
            }
        }

        private async Task SetSuperbikeCurrentRaceEvents(IReadOnlyCollection<MotorsportRaceEvent> raceEvents)
        {
            var superbikeNextActivePair = GetSuperbikeNextActivePair(raceEvents);

            var superbikePreviousActivePair = GetSuperbikePreviousActivePair(raceEvents);

            var isNextEventsAvailable = superbikeNextActivePair.firstEvent != null && superbikeNextActivePair.secondEvent != null;

            var isPreviousEventsAvailable = superbikePreviousActivePair.firstEvent != null && superbikePreviousActivePair.secondEvent != null;

            if (isNextEventsAvailable)
            {
                superbikeNextActivePair.firstEvent.IsCurrent = true;
                superbikeNextActivePair.secondEvent.IsCurrent = true;

                _publicSportDataUnitOfWork.MotorsportRaceEvents.Update(superbikeNextActivePair.firstEvent);
                _publicSportDataUnitOfWork.MotorsportRaceEvents.Update(superbikeNextActivePair.secondEvent);
            }

            if (isPreviousEventsAvailable && isNextEventsAvailable)
            {
                superbikePreviousActivePair.firstEvent.IsCurrent = false;
                superbikePreviousActivePair.secondEvent.IsCurrent = false;

                _publicSportDataUnitOfWork.MotorsportRaceEvents.Update(superbikePreviousActivePair.firstEvent);
                _publicSportDataUnitOfWork.MotorsportRaceEvents.Update(superbikePreviousActivePair.secondEvent);
            }

            if (isPreviousEventsAvailable || isNextEventsAvailable)
            {
                await _publicSportDataUnitOfWork.SaveChangesAsync();
            }
        }

        private static (MotorsportRaceEvent firstEvent, MotorsportRaceEvent secondEvent) GetSuperbikePreviousActivePair(IEnumerable<MotorsportRaceEvent> raceEvents)
        {
            var currentEvents = raceEvents.Where(e => e.IsCurrent).ToList();

            return currentEvents.Count != 2 ? (null, null) : (currentEvents[0], currentEvents[1]);
        }

        private static (MotorsportRaceEvent firstEvent, MotorsportRaceEvent secondEvent) GetSuperbikeNextActivePair(IEnumerable<MotorsportRaceEvent> raceEvents)
        {
            var nextActiveEvents = raceEvents.Where(ShouldSetRaceEventAsCurrent).ToList();

            return nextActiveEvents.Count != 2 ? (null, null) : (nextActiveEvents[0], nextActiveEvents[1]);
        }

        private IEnumerable<MotorsportRaceEvent> GetRaceEventsForLeague(MotorsportLeague league, MotorsportSeason currentSeason)
        {
            return _publicSportDataUnitOfWork.MotorsportRaceEvents.Where(e =>
                e.MotorsportSeason.Id == currentSeason.Id &&
                e.MotorsportRace.MotorsportLeague.Id == league.Id);
        }

        private static bool ShouldSetRaceEventAsCurrent(MotorsportRaceEvent raceEvent)
        {
            if (raceEvent.StartDateTimeUtc == null) return false;

            var hoursBeforeEventStarts =
                Math.Round(raceEvent.StartDateTimeUtc.Value.Subtract(DateTime.UtcNow).TotalHours, MidpointRounding.AwayFromZero);

            const int hourToSetEventCurrent = 72;

            return (int)hoursBeforeEventStarts < hourToSetEventCurrent && (int)hoursBeforeEventStarts > 0;
        }

        public async Task<SchedulerTrackingMotorsportRaceEvent> GetSchedulerTrackingEvent(MotorsportRaceEvent raceEvent)
        {
            var schedulerTrackingEvent =
                _systemSportDataUnitOfWork.SchedulerTrackingMotorsportRaceEvents.FirstOrDefault(e =>
                    e.MotorsportRaceEventId == raceEvent.Id);

            return await Task.FromResult(schedulerTrackingEvent);
        }
    }
}
