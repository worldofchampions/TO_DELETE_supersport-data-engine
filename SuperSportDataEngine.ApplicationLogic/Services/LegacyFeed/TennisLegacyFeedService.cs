using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces.LegacyFeed;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models.Enums;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models.Tennis;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.UnitOfWork;
using SuperSportDataEngine.ApplicationLogic.Entities.Legacy.Tennis;

namespace SuperSportDataEngine.ApplicationLogic.Services.LegacyFeed
{
    public class TennisLegacyFeedService : ITennisLegacyFeedService
    {
        private readonly IPublicSportDataUnitOfWork _publicSportDataUnitOfWork;

        public TennisLegacyFeedService(
            IPublicSportDataUnitOfWork publicSportDataUnitOfWork)
        {
            _publicSportDataUnitOfWork = publicSportDataUnitOfWork;
        }

        public async Task<List<TennisEventEntity>> GetSchedules(string category, bool currentValue)
        {
            var currentSeason = await GetCurrentSeasonForCategory(category);

            var schedule = new List<TennisEventEntity>();

            if (currentSeason == null)
            {
                return schedule;
            }

            var today = DateTime.UtcNow;

            var events =
                _publicSportDataUnitOfWork.TennisEvents.Where(e =>
                        e.TennisSeason.ProviderSeasonId == currentSeason.ProviderSeasonId &&
                        e.TennisTournament.TennisLeagues.Any(l => l.Slug.Equals(category)))
                    .OrderBy(e => e.StartDateUtc)
                    .ToList();

            if (currentValue)
            {
                events =
                    events.Where(e =>
                        e.StartDateUtc > today).ToList();
            }

            schedule = events.Select(x => new TennisEventEntity()
            {
                TennisEvent = x,
                TennisEventTennisLeague = 
                    _publicSportDataUnitOfWork.TennisEventTennisLeagues.FirstOrDefault(
                        m => m.TennisEvent.ProviderEventId == x.ProviderEventId &&
                             m.TennisLeague.ProviderSlug == category)
            }).ToList();

            return await Task.FromResult(schedule);
        }

        public Task<List<TennisRanking>> GetRankings(string category)
        {
            var rankings = new List<TennisRanking>();

            var season =
                _publicSportDataUnitOfWork.TennisSeasons.FirstOrDefault(
                    s => s.IsCurrent && s.TennisLeague.Slug == category);

            if (season == null)
                return Task.FromResult(rankings);

            rankings =
                _publicSportDataUnitOfWork.TennisRankings.Where(
                    r => r.TennisSeason.Id == season.Id &&
                         r.TennisRankingType == TennisRankingType.Rank)
                    .OrderBy(r => r.Rank)
                    .Take(100)
                    .ToList();

            return Task.FromResult(rankings);
        }

        public Task<List<TennisRanking>> GetRaceRankings(string category)
        {
            var rankings = new List<TennisRanking>();

            var season =
                _publicSportDataUnitOfWork.TennisSeasons.FirstOrDefault(
                    s => s.IsCurrent && s.TennisLeague.Slug == category);

            if (season == null)
                return Task.FromResult(rankings);

            rankings =
                _publicSportDataUnitOfWork.TennisRankings.Where(
                        r => r.TennisSeason.Id == season.Id &&
                             r.TennisRankingType == TennisRankingType.Race)
                    .OrderBy(r => r.Rank)
                    .Take(100)
                    .ToList();

            return Task.FromResult(rankings);
        }

        public async Task<List<TennisEventEntity>> GetCurrentSchedules(string category = null)
        {
            var today = DateTime.UtcNow;

            var events =
                _publicSportDataUnitOfWork.TennisEvents.Where(e =>
                        e.StartDateUtc <= today &&
                        e.EndDateUtc >= today)
                    .OrderBy(e => e.StartDateUtc)
                    .ToList();

            if (category != null)
            {
                events =
                    events.Where(e => 
                        e.TennisTournament.TennisLeagues.Any(l => 
                            l.Slug == category)).ToList();
            }
            else
            {
                events =
                    events.Where(e =>
                        e.TennisTournament.TennisTournamentType != TennisTournamentType.CO &&
                        e.TennisTournament.TennisTournamentType != TennisTournamentType.GS)
                     .ToList();
            }

            var schedule = events.Select(x => new TennisEventEntity()
            {
                TennisEvent = x,
                TennisEventTennisLeague =
                    _publicSportDataUnitOfWork.TennisEventTennisLeagues.FirstOrDefault(
                        m => m.TennisEvent.ProviderEventId == x.ProviderEventId)
            }).ToList();

            return await Task.FromResult(schedule);
        }

        public async Task<List<TennisMatchEntity>> GetTennisResults(int eventId)
        {
            var tennisEvent =
                _publicSportDataUnitOfWork.TennisEvents.FirstOrDefault(
                    e => e.LegacyEventId == eventId);

            if (tennisEvent == null)
                return await Task.FromResult(new List<TennisMatchEntity>());

            var seeds =
                _publicSportDataUnitOfWork.TennisEventSeeds.Where(
                    s => s.TennisEvent.LegacyEventId == eventId).ToList();

            var entities = 
                _publicSportDataUnitOfWork.TennisMatches.Where(m => 
                    m.AssociatedTennisEventTennisLeague.TennisEvent.LegacyEventId == eventId &&
                    (m.TennisMatchStatus == TennisMatchStatus.Final || m.TennisMatchStatus == TennisMatchStatus.Suspended))
                .OrderByDescending(m => m.MakeupDateTimeUtc ?? m.StartDateTimeUtc)
                .Select(m => new TennisMatchEntity()
                    {
                        TennisMatch = m,
                        TennisSeeds = seeds
                    }).ToList();

            return await Task.FromResult(entities);
        }

        public async Task<List<TennisMatchEntity>> GetLiveMatchesForEvent(int eventId)
        {
            var tennisEvent =
                _publicSportDataUnitOfWork.TennisEvents.FirstOrDefault(
                    e => e.LegacyEventId == eventId);

            if (tennisEvent == null)
                return await Task.FromResult(new List<TennisMatchEntity>());

            var seeds =
                _publicSportDataUnitOfWork.TennisEventSeeds.Where(
                    s => s.TennisEvent.LegacyEventId == eventId).ToList();

            var entities =
                _publicSportDataUnitOfWork.TennisMatches.Where(m =>
                        m.AssociatedTennisEventTennisLeague.TennisEvent.LegacyEventId == eventId &&
                        m.TennisMatchStatus == TennisMatchStatus.InProgress)
                    .OrderByDescending(m => m.MakeupDateTimeUtc ?? m.StartDateTimeUtc)
                    .Select(m => new TennisMatchEntity()
                    {
                        TennisMatch = m,
                        TennisSeeds = seeds
                    }).ToList();

            return await Task.FromResult(entities);
        }

        public async Task<List<TennisMatchEntity>> GetLiveMatchesForEventForMen(int eventId)
        {
            var tennisEvent =
                _publicSportDataUnitOfWork.TennisEvents.FirstOrDefault(
                    e => e.LegacyEventId == eventId);

            if (tennisEvent == null)
                return await Task.FromResult(new List<TennisMatchEntity>());

            var seeds =
                _publicSportDataUnitOfWork.TennisEventSeeds.Where(
                    s => s.TennisEvent.LegacyEventId == eventId).ToList();

            var entities =
                _publicSportDataUnitOfWork.TennisMatches.Where(m =>
                        m.AssociatedTennisEventTennisLeague.TennisEvent.LegacyEventId == eventId &&
                        m.TennisMatchStatus == TennisMatchStatus.InProgress &&
                        m.AssociatedTennisEventTennisLeague.TennisLeague.Gender == TennisGender.Male)
                    .OrderByDescending(m => m.MakeupDateTimeUtc ?? m.StartDateTimeUtc)
                    .Select(m => new TennisMatchEntity()
                    {
                        TennisMatch = m,
                        TennisSeeds = seeds
                    }).ToList();

            return await Task.FromResult(entities);
        }

        public async Task<List<TennisMatchEntity>> GetLiveMatchesForEventForWomen(int eventId)
        {
            var tennisEvent =
                _publicSportDataUnitOfWork.TennisEvents.FirstOrDefault(
                    e => e.LegacyEventId == eventId);

            if (tennisEvent == null)
                return await Task.FromResult(new List<TennisMatchEntity>());

            var seeds =
                _publicSportDataUnitOfWork.TennisEventSeeds.Where(
                    s => s.TennisEvent.LegacyEventId == eventId).ToList();

            var entities =
                _publicSportDataUnitOfWork.TennisMatches.Where(m =>
                        m.AssociatedTennisEventTennisLeague.TennisEvent.LegacyEventId == eventId &&
                        m.TennisMatchStatus == TennisMatchStatus.InProgress &&
                        m.AssociatedTennisEventTennisLeague.TennisLeague.Gender == TennisGender.Female)
                    .OrderByDescending(m => m.MakeupDateTimeUtc ?? m.StartDateTimeUtc)
                    .Select(m => new TennisMatchEntity()
                    {
                        TennisMatch = m,
                        TennisSeeds = seeds
                    }).ToList();

            return await Task.FromResult(entities);
        }

        public Task<List<TennisMatchEntity>> GetTennisResultsForMen(int eventId)
        {
            var tennisEvent =
                _publicSportDataUnitOfWork.TennisEvents.FirstOrDefault(
                    e => e.LegacyEventId == eventId);

            if (tennisEvent == null)
                return Task.FromResult(new List<TennisMatchEntity>());

            var seeds =
                _publicSportDataUnitOfWork.TennisEventSeeds.Where(
                    s => s.TennisEvent.LegacyEventId == eventId).ToList();

            var entities =
                _publicSportDataUnitOfWork.TennisMatches.Where(m =>
                        m.AssociatedTennisEventTennisLeague.TennisEvent.LegacyEventId == eventId &&
                        m.AssociatedTennisEventTennisLeague.TennisLeague.Gender == TennisGender.Male &&
                        (m.TennisMatchStatus == TennisMatchStatus.Final || m.TennisMatchStatus == TennisMatchStatus.Suspended))
                    .OrderByDescending(m => m.MakeupDateTimeUtc ?? m.StartDateTimeUtc)
                    .Select(m => new TennisMatchEntity()
                    {
                        TennisMatch = m,
                        TennisSeeds = seeds
                    }).ToList();

            return Task.FromResult(entities);
        }

        public Task<List<TennisMatchEntity>> GetTennisResultsForWomen(int eventId)
        {
            var tennisEvent =
                _publicSportDataUnitOfWork.TennisEvents.FirstOrDefault(
                    e => e.LegacyEventId == eventId);

            if (tennisEvent == null)
                return Task.FromResult(new List<TennisMatchEntity>());

            var seeds =
                _publicSportDataUnitOfWork.TennisEventSeeds.Where(
                    s => s.TennisEvent.LegacyEventId == eventId).ToList();

            var entities =
                _publicSportDataUnitOfWork.TennisMatches.Where(m =>
                        m.AssociatedTennisEventTennisLeague.TennisEvent.LegacyEventId == eventId &&
                        m.AssociatedTennisEventTennisLeague.TennisLeague.Gender == TennisGender.Female &&
                        (m.TennisMatchStatus == TennisMatchStatus.Final || m.TennisMatchStatus == TennisMatchStatus.Suspended))
                    .OrderByDescending(m => m.MakeupDateTimeUtc ?? m.StartDateTimeUtc)
                    .Select(m => new TennisMatchEntity()
                    {
                        TennisMatch = m,
                        TennisSeeds = seeds
                    }).ToList();

            return Task.FromResult(entities);
        }

        private async Task<TennisSeason> GetCurrentSeasonForCategory(string category)
        {
            return await Task.FromResult(
                _publicSportDataUnitOfWork.TennisSeasons.FirstOrDefault(s =>
                    s.IsCurrent && s.TennisLeague.Slug == category));
        }
    }
}
