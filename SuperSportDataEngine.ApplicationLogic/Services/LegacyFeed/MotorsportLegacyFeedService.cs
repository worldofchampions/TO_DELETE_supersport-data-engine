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
        //private readonly IPublicSportDataUnitOfWork _publicSportDataUnitOfWork;

        //public MotorsportLegacyFeedService(IPublicSportDataUnitOfWork publicSportDataUnitOfWork)
        //{
        //    _publicSportDataUnitOfWork = publicSportDataUnitOfWork;
        //}

        //public async Task<MotorsportScheduleEntity> GetSchedules()
        //{
        //    var raceEvents =
        //        _publicSportDataUnitOfWork.MotorsportRaceEvents.Where(e =>
        //            e.MotorsportSeason.IsCurrent).ToList();

        //    var schedule = new MotorsportScheduleEntity()
        //    {
        //        MotorsportRaceEvents = raceEvents
        //    };

        //    return await Task.FromResult(schedule);
        //}
    }
}
