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

    public class MotorsportService : IMotorsportService
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
    }
}
