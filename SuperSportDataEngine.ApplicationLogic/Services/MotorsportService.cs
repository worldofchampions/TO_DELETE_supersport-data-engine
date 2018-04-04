using System.Collections;

namespace SuperSportDataEngine.ApplicationLogic.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.UnitOfWork;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models.Enums;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.UnitOfWork;

    public class MotorsportService: IMotorsportService
    {
        private readonly IPublicSportDataUnitOfWork _publicSportDataUnitOfWork;
        private readonly ISystemSportDataUnitOfWork _systemSportDataUnitOfWork;

        public MotorsportService(
            IPublicSportDataUnitOfWork publicSportDataUnitOfWork,
            ISystemSportDataUnitOfWork systemSportDataUnitOfWork)
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
            return await  Task.FromResult(SchedulerStateForManagerJobPolling.NotRunning);
        }

        public async Task<IEnumerable<MotorsportRace>> GetRacesForLeague(Guid leagueId)
        {
            var races =  _publicSportDataUnitOfWork.MotorsportRaces.Where(r =>
                r.MotorsportLeague.Id == leagueId).ToList();

            return await Task.FromResult(races);
        }

        public async Task<MotorsportSeason> GetCurrentSeasonForLeague(Guid leagueId, CancellationToken cancellationToken)
        {
            var season =
                 _publicSportDataUnitOfWork.MotorsportSeasons.FirstOrDefault(s =>
                    s.IsCurrent && s.MotorsportLeague.Id == leagueId);

            return await Task.FromResult(season);
        }

        public async Task<IEnumerable<MotorsportSeason>> GetPastSeasonsForLeague(Guid leagueId, CancellationToken cancellationToken)
        {
            var currentYear = DateTime.UtcNow.Year;
            const int numberOfPastSeasons = 2;

            var pastSeasonsForLeague = await _publicSportDataUnitOfWork.MotorsportSeasons.WhereAsync(s =>
                s.ProviderSeasonId >= currentYear - numberOfPastSeasons && s.ProviderSeasonId <= currentYear -1
                && s.MotorsportLeague.Id == leagueId);

            return pastSeasonsForLeague;
        }

        public async Task<MotorsportRaceEvent> GetTodayEventForRace(Guid raceId)
        {
            var raceEvent = _publicSportDataUnitOfWork.MotorsportRaceEvents.FirstOrDefault(r =>
                r.MotorsportRace.Id == raceId);

            return await Task.FromResult(raceEvent);
        }

        public async Task<IEnumerable<MotorsportRaceEvent>> GetEventsForRace(Guid raceId, Guid seasonId)
        {
            var raceEvents = _publicSportDataUnitOfWork.MotorsportRaceEvents.Where(e =>
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

                var previousRaceEvent = raceEvents.FirstOrDefault(e => e.IsCurrent);

                raceEvents.Remove(previousRaceEvent);

                foreach (var raceEvent in raceEvents)
                {
                    if (ShouldSetRaceEventAsCurrent(raceEvent))
                    {
                        raceEvent.IsCurrent = true;
                        _publicSportDataUnitOfWork.MotorsportRaceEvents.Update(raceEvent);

                        if (previousRaceEvent != null)
                        {
                            previousRaceEvent.IsCurrent = false;
                            _publicSportDataUnitOfWork.MotorsportRaceEvents.Update(previousRaceEvent);
                        }

                        break;
                    }
                }
            }
        }

        private IEnumerable<MotorsportRaceEvent> GetRaceEventsForLeague(MotorsportLeague league, MotorsportSeason currentSeason)
        {
            return _publicSportDataUnitOfWork.MotorsportRaceEvents.Where(e =>
                e.MotorsportSeason.Id == currentSeason.Id &&
                e.MotorsportRace.MotorsportLeague.Id == league.Id);
        }

        private static bool ShouldSetRaceEventAsCurrent(MotorsportRaceEvent raceEvent)
        {
            return false;
        }
    }
}
