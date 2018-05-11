using SuperSportDataEngine.ApplicationLogic.Entities.SystemAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.CmsLogic.Interfaces
{
    public interface IRugbyCmsService
    {
        Task<PagedResultsEntity<RugbyTournamentEntity>> GetAllTournaments(int pageIndex, int pageSize, string abpath, string query = null);
        Task<PagedResultsEntity<RugbyFixtureEntity>> GetAllFixtures(int pageIndex, int pageSize, string abpath, string query = null);
        Task<PagedResultsEntity<RugbySeasonEntity>> GetAllSeasons(int pageIndex, int pageSize, string abpath, string query = null);
        Task<PagedResultsEntity<RugbyTeamEntity>> GetAllTeams(int pageIndex, int pageSize, string abpath, string query = null);
        Task<PagedResultsEntity<RugbyPlayerEntity>> GetAllPlayers(int pageIndex, int pageSize, string abpath, string query = null);
        Task<PagedResultsEntity<RugbyVenueEntity>> GetAllVenues(int pageIndex, int pageSize, string abpath, string query = null);
        Task<PagedResultsEntity<RugbySeasonEntity>> GetSeasonsForTournament(Guid tournamentId, int pageIndex, int pageSize, string abpath, string query = null);
        Task<PagedResultsEntity<RugbyFixtureEntity>> GetFixturesForTournamentSeason(Guid seasonId, int pageIndex, int pageSize, string abpath, string query = null);
        Task<PagedResultsEntity<RugbyFixtureEntity>> GetTournamentFixtures(Guid tournamentId, Guid? seasonId, int pageIndex, int pageSize, string abpath, string query = null, string status = null);

        Task<RugbyTournamentEntity> GetTournamentById(Guid tournamentId);
        Task<RugbyFixtureEntitySingle> GetFixtureById(Guid fixtureId);
        Task<RugbySeasonEntity> GetSeasonById(Guid seasonId);
        Task<RugbyTeamEntity> GetTeamById(Guid teamId);
        Task<RugbyPlayerEntity> GetPlayerById(Guid playerId);
        Task<RugbyVenueEntity> GetVenueById(Guid id);

        Task<bool> UpdateTournament(Guid tournamentId, RugbyTournamentEntity rugbyTournamentEntity);
        Task<bool> UpdateFixture(Guid fixtureId, RugbyFixtureEntity rugbyFixtureEntity);
        Task<bool> UpdateSeason(Guid seasonId, RugbySeasonEntity rugbySeasonEntity);
        Task<bool> UpdateTeam(Guid teamId, RugbyTeamEntity rugbyTeamEntity);
        Task<bool> UpdatePlayer(Guid playerId, RugbyPlayerEntity rugbyPlayerEntity);
        Task<bool> UpdateVenue(Guid id, RugbyVenueEntity rugbyVenueEntity);
    }
}
