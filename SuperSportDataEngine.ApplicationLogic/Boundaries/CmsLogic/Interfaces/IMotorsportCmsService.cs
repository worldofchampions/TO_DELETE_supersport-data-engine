using SuperSportDataEngine.ApplicationLogic.Entities.SystemAPI;
using SuperSportDataEngine.ApplicationLogic.Entities.SystemAPI.Motorsport;
using System;
using System.Threading.Tasks;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.CmsLogic.Interfaces
{
    public interface IMotorsportCmsService
    {
        Task<PagedResultsEntity<MotorsportLeagueEntity>> GetAllLeagues(int pageIndex, int pageSize, string abpath, string query = null);
        Task<PagedResultsEntity<MotorsportTeamEntity>> GetAllTeams(int pageIndex, int pageSize, string abpath, string query = null);
        Task<PagedResultsEntity<MotorsportSeasonEntity>> GetSeasonsForLeague(Guid leagueId, int pageIndex, int pageSize, string abpath, string query = null);
        Task<PagedResultsEntity<MotorsportRaceEntity>> GetRacesForLeague(Guid leagueId, int pageIndex, int pageSize, string abpath, string query = null);
        Task<PagedResultsEntity<MotorsportDriverEntity>> GetDriversForLeague(Guid leagueId, int pageIndex, int pageSize, string abpath, string query = null);
        Task<PagedResultsEntity<MotorsportRaceEventEntity>> GetRaceEvents(Guid raceId, Guid? seasonId, int pageIndex, int pageSize, string abpath, string query = null, string status = null);

        Task<MotorsportLeagueEntity> GetLeagueById(Guid leagueId);
        Task<MotorsportTeamEntity> GetTeamById(Guid teamId);
        Task<MotorsportSeasonEntity> GetSeasonById(Guid seasonId);
        Task<MotorsportRaceEntity> GetRaceById(Guid raceId);
        Task<MotorsportDriverEntity> GetDriverById(Guid driverId);
        Task<MotorsportRaceEventEntitySingle> GetRaceEventById(Guid raceEventId);

        Task<bool> UpdateLeague(Guid leagueId, MotorsportLeagueEntity motorsportLeagueEntity);
        Task<bool> UpdateTeam(Guid teamId, MotorsportTeamEntity rugbyTeamEntity);
        Task<bool> UpdateSeason(Guid seasonId, Guid leagueId, MotorsportSeasonEntity motorsportSeasonEntity);
        Task<bool> UpdateRace(Guid raceId, MotorsportRaceEntity motorsportRaceEntity);
        Task<bool> UpdateDriver(Guid driverId, MotorsportDriverEntity motorsportDriverEntity);
        Task<bool> UpdateRaceEvent(Guid raceId, MotorsportRaceEventEntity motorsportRaceEventEntity);
    }
}
