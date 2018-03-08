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
    }
}
