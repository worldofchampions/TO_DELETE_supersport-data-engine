namespace SuperSportDataEngine.ApplicationLogic.Services.LegacyFeed
{
    using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces.LegacyFeed;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.UnitOfWork;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
    using SuperSportDataEngine.ApplicationLogic.Entities.Legacy.Motorsport;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class MotorsportLegacyFeedService : IMotorsportLegacyFeedService
    {
        private readonly IPublicSportDataUnitOfWork _publicSportDataUnitOfWork;

        public MotorsportLegacyFeedService(IPublicSportDataUnitOfWork publicSportDataUnitOfWork)
        {
            _publicSportDataUnitOfWork = publicSportDataUnitOfWork;
        }

        public async Task<MotorsportScheduleEntity> GetSchedules(string category, bool current)
        {
            var currentSeason = await GetCurrentSeasonForCategory(category);

            var schedule = new MotorsportScheduleEntity();

            if (currentSeason == null)
            {
                return schedule;
            }

            var today = DateTime.UtcNow;

            var raceEvents =
                _publicSportDataUnitOfWork.MotorsportRaceEvents.Where(e =>
                    e.MotorsportSeason.ProviderSeasonId == currentSeason.ProviderSeasonId &&
                    e.MotorsportSeason.MotorsportLeague.Slug.Equals(category))
                  .OrderBy(e => e.StartDateTimeUtc)
                  .ToList();

            if (current)
            {
                raceEvents =
                    raceEvents.Where(e =>
                        e.StartDateTimeUtc > today).ToList();
            }

            schedule.MotorsportRaceEvents =
                raceEvents;

            return await Task.FromResult(schedule);
        }

        public async Task<MotorsportRaceEventGridEntity> GetGridForRaceEventId(string category, int eventId)
        {
            var raceEvent = _publicSportDataUnitOfWork.MotorsportRaceEvents
                .FirstOrDefault(r =>
                    r.LegacyRaceEventId == eventId &&
                    r.MotorsportSeason.MotorsportLeague.Slug.Equals(category));

            var grid = new MotorsportRaceEventGridEntity
            {
                MotorsportRaceEventGrids = new List<MotorsportRaceEventGrid>(),
                MotorsportRaceEvent = raceEvent
            };

            var group =
                    _publicSportDataUnitOfWork.MotorsportRaceEventGrids
                    .Where(g => g.MotorsportRaceEvent.LegacyRaceEventId == eventId && g.GridPosition != 0)
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

            if (raceEvent == null)
                return grid;

            var group =
                _publicSportDataUnitOfWork.MotorsportRaceEventGrids
                    .Where(g => g.MotorsportRaceEvent.LegacyRaceEventId == raceEvent.LegacyRaceEventId && g.GridPosition != 0)
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
                    .Where(g => g.MotorsportRaceEvent.LegacyRaceEventId == eventId && g.Position != 0)
                    .OrderByDescending(g => g.MotorsportRaceEvent.StartDateTimeUtc)
                    .GroupBy(g => g.MotorsportRaceEvent.Id)
                    .FirstOrDefault();

            if (group != null)
                results.MotorsportRaceEventResults =
                    group.ToList().OrderBy(g => g.Position).ToList();

            return await Task.FromResult(results);
        }

        public async Task<MotorsportRaceEventResultsEntity> GetResultsForRaceEventId(string category, int eventId, bool completedEventsOnly)
        {
            var raceEvent =
                _publicSportDataUnitOfWork.MotorsportRaceEvents.FirstOrDefault(r =>
                    r.LegacyRaceEventId == eventId &&
                    r.MotorsportSeason.MotorsportLeague.Slug.Equals(category));

            var results = new MotorsportRaceEventResultsEntity
            {
                MotorsportRaceEventResults = new List<MotorsportRaceEventResult>(),
                MotorsportRaceEvent = raceEvent
            };

            if (raceEvent == null || (completedEventsOnly && raceEvent.MotorsportRaceEventStatus != MotorsportRaceEventStatus.Result))
            {
                return await Task.FromResult(results);
            }

            var group =
                _publicSportDataUnitOfWork.MotorsportRaceEventResults
                    .Where(g => g.MotorsportRaceEvent.LegacyRaceEventId == eventId && g.Position != 0)
                    .OrderByDescending(g => g.MotorsportRaceEvent.StartDateTimeUtc)
                    .GroupBy(g => g.MotorsportRaceEvent.Id)
                    .FirstOrDefault();

            if (group != null)
                results.MotorsportRaceEventResults = group.ToList().OrderBy(g => g.Position).ToList();

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

            if (raceEvent == null)
                return results;

            var group =
                _publicSportDataUnitOfWork.MotorsportRaceEventResults
                    .Where(g => g.MotorsportRaceEvent.LegacyRaceEventId == raceEvent.LegacyRaceEventId && g.Position != 0)
                    .OrderByDescending(g => g.MotorsportRaceEvent.StartDateTimeUtc)
                    .GroupBy(g => g.MotorsportRaceEvent.Id)
                    .FirstOrDefault();

            if (group != null)
                results.MotorsportRaceEventResults =
                    group.ToList()
                        .OrderBy(g => g.Position)
                        .ToList();

            return await Task.FromResult(results);
        }

        public async Task<MotorsportRaceEventResultsEntity> GetLatestResult(string category, bool completedEventsOnly)
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

            if (raceEvent == null || completedEventsOnly && raceEvent.MotorsportRaceEventStatus != MotorsportRaceEventStatus.Result)
            {
                return await Task.FromResult(results);
            }

            var group =
                _publicSportDataUnitOfWork.MotorsportRaceEventResults
                    .Where(g => g.MotorsportRaceEvent.LegacyRaceEventId == raceEvent.LegacyRaceEventId && g.Position != 0)
                    .OrderByDescending(g => g.MotorsportRaceEvent.StartDateTimeUtc)
                    .GroupBy(g => g.MotorsportRaceEvent.Id)
                    .FirstOrDefault();

            if (group != null)
                results.MotorsportRaceEventResults =
                    group.ToList()
                        .OrderBy(g => g.Position)
                        .ToList();

            return await Task.FromResult(results);
        }

        public async Task<MotorsportDriverStandingsEntity> GetDriverStandings(string category)
        {
            var motorsportSeason = await GetCurrentSeasonForCategory(category);
            if (motorsportSeason == null)
                return null;

            var motorsportLeague = motorsportSeason.MotorsportLeague;

            var motorsportDriverStandings = (await _publicSportDataUnitOfWork.MotorsportDriverStandings.WhereAsync(x =>
                x.MotorsportLeagueId.Equals(motorsportLeague.Id) &&
                x.MotorsportSeasonId.Equals(motorsportSeason.Id)
            )).OrderBy(x => x.Position);

            var result = new MotorsportDriverStandingsEntity
            {
                MotorsportLeague = motorsportLeague,
                MotorsportDriverStandings = motorsportDriverStandings.ToList()
            };

            return result;
        }

        public async Task<MotorsportTeamStandingsEntity> GetTeamStandings(string category)
        {
            var motorsportSeason = await GetCurrentSeasonForCategory(category);
            if (motorsportSeason == null)
                return null;

            var motorsportLeague = motorsportSeason.MotorsportLeague;

            var motorsportTeamStandings = (await _publicSportDataUnitOfWork.MotorsportTeamStandings.WhereAsync(x =>
                x.MotorsportLeagueId.Equals(motorsportLeague.Id) &&
                x.MotorsportSeasonId.Equals(motorsportSeason.Id)
                )).OrderBy(x => x.Position);

            var result = new MotorsportTeamStandingsEntity
            {
                MotorsportLeague = motorsportLeague,
                MotorsportTeamStandings = motorsportTeamStandings.ToList()
            };

            return result;
        }

        private async Task<MotorsportSeason> GetCurrentSeasonForCategory(string category)
        {
            return await Task.FromResult(_publicSportDataUnitOfWork.MotorsportSeasons.FirstOrDefault(s =>
               s.MotorsportLeague.Slug.Equals(category) &&
               s.IsCurrent));
        }
    }
}
