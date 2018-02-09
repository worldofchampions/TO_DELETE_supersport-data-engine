using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
using SuperSportDataEngine.ApplicationLogic.Entities.SystemAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.CmsLogic.Interfaces
{
    public interface IRugbyCmsService
    {
        Task<IEnumerable<RugbyTournamentEntity>> GetAllTournaments(int pageIndex, int pageSize);
        Task<IEnumerable<RugbyFixtureEntity>> GetAllFixtures(int pageIndex, int pageSize);
        Task<IEnumerable<RugbySeasonEntity>> GetAllSeasons(int pageIndex, int pageSize);
        Task<IEnumerable<RugbyTeamEntity>> GetAllTeams(int pageIndex, int pageSize);
        Task<IEnumerable<RugbyPlayerEntity>> GetAllPlayers(int pageIndex, int pageSize);

        Task<RugbyTournamentEntity> GetTournamentById(int id);
        Task<RugbyFixtureEntity> GetFixtureById(int id);
        Task<RugbySeasonEntity> GetSeasonById(int id);
        Task<RugbyTeamEntity> GetTeamById(int id);
        Task<RugbyPlayerEntity> GetPlayerById(int id);

        Task<bool> UpdateTournament(int tournamentId, RugbyTournamentEntity rugbyTournamentEntity);
        Task<bool> UpdateFixture(int fixtureId, RugbyFixtureEntity rugbyFixtureEntity);
        Task<bool> UpdateSeason(int seasonId, RugbySeasonEntity rugbySeasonEntity);
        Task<bool> UpdateTeam(int teamId, RugbyTeamEntity rugbyTeamEntity);
        Task<bool> UpdatePlayer(int playerId, RugbyPlayerEntity rugbyPlayerEntity);
    }
}
