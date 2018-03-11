using System.Threading.Tasks;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
using SuperSportDataEngine.ApplicationLogic.Entities.Legacy.Motorsport;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces.LegacyFeed
{
    // TODO: @motorsport-feed: implement.
    public interface IMotorsportLegacyFeedService
    {
        Task<MotorsportScheduleEntity> GetSchedules(string category, bool current);
        Task<MotorsportRaceEventGridEntity> GetGridForRaceEventId(string category, int eventId);
        Task<MotorsportRaceEventGridEntity> GetLatestGrid(string category);
    }
}
