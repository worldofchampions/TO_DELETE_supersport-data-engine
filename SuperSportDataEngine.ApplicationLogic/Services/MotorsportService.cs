using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.UnitOfWork;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models.Enums;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.UnitOfWork;

namespace SuperSportDataEngine.ApplicationLogic.Services
{
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

        public async Task<IEnumerable<MotorsportRace>> GetLeagueRacesByProviderSeasonId(Guid leagueId, int providerSeasonId)
        {
            //TODO 
            var races = new List<MotorsportRace>();

            return await Task.FromResult(races);
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

        public async Task<MotorsportSeason> GetPastSeasonsForLeague(Guid leagueId, CancellationToken cancellationToken)
        {
            var pastSeasonProviderId = DateTime.UtcNow.Year - 1;

            var league = _publicSportDataUnitOfWork.MotorsportLeagues.FirstOrDefault(l => l.Id == leagueId);

            var pastSeason = _publicSportDataUnitOfWork.MotorsportSeasons.FirstOrDefault(s =>
                s.ProviderSeasonId == pastSeasonProviderId);

            if (pastSeason != null) return pastSeason;
            
            //TODO: Seed past seasons since provider does not expose such data
            var tempSeason = new MotorsportSeason
            {
                DataProvider = DataProvider.Stats,
                IsActive = false,
                ProviderSeasonId = pastSeasonProviderId,
                MotorsportLeague = league,
                Name = "Seed Season"
            };

            _publicSportDataUnitOfWork.MotorsportSeasons.Add(tempSeason);

            await _publicSportDataUnitOfWork.SaveChangesAsync();

            pastSeason = _publicSportDataUnitOfWork.MotorsportSeasons.FirstOrDefault(s =>
                s.ProviderSeasonId == pastSeasonProviderId);

            return pastSeason;
        }

        public async Task<MotorsportRaceEvent> GetTodayEventForRace(Guid raceId)
        {
            var raceEvent = _publicSportDataUnitOfWork.MotorsportRaceEvents.FirstOrDefault(r =>
                r.MotorsportRace.Id == raceId);

            return await Task.FromResult(raceEvent);
        }

        public async Task<IEnumerable<MotorsportRaceEvent>> GetEventsForRace(Guid raceId, Guid seasonId)
        {
            var raceEvents = _publicSportDataUnitOfWork.MotorsportRaceEvents.Where(
                e => e.MotorsportRace.Id == raceId && e.MotorsportSeason.Id == seasonId);

            return await Task.FromResult(raceEvents);
        }
    }
}
