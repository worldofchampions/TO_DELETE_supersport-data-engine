using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.UnitOfWork;
using SuperSportDataEngine.ApplicationLogic.Entities.Legacy.Motorsport;

namespace SuperSportDataEngine.ApplicationLogic.Services.LegacyFeed
{
    using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces.LegacyFeed;

    // TODO: @motorsport-feed: implement.
    public class MotorsportLegacyFeedService : IMotorsportLegacyFeedService
    {
        private readonly IPublicSportDataUnitOfWork _publicSportDataUnitOfWork;

        public MotorsportLegacyFeedService(IPublicSportDataUnitOfWork publicSportDataUnitOfWork)
        {
            _publicSportDataUnitOfWork = publicSportDataUnitOfWork;
        }

        public async Task<MotorsportScheduleEntity> GetSchedules(string category, bool current)
        {
            var currentSeason =
                _publicSportDataUnitOfWork.MotorsportSeasons.FirstOrDefault(s =>
                    s.MotorsportLeague.Slug.Equals(category) &&
                    s.IsCurrent);

            var schedule = new MotorsportScheduleEntity();

            if (currentSeason == null)
            {
                return schedule;
            }

            var today = DateTime.UtcNow;

            var raceEvents =
                _publicSportDataUnitOfWork.MotorsportRaceEvents.Where(e =>
                    e.MotorsportSeason.ProviderSeasonId == currentSeason.ProviderSeasonId).ToList();

            if (current)
            {
                raceEvents = 
                    raceEvents.Where(e => 
                        e.StartDateTimeUtc > today).ToList();
            }

            schedule.MotorsportRaceEvents = raceEvents;

            return await Task.FromResult(schedule);
        }

        
        public async Task<MotorsportRaceEventGridEntity> GetGridForRaceEventId(string category, int eventId)
        {
            var raceEvent = _publicSportDataUnitOfWork.MotorsportRaceEvents
                .FirstOrDefault(r =>
                    r.LegacyRaceEventId == eventId &&
                    r.MotorsportSeason.MotorsportLeague.Slug.Equals(category));

            var grid = new MotorsportRaceEventGridEntity()
            {
                MotorsportRaceEventGrids = new List<MotorsportRaceEventGrid>(),
                MotorsportRaceEvent = raceEvent
            };

            var group =
                    _publicSportDataUnitOfWork.MotorsportRaceEventGrids
                    .Where(g => g.MotorsportRaceEvent.LegacyRaceEventId == eventId)
                    .OrderByDescending(g => g.MotorsportRaceEvent.StartDateTimeUtc)
                    .GroupBy(g => g.MotorsportRaceEvent.Id)
                    .FirstOrDefault();

            if (group != null)
                grid.MotorsportRaceEventGrids = 
                    group.ToList().OrderBy(g => g.GridPosition).ToList();
            
            return await Task.FromResult(grid);
        }

        public async Task<MotorsportRaceEventGridEntity> GetLatestGrid(string category)
        {
            var raceEvent = _publicSportDataUnitOfWork.MotorsportRaceEvents
                .FirstOrDefault(r =>
                    r.IsCurrent &&
                    r.MotorsportSeason.MotorsportLeague.Slug.Equals(category));

            var grid = new MotorsportRaceEventGridEntity()
            {
                MotorsportRaceEventGrids = new List<MotorsportRaceEventGrid>(),
                MotorsportRaceEvent = raceEvent
            };

            var group =
                _publicSportDataUnitOfWork.MotorsportRaceEventGrids
                    .Where(g => g.MotorsportRaceEvent.LegacyRaceEventId == raceEvent.LegacyRaceEventId)
                    .OrderByDescending(g => g.MotorsportRaceEvent.StartDateTimeUtc)
                    .GroupBy(g => g.MotorsportRaceEvent.Id)
                    .FirstOrDefault();

            if (group != null)
                grid.MotorsportRaceEventGrids =
                    group.ToList()
                        .OrderBy(g => g.GridPosition)
                        .ToList();

            return await Task.FromResult(grid);
        }

        public async Task<MotorsportRaceEventResultsEntity> GetResultsForRaceEventId(string category, int eventId)
        {
            var raceEvent = _publicSportDataUnitOfWork.MotorsportRaceEvents
                .FirstOrDefault(r =>
                    r.LegacyRaceEventId == eventId &&
                    r.MotorsportSeason.MotorsportLeague.Slug.Equals(category));

            var results = new MotorsportRaceEventResultsEntity()
            {
                MotorsportRaceEventResults = new List<MotorsportRaceEventResult>(),
                MotorsportRaceEvent = raceEvent
            };

            var group =
                _publicSportDataUnitOfWork.MotorsportRaceEventResults
                    .Where(g => g.MotorsportRaceEvent.LegacyRaceEventId == eventId)
                    .OrderByDescending(g => g.MotorsportRaceEvent.StartDateTimeUtc)
                    .GroupBy(g => g.MotorsportRaceEvent.Id)
                    .FirstOrDefault();

            if (group != null)
                results.MotorsportRaceEventResults =
                    group.ToList().OrderBy(g => g.GridPosition).ToList();

            return await Task.FromResult(results);
        }

        public async Task<MotorsportRaceEventResultsEntity> GetLatestResult(string category)
        {
            var raceEvent = _publicSportDataUnitOfWork.MotorsportRaceEvents
                .FirstOrDefault(r =>
                    r.IsCurrent &&
                    r.MotorsportSeason.MotorsportLeague.Slug.Equals(category));

            var results = new MotorsportRaceEventResultsEntity()
            {
                MotorsportRaceEventResults = new List<MotorsportRaceEventResult>(),
                MotorsportRaceEvent = raceEvent
            };

            var group =
                _publicSportDataUnitOfWork.MotorsportRaceEventResults
                    .Where(g => g.MotorsportRaceEvent.LegacyRaceEventId == raceEvent.LegacyRaceEventId)
                    .OrderByDescending(g => g.MotorsportRaceEvent.StartDateTimeUtc)
                    .GroupBy(g => g.MotorsportRaceEvent.Id)
                    .FirstOrDefault();

            if (group != null)
                results.MotorsportRaceEventResults =
                    group.ToList()
                        .OrderBy(g => g.GridPosition)
                        .ToList();

            return await Task.FromResult(results);
        }
    }
}
