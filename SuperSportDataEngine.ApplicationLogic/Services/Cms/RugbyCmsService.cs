using SuperSportDataEngine.ApplicationLogic.Boundaries.CmsLogic.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
using System;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperSportDataEngine.ApplicationLogic.Entities.SystemAPI;
using System.Text.RegularExpressions;

namespace SuperSportDataEngine.ApplicationLogic.Services.Cms
{
    public class RugbyCmsService : IRugbyCmsService
    {
        private readonly IBaseEntityFrameworkRepository<RugbyTournament> _rugbyTournamentRepository;
        private readonly IBaseEntityFrameworkRepository<RugbyFixture> _rugbyFixtureRepository;
        private readonly IBaseEntityFrameworkRepository<RugbySeason> _rugbySeasonRepository;
        private readonly IBaseEntityFrameworkRepository<RugbyTeam> _rugbyTeamRepository;
        private readonly IBaseEntityFrameworkRepository<RugbyPlayer> _rugbyPlayerRepository;

        IMapper iMapper;

        public RugbyCmsService(
            IBaseEntityFrameworkRepository<RugbyTournament> rugbyTournamentRepository,
            IBaseEntityFrameworkRepository<RugbyFixture> rugbyFixtureRepository,
            IBaseEntityFrameworkRepository<RugbySeason> rugbySeasonRepository,
            IBaseEntityFrameworkRepository<RugbyTeam> rugbyTeamRepository,
            IBaseEntityFrameworkRepository<RugbyPlayer> rugbyPlayerRepository
            )
        {
            _rugbyTournamentRepository = rugbyTournamentRepository;
            _rugbyFixtureRepository = rugbyFixtureRepository;
            _rugbySeasonRepository = rugbySeasonRepository;
            _rugbyTeamRepository = rugbyTeamRepository;
            _rugbyPlayerRepository = rugbyPlayerRepository;

            var config = new MapperConfiguration(
                cfg => { cfg.CreateMap<RugbyTournament, RugbyTournamentEntity>();
                        cfg.CreateMap<RugbyFixture, RugbyFixtureEntity>();
                        cfg.CreateMap<RugbySeason, RugbySeasonEntity>();
                        cfg.CreateMap<RugbyTeam, RugbyTeamEntity>();
                        cfg.CreateMap<RugbyPlayer, RugbyPlayerEntity>();
                });
            iMapper = config.CreateMapper();
        }

        public async Task<IEnumerable<RugbyTournamentEntity>> GetAllTournaments(int pageIndex, int pageSize, string query = null)
        {
            var tournaments = (IEnumerable<RugbyTournament>)null;

            if (!String.IsNullOrEmpty(query))
            {
                tournaments = await Task.FromResult(_rugbyTournamentRepository.Where(q => q.Name.Contains(query) 
                                                        || q.NameCmsOverride.Contains(query) )
                                                        .Skip(pageIndex * pageSize).Take(pageSize));
            }
            else
            {
                tournaments = await Task.FromResult(_rugbyTournamentRepository.All().Skip(pageIndex * pageSize).Take(pageSize));
            }
            
            if (tournaments.Any())
            {
                return iMapper.Map<IEnumerable<RugbyTournament>, IEnumerable<RugbyTournamentEntity>>(tournaments);
            }
            return null;
        }

        public async Task<IEnumerable<RugbyFixtureEntity>> GetAllFixtures(int pageIndex, int pageSize)
        {
            var fixtures = await Task.FromResult(_rugbyFixtureRepository.All().Skip(pageIndex * pageSize).Take(pageSize));

            if (fixtures.Any())
            {
                return iMapper.Map<IEnumerable<RugbyFixture>, IEnumerable<RugbyFixtureEntity>>(fixtures);
            }
            return null;
        }

        public async Task<IEnumerable<RugbySeasonEntity>> GetAllSeasons(int pageIndex, int pageSize)
        {
            var seasons = await Task.FromResult(_rugbySeasonRepository.All().Skip(pageIndex * pageSize).Take(pageSize));

            if (seasons.Any())
            {
                return iMapper.Map<IEnumerable<RugbySeason>, IEnumerable<RugbySeasonEntity>>(seasons);
            }
            return null;
        }

        public async Task<IEnumerable<RugbyTeamEntity>> GetAllTeams(int pageIndex, int pageSize)
        {
            var teams = await Task.FromResult(_rugbyTeamRepository.All().Skip(pageIndex * pageSize).Take(pageSize));

            if (teams.Any())
            {
                return iMapper.Map<IEnumerable<RugbyTeam>, IEnumerable<RugbyTeamEntity>>(teams);
            }
            return null;
        }

        public async Task<IEnumerable<RugbyPlayerEntity>> GetAllPlayers(int pageIndex, int pageSize)
        {
            var players = await Task.FromResult(_rugbyPlayerRepository.All().Skip(pageIndex * pageSize).Take(pageSize));

            if (players.Any())
            {
                return iMapper.Map<IEnumerable<RugbyPlayer>, IEnumerable<RugbyPlayerEntity>>(players);
            }
            return null;
        }

        public async Task<IEnumerable<RugbySeasonEntity>> GetSeasonsForTournament(Guid tournamentId, int pageIndex, int pageSize)
        {
            var tournamentSeasons = await Task.FromResult(_rugbySeasonRepository.Where(
                                                            query => query.RugbyTournament.Id == tournamentId)
                                                            .Skip(pageIndex * pageSize)
                                                            .Take(pageSize));

            if (tournamentSeasons.Any())
            {
                return iMapper.Map<IEnumerable<RugbySeason>, IEnumerable<RugbySeasonEntity>>(tournamentSeasons);
            }
            return null;
        }

        public async Task<IEnumerable<RugbyFixtureEntity>> GetFixturesForTournamentSeason(Guid seasonId, int pageIndex, int pageSize)
        {
            var tournamentSeasonFixtures = await Task.FromResult(_rugbyFixtureRepository.Where(
                                                            query => query.RugbySeason.Id == seasonId)
                                                            .Skip(pageIndex * pageSize)
                                                            .Take(pageSize));

            if (tournamentSeasonFixtures.Any())
            {
                return iMapper.Map<IEnumerable<RugbyFixture>, IEnumerable<RugbyFixtureEntity>>(tournamentSeasonFixtures);
            }
            return null;
        }

        public async Task<RugbyTournamentEntity> GetTournamentById(Guid id)
        {
            var rugbyTournament = await Task.FromResult(_rugbyTournamentRepository.FirstOrDefault(
                                                        tournament => tournament.Id == id));

            if (rugbyTournament != null)
            {
                return iMapper.Map<RugbyTournament, RugbyTournamentEntity>(rugbyTournament);
            }

            return null;
        }

        public async Task<RugbyFixtureEntity> GetFixtureById(Guid id)
        {
            var rugbyFixture = await Task.FromResult(_rugbyFixtureRepository.FirstOrDefault(
                                                        fixture => fixture.Id == id));

            if (rugbyFixture != null)
            {
                return iMapper.Map<RugbyFixture, RugbyFixtureEntity>(rugbyFixture);
            }

            return null;
        }

        public async Task<RugbySeasonEntity> GetSeasonById(Guid id)
        {
            var rugbySeason = await Task.FromResult(_rugbySeasonRepository.FirstOrDefault(
                                                        season => season.Id == id));

            if (rugbySeason != null)
            {
                return iMapper.Map<RugbySeason, RugbySeasonEntity>(rugbySeason);
            }

            return null;
        }

        public async Task<RugbyTeamEntity> GetTeamById(Guid id)
        {
            var rugbyTeam = await Task.FromResult(_rugbyTeamRepository.FirstOrDefault(
                                                        team => team.Id == id));

            if (rugbyTeam != null)
            {
                return iMapper.Map<RugbyTeam, RugbyTeamEntity>(rugbyTeam);
            }

            return null;
        }

        public async Task<RugbyPlayerEntity> GetPlayerById(Guid id)
        {
            var rugbyPlayer = await Task.FromResult(_rugbyPlayerRepository.FirstOrDefault(
                                                        player => player.Id == id));

            if (rugbyPlayer != null)
            {
                return iMapper.Map<RugbyPlayer, RugbyPlayerEntity>(rugbyPlayer);
            }

            return null;
        }

        /// <summary>
        /// Updates tournament table and only accepts values => NameCmsOverride, IsEnabled, IsLiveScored, Slug
        /// </summary>
        /// <param name="tournamentId"></param>
        /// <param name="rugbyTournamentEntity"></param>
        /// <returns></returns>
        public async Task<bool> UpdateTournament(Guid id, RugbyTournamentEntity rugbyTournamentEntity)
        {
            var success = false;

            if (rugbyTournamentEntity != null)
            {
                var rugbyTournament = (await Task.FromResult(_rugbyTournamentRepository.FirstOrDefault(
                                                            tournament => tournament.Id == id)));

                if (rugbyTournament != null)
                {
                    /** Only accept these values hard coded below **/
                    rugbyTournament.NameCmsOverride = rugbyTournamentEntity.NameCmsOverride;
                    rugbyTournament.IsEnabled = rugbyTournamentEntity.IsEnabled;
                    rugbyTournament.IsLiveScored = rugbyTournamentEntity.IsLiveScored;
                    rugbyTournament.Slug = Regex.Replace(rugbyTournamentEntity.Slug, @"[^A-Za-z0-9_\.~]+", "-");

                    _rugbyTournamentRepository.Update(rugbyTournament);
                    await _rugbyTournamentRepository.SaveAsync();
                    success = true;
                }

            }
            return success;
        }

        /// <summary>
        /// Updates fixture table and only accepts values => IsDisabledOutbound, IsDisabledInbound, IsLiveScored, CmsOverrideModeIsActive
        /// </summary>
        /// <param name="fixtureId"></param>
        /// <param name="rugbyFixtureEntity"></param>
        /// <returns></returns>
        public async Task<bool> UpdateFixture(Guid id, RugbyFixtureEntity rugbyFixtureEntity)
        {
            var success = false;

            if (rugbyFixtureEntity != null)
            {
                var rugbyFixture= (await Task.FromResult(_rugbyFixtureRepository.FirstOrDefault(
                                                            fixture => fixture.Id == id)));

                if (rugbyFixture != null)
                {
                    /** Only accept these values hard coded below **/
                    rugbyFixture.IsDisabledInbound = rugbyFixtureEntity.IsDisabledInbound;
                    rugbyFixture.IsDisabledOutbound = rugbyFixtureEntity.IsDisabledOutbound;
                    rugbyFixture.IsLiveScored = rugbyFixtureEntity.IsLiveScored;
                    rugbyFixture.CmsOverrideModeIsActive = rugbyFixtureEntity.CmsOverrideModeIsActive;

                    _rugbyFixtureRepository.Update(rugbyFixture);
                    await _rugbyFixtureRepository.SaveAsync();
                    success = true;
                }

            }
            return success;
        }

        /// <summary>
        /// Updates season table and only accepts values => IsCurrent, CurrentRoundNumber
        /// </summary>
        /// <param name="seasonId"></param>
        /// <param name="rugbySeasonEntity"></param>
        /// <returns></returns>
        public async Task<bool> UpdateSeason(Guid id, RugbySeasonEntity rugbySeasonEntity)
        {
            var success = false;

            if (rugbySeasonEntity != null)
            {
                var rugbySeason = (await Task.FromResult(_rugbySeasonRepository.FirstOrDefault(
                                                            season => season.Id == id)));

                if (rugbySeason != null)
                {
                    /** Only accept these values hard coded below **/
                    rugbySeason.IsCurrent = rugbySeasonEntity.IsCurrent;
                    rugbySeason.CurrentRoundNumber = rugbySeasonEntity.CurrentRoundNumber;

                    _rugbySeasonRepository.Update(rugbySeason);
                    await _rugbySeasonRepository.SaveAsync();
                    success = true;
                }

            }
            return success;
        }


        /// <summary>
        /// Updates rugby teams table and only accepts values => NameCmsOverride
        /// </summary>
        /// <param name="teamId"></param>
        /// <param name="rugbyteamEntity"></param>
        /// <returns></returns>
        public async Task<bool> UpdateTeam(Guid id, RugbyTeamEntity rugbyteamEntity)
        {
            var success = false;

            if (rugbyteamEntity != null)
            {
                var rugbyTeam = (await Task.FromResult(_rugbyTeamRepository.FirstOrDefault(
                                                            team => team.Id == id)));

                if (rugbyTeam != null)
                {
                    /** Only accept these values hard coded below **/
                    rugbyTeam.NameCmsOverride = rugbyteamEntity.NameCmsOverride;

                    _rugbyTeamRepository.Update(rugbyTeam);
                    await _rugbyTeamRepository.SaveAsync();
                    success = true;
                }

            }
            return success;
        }

        /// <summary>
        /// Updates rugby players table and only accepts values => DisplayNameCmsOverride
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="rugbyPlayerEntity"></param>
        /// <returns></returns>
        public async Task<bool> UpdatePlayer(Guid id, RugbyPlayerEntity rugbyPlayerEntity)
        {
            var success = false;

            if (rugbyPlayerEntity != null)
            {
                var rugbyPlayer = (await Task.FromResult(_rugbyPlayerRepository.FirstOrDefault(
                                                            player => player.Id == id)));

                if (rugbyPlayer != null)
                {
                    /** Only accept these values hard coded below **/
                    rugbyPlayer.DisplayNameCmsOverride = rugbyPlayerEntity.DisplayNameCmsOverride;

                    _rugbyPlayerRepository.Update(rugbyPlayer);
                    await _rugbyPlayerRepository.SaveAsync();
                    success = true;
                }

            }
            return success;
        }
    }
}
