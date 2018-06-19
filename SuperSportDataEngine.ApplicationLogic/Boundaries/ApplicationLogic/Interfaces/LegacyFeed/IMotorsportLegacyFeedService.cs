namespace SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces.LegacyFeed
{
    using SuperSportDataEngine.ApplicationLogic.Entities.Legacy.Motorsport;
    using System.Threading.Tasks;

    public interface IMotorsportLegacyFeedService : ISportFeedService
    {
        Task<MotorsportScheduleEntity> GetSchedules(string category, bool current);

        Task<MotorsportRaceEventGridEntity> GetGridForRaceEventId(string category, int eventId);

        Task<MotorsportRaceEventGridEntity> GetLatestGrid(string category);

        Task<MotorsportRaceEventResultsEntity> GetResultsForRaceEventId(string category, int eventId);

        Task<MotorsportRaceEventResultsEntity> GetLatestResult(string category);

        Task<MotorsportDriverStandingsEntity> GetDriverStandings(string category);

        Task<MotorsportTeamStandingsEntity> GetTeamStandings(string category);
    }
}
