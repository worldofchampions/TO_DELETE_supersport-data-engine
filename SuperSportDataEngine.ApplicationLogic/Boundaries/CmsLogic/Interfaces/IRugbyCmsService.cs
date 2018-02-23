using SuperSportDataEngine.ApplicationLogic.Entities.SystemAPI;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.CmsLogic.Interfaces
{
    public interface IRugbyCmsService
    {
        Task<IEnumerable<RugbyTournamentEntity>> GetAllTournaments(int pageIndex, int pageSize, string query = null);
        Task<IEnumerable<RugbyFixtureEntity>> GetAllFixtures(int pageIndex, int pageSize);
        Task<IEnumerable<RugbySeasonEntity>> GetAllSeasons(int pageIndex, int pageSize);
        Task<IEnumerable<RugbyTeamEntity>> GetAllTeams(int pageIndex, int pageSize);
        Task<IEnumerable<RugbyPlayerEntity>> GetAllPlayers(int pageIndex, int pageSize);
        Task<IEnumerable<RugbySeasonEntity>> GetSeasonsForTournament(Guid tournamentId, int pageIndex, int pageSize);
        Task<IEnumerable<RugbyFixtureEntity>> GetFixturesForTournamentSeason(Guid seasonId, int pageIndex, int pageSize);

        Task<RugbyTournamentEntity> GetTournamentById(Guid tournamentId);
        Task<RugbyFixtureEntity> GetFixtureById(Guid fixtureId);
        Task<RugbySeasonEntity> GetSeasonById(Guid seasonId);
        Task<RugbyTeamEntity> GetTeamById(Guid teamId);
        Task<RugbyPlayerEntity> GetPlayerById(Guid playerId);

        Task<bool> UpdateTournament(Guid tournamentId, RugbyTournamentEntity rugbyTournamentEntity);
        Task<bool> UpdateFixture(Guid fixtureId, RugbyFixtureEntity rugbyFixtureEntity);
        Task<bool> UpdateSeason(Guid seasonId, RugbySeasonEntity rugbySeasonEntity);
        Task<bool> UpdateTeam(Guid teamId, RugbyTeamEntity rugbyTeamEntity);
        Task<bool> UpdatePlayer(Guid playerId, RugbyPlayerEntity rugbyPlayerEntity);
    }
}
