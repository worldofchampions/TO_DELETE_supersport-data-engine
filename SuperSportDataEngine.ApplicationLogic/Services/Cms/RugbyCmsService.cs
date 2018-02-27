using SuperSportDataEngine.ApplicationLogic.Boundaries.CmsLogic.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
using System;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;
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
            IBaseEntityFrameworkRepository<RugbyTeam> rugbyTeamRepository,
            IBaseEntityFrameworkRepository<RugbyFixture> rugbyFixtureRepository,
            IBaseEntityFrameworkRepository<RugbySeason> rugbySeasonRepository,
            IBaseEntityFrameworkRepository<RugbyPlayer> rugbyPlayerRepository
            )
        {
            _rugbyTournamentRepository = rugbyTournamentRepository;
            _rugbyFixtureRepository = rugbyFixtureRepository;
            _rugbySeasonRepository = rugbySeasonRepository;
            _rugbyTeamRepository = rugbyTeamRepository;
            _rugbyPlayerRepository = rugbyPlayerRepository;

            var config = new MapperConfiguration(
                cfg =>
                {
                    cfg.CreateMap<RugbyTournament, RugbyTournamentEntity>();
                    cfg.CreateMap<RugbyTeam, RugbyTeamEntity>().ReverseMap();
                    cfg.CreateMap<RugbyFixture, RugbyFixtureEntity>()
                        .ForMember(dest => dest.TeamA, opt => opt.MapFrom(origin => origin.TeamA))
                        .ForMember(dest => dest.TeamB, opt => opt.MapFrom(origin => origin.TeamB));
                    cfg.CreateMap<RugbySeason, RugbySeasonEntity>();
                    cfg.CreateMap<RugbyPlayer, RugbyPlayerEntity>();
                });
            iMapper = config.CreateMapper();
        }

        public async Task<PagedResultsEntity<RugbyTournamentEntity>> GetAllTournaments(int pageIndex, int pageSize, string abpath, string query = null)
        {
            var tournaments = (PagedResultsEntity<RugbyTournamentEntity>)null;

            if (!String.IsNullOrEmpty(query))
            {
                tournaments = await CreatePagedResults<RugbyTournament, RugbyTournamentEntity>(
                                    _rugbyTournamentRepository.Where(q => q.Name.Contains(query)
                                                        || q.NameCmsOverride.Contains(query)), pageIndex, pageSize, abpath);
            }
            else
            {
                tournaments = await CreatePagedResults<RugbyTournament, RugbyTournamentEntity>(_rugbyTournamentRepository.All(), pageIndex, pageSize, abpath);
            }

            if (tournaments.Results.Any())
            {
                return tournaments;
            }
            return null;
        }

        public async Task<PagedResultsEntity<RugbyFixtureEntity>> GetAllFixtures(int pageIndex, int pageSize, string abpath, string query = null)
        {
            var fixtures = (PagedResultsEntity<RugbyFixtureEntity>)null;

            if (!String.IsNullOrEmpty(query))
            {
                fixtures = await CreatePagedResults<RugbyFixture, RugbyFixtureEntity>(
                                    _rugbyFixtureRepository.Where(q => q.TeamA.Name.Contains(query)
                                                        || q.TeamA.NameCmsOverride.Contains(query)
                                                        || q.TeamB.Name.Contains(query)
                                                        || q.TeamB.NameCmsOverride.Contains(query)), pageIndex, pageSize, abpath);
            }
            else
            {
                fixtures = await CreatePagedResults<RugbyFixture, RugbyFixtureEntity>(_rugbyFixtureRepository.All(), pageIndex, pageSize, abpath);
            }

            if (fixtures.Results.Any())
            {
                return fixtures;
            }
            return null;
        }

        public async Task<PagedResultsEntity<RugbySeasonEntity>> GetAllSeasons(int pageIndex, int pageSize, string abpath, string query = null)
        {
            var seasons = (PagedResultsEntity<RugbySeasonEntity>)null;

            if (!String.IsNullOrEmpty(query))
            {
                seasons = await CreatePagedResults<RugbySeason, RugbySeasonEntity>(
                                    _rugbySeasonRepository.Where(q => q.Name.Contains(query)), pageIndex, pageSize, abpath);
            }
            else
            {
                seasons = await CreatePagedResults<RugbySeason, RugbySeasonEntity>(_rugbySeasonRepository.All(), pageIndex, pageSize, abpath);
            }

            if (seasons.Results.Any())
            {
                return seasons;
            }
            return null;
        }

        public async Task<PagedResultsEntity<RugbyTeamEntity>> GetAllTeams(int pageIndex, int pageSize, string abpath, string query = null)
        {
            var teams = (PagedResultsEntity<RugbyTeamEntity>)null;

            if (!String.IsNullOrEmpty(query))
            {
                teams = await CreatePagedResults<RugbyTeam, RugbyTeamEntity>(
                                    _rugbyTeamRepository.Where(q => q.Name.Contains(query)
                                                        || q.NameCmsOverride.Contains(query)
                                                        || q.Name.Contains(query)
                                                        || q.NameCmsOverride.Contains(query)), pageIndex, pageSize, abpath);
            }
            else
            {
                teams = await CreatePagedResults<RugbyTeam, RugbyTeamEntity>(_rugbyTeamRepository.All(), pageIndex, pageSize, abpath);
            }

            if (teams.Results.Any())
            {
                return teams;
            }
            return null;
        }

        public async Task<PagedResultsEntity<RugbyPlayerEntity>> GetAllPlayers(int pageIndex, int pageSize, string abpath, string query = null)
        {
            var players = (PagedResultsEntity<RugbyPlayerEntity>)null;

            if (!String.IsNullOrEmpty(query))
            {
                players = await CreatePagedResults<RugbyPlayer, RugbyPlayerEntity>(
                                    _rugbyPlayerRepository.Where(q => q.FirstName.Contains(query)
                                                           || q.LastName.Contains(query)
                                                           || q.DisplayNameCmsOverride.Contains(query)), pageIndex, pageSize, abpath);
            }
            else
            {
                players = await CreatePagedResults<RugbyPlayer, RugbyPlayerEntity>(_rugbyPlayerRepository.All(), pageIndex, pageSize, abpath);
            }

            if (players.Results.Any())
            {
                return players;
            }
            return null;
        }

        public async Task<PagedResultsEntity<RugbySeasonEntity>> GetSeasonsForTournament(Guid tournamentId, int pageIndex, int pageSize, string abpath, string query = null)
        {
            var tournamentSeasons = (PagedResultsEntity<RugbySeasonEntity>)null;

            if (!String.IsNullOrEmpty(query))
            {
                tournamentSeasons = await CreatePagedResults<RugbySeason, RugbySeasonEntity>(
                                    _rugbySeasonRepository.Where(q => q.Name.Contains(query) && q.RugbyTournament.Id == tournamentId), pageIndex, pageSize, abpath);
            }
            else
            {
                tournamentSeasons = await CreatePagedResults<RugbySeason, RugbySeasonEntity>(_rugbySeasonRepository.Where(
                                                            season => season.RugbyTournament.Id == tournamentId), pageIndex, pageSize, abpath);
            }

            if (tournamentSeasons.Results.Any())
            {
                return tournamentSeasons;
            }
            return null;
        }

        public async Task<PagedResultsEntity<RugbyFixtureEntity>> GetFixturesForTournamentSeason(Guid seasonId, int pageIndex, int pageSize, string abpath, string query = null)
        {
            var tournamentSeasonFixtures = (PagedResultsEntity<RugbyFixtureEntity>)null;

            if (!String.IsNullOrEmpty(query))
            {
                tournamentSeasonFixtures = await CreatePagedResults<RugbyFixture, RugbyFixtureEntity>(
                                    _rugbyFixtureRepository.Where(fixture => fixture.RugbySeason.Id == seasonId)
                                                        .Where(q => q.TeamA.Name.Contains(query)
                                                        || q.TeamA.NameCmsOverride.Contains(query)
                                                        || q.TeamB.Name.Contains(query)
                                                        || q.TeamB.NameCmsOverride.Contains(query)), pageIndex, pageSize, abpath);
            }
            else
            {
                tournamentSeasonFixtures = await CreatePagedResults<RugbyFixture, RugbyFixtureEntity>(_rugbyFixtureRepository.Where(
                                                            fixture => fixture.RugbySeason.Id == seasonId), pageIndex, pageSize, abpath);
            }

            if (tournamentSeasonFixtures.Results.Any())
            {
                return tournamentSeasonFixtures;
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
                var rugbyFixture = (await Task.FromResult(_rugbyFixtureRepository.FirstOrDefault(
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

        protected async Task<PagedResultsEntity<TReturn>> CreatePagedResults<T, TReturn>(IEnumerable<T> queryable, int pageIndex, int pageSize, string abpath)
        {
            var skipAmount = pageSize * (pageIndex - 1);

            var projection = queryable.Skip(skipAmount).Take(pageSize).ToList();
            var totalNumberOfRecords = await Task.FromResult(queryable.Count());

            var mod = totalNumberOfRecords % pageSize;
            var totalPageCount = (totalNumberOfRecords / pageSize) + (mod == 0 ? 0 : 1);
             
            var nextPageUrl = pageIndex == totalPageCount ? null : abpath + string.Format("?pageIndex={0}&pageSize={1}", pageIndex + 1, pageSize);

            var results = iMapper.Map<IEnumerable<T>, IEnumerable<TReturn>>(projection);
            return new PagedResultsEntity<TReturn>
            {
                Results = results,
                PageNumber = pageIndex,
                PageSize = projection.Count(),
                TotalNumberOfPages = totalPageCount,
                TotalNumberOfRecords = totalNumberOfRecords,
                NextPageUrl = nextPageUrl
            };
        }
    }
}
