using System;
using System.Linq;
using System.Threading.Tasks;
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

            // [TODO] This needs to be removed before we deploy.
            // [TODO] Since we are not getting data for 2018, show 2017 data if the 2017 season is current.
            today = new DateTime(currentSeason.ProviderSeasonId, today.Month, today.Day);

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
