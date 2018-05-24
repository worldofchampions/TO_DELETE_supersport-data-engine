using SuperSportDataEngine.ApplicationLogic.Entities.SystemAPI;
using SuperSportDataEngine.ApplicationLogic.Entities.SystemAPI.Motorsport;
using System;
using System.Threading.Tasks;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.CmsLogic.Interfaces
{
    public interface IMotorsportCmsService
    {
        Task<PagedResultsEntity<MotorsportLeagueEntity>> GetAllLeagues(int pageIndex, int pageSize, string abpath, string query = null);

        Task<MotorsportLeagueEntity> GetLeagueById(Guid leagueId);

        Task<bool> UpdateLeague(Guid leagueId, MotorsportLeagueEntity motorsportLeagueEntity);
    }
}
