using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models.Tennis;
using SuperSportDataEngine.ApplicationLogic.Entities.Legacy.Tennis;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces.LegacyFeed
{
    public interface ITennisLegacyFeedService
    {
        Task<List<TennisEventEntity>> GetSchedules(string category, bool currentValue);
        Task<List<TennisRanking>> GetRankings(string category);
        Task<List<TennisRanking>> GetRaceRankings(string category);
        Task<List<TennisEventEntity>> GetCurrentSchedules(string category = null);
        Task<List<TennisMatchEntity>> GetTennisResults(int eventId);
        Task<List<TennisMatchEntity>> GetTennisResultsForMen(int eventId);
        Task<List<TennisMatchEntity>> GetTennisResultsForWomen(int eventId);
        Task<List<TennisMatchEntity>> GetLiveMatchesForEvent(int eventId);
        Task<List<TennisMatchEntity>> GetLiveMatchesForEventForMen(int eventId);
        Task<List<TennisMatchEntity>> GetLiveMatchesForEventForWomen(int eventId);
    }
}
