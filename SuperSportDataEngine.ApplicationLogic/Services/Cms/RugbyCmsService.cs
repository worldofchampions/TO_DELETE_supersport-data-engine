using SuperSportDataEngine.ApplicationLogic.Boundaries.CmsLogic.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
using System;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SuperSportDataEngine.ApplicationLogic.Entities.SystemAPI;
using System.Text.RegularExpressions;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.UnitOfWork;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;

namespace SuperSportDataEngine.ApplicationLogic.Services.Cms
{
    public class RugbyCmsService : IRugbyCmsService
    {
        private readonly IPublicSportDataUnitOfWork _publicSportDataUnitOfWork;
        IMapper iMapper;

        public RugbyCmsService(IPublicSportDataUnitOfWork publicSportDataUnitOfWork)
        {
            _publicSportDataUnitOfWork = publicSportDataUnitOfWork;
            
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
                                    _publicSportDataUnitOfWork.RugbyTournaments.Where(q => q.Name.Contains(query)
                                                        || q.NameCmsOverride.Contains(query)).OrderBy(tournament => tournament.Name), pageIndex, pageSize, abpath, query);
            }
            else
            {
                tournaments = await CreatePagedResults<RugbyTournament, RugbyTournamentEntity>(
                                            _publicSportDataUnitOfWork.RugbyTournaments.All().OrderBy(tournament => tournament.Name), pageIndex, pageSize, abpath, query);
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
                                    _publicSportDataUnitOfWork.RugbyFixtures.Where(q => q.TeamA.Name.Contains(query)
                                                        || q.TeamA.NameCmsOverride.Contains(query)
                                                        || q.TeamB.Name.Contains(query)
                                                        || q.TeamB.NameCmsOverride.Contains(query)), pageIndex, pageSize, abpath, query);
            }
            else
            {
                fixtures = await CreatePagedResults<RugbyFixture, RugbyFixtureEntity>(
                                    _publicSportDataUnitOfWork.RugbyFixtures.All(), pageIndex, pageSize, abpath, query);
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
                                    _publicSportDataUnitOfWork.RugbySeasons.Where(q => q.Name.Contains(query)), pageIndex, pageSize, abpath, query);
            }
            else
            {
                seasons = await CreatePagedResults<RugbySeason, RugbySeasonEntity>(
                                    _publicSportDataUnitOfWork.RugbySeasons.All(), pageIndex, pageSize, abpath, query);
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
                                    _publicSportDataUnitOfWork.RugbyTeams.Where(q => q.Name.Contains(query)
                                                        || q.NameCmsOverride.Contains(query)
                                                        || q.Abbreviation.Contains(query)), pageIndex, pageSize, abpath, query);
            }
            else
            {
                teams = await CreatePagedResults<RugbyTeam, RugbyTeamEntity>(
                                    _publicSportDataUnitOfWork.RugbyTeams.All(), pageIndex, pageSize, abpath, query);
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
                                    _publicSportDataUnitOfWork.RugbyPlayers.Where(q => q.FirstName.Contains(query)
                                                           || q.LastName.Contains(query)
                                                           || q.FullName.Contains(query)
                                                           || q.DisplayNameCmsOverride.Contains(query)), pageIndex, pageSize, abpath, query);
            }
            else
            {
                players = await CreatePagedResults<RugbyPlayer, RugbyPlayerEntity>(
                                    _publicSportDataUnitOfWork.RugbyPlayers.All(), pageIndex, pageSize, abpath, query);
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
                                    _publicSportDataUnitOfWork.RugbySeasons.Where(q => q.Name.Contains(query)
                                                                                    && q.RugbyTournament.Id == tournamentId), pageIndex, pageSize, abpath, query);
            }
            else
            {
                tournamentSeasons = await CreatePagedResults<RugbySeason, RugbySeasonEntity>(_publicSportDataUnitOfWork.RugbySeasons.Where(
                                                            season => season.RugbyTournament.Id == tournamentId), pageIndex, pageSize, abpath, query);
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
                                    _publicSportDataUnitOfWork.RugbyFixtures.Where(fixture => fixture.RugbySeason.Id == seasonId)
                                                        .Where(q => q.TeamA.Name.Contains(query)
                                                        || q.TeamA.NameCmsOverride.Contains(query)
                                                        || q.TeamB.Name.Contains(query)
                                                        || q.TeamB.NameCmsOverride.Contains(query)), pageIndex, pageSize, abpath, query);
            }
            else
            {
                tournamentSeasonFixtures = await CreatePagedResults<RugbyFixture, RugbyFixtureEntity>(_publicSportDataUnitOfWork.RugbyFixtures.Where(
                                                            fixture => fixture.RugbySeason.Id == seasonId), pageIndex, pageSize, abpath, query);
            }

            if (tournamentSeasonFixtures.Results.Any())
            {
                return tournamentSeasonFixtures;
            }
            return null;
        }

        public async Task<PagedResultsEntity<RugbyFixtureEntity>> GetTournamentFixtures(Guid tournamentId, Guid? seasonId, int pageIndex, int pageSize, string abpath, string query = null)
        {
            var tournamentFixtures = (PagedResultsEntity<RugbyFixtureEntity>)null;

            var tourFixtures = (await _publicSportDataUnitOfWork.RugbyFixtures.WhereAsync(t => t.RugbyTournament.Id == tournamentId)).OrderBy(f => f.StartDateTime).ToList();

            if (seasonId != null)
            {
                tourFixtures = tourFixtures.Where(fixture => fixture?.RugbySeason?.Id != null
                                                    && fixture?.RugbySeason?.Id == seasonId).ToList();
            }
            else
            {
                tourFixtures = tourFixtures.Where(fixture => fixture?.RugbySeason?.Id != null
                                                                && fixture?.RugbySeason?.IsCurrent == true).ToList();
            }

            if (!String.IsNullOrEmpty(query))
            {
                query = query.ToLower();
                tourFixtures = tourFixtures.Where(q => (q.TeamA.Name != null && q.TeamA.Name.ToLower().Contains(query))
                                                       || (q.TeamA.NameCmsOverride != null && q.TeamA.NameCmsOverride.ToLower().Contains(query))
                                                       || (q.TeamB.Name != null && q.TeamB.Name.ToLower().Contains(query))
                                                       || (q.TeamB.NameCmsOverride != null && q.TeamB.NameCmsOverride.ToLower().Contains(query))).ToList();
            }

            tournamentFixtures = await CreatePagedResults<RugbyFixture, RugbyFixtureEntity>(tourFixtures, pageIndex, pageSize, abpath, query);

            if (tournamentFixtures.Results.Any())
            {
                return tournamentFixtures;
            }
            return null;
        }

        public async Task<RugbyTournamentEntity> GetTournamentById(Guid id)
        {
            var rugbyTournament = await Task.FromResult(_publicSportDataUnitOfWork.RugbyTournaments.FirstOrDefault(
                                                        tournament => tournament.Id == id));

            if (rugbyTournament != null)
            {
                return iMapper.Map<RugbyTournament, RugbyTournamentEntity>(rugbyTournament);
            }

            return null;
        }

        public async Task<RugbyFixtureEntitySingle> GetFixtureById(Guid id)
        {
            var rugbyFixture = await Task.FromResult(_publicSportDataUnitOfWork.RugbyFixtures.FirstOrDefault(
                                                        fixture => fixture.Id == id));

            if (rugbyFixture != null)
            {
                return new RugbyFixtureEntitySingle
                {
                    Fixture = iMapper.Map<RugbyFixture, RugbyFixtureEntity>(rugbyFixture)
                };
            }

            return null;
        }

        public async Task<RugbySeasonEntity> GetSeasonById(Guid id)
        {
            var rugbySeason = await Task.FromResult(_publicSportDataUnitOfWork.RugbySeasons.FirstOrDefault(
                                                        season => season.Id == id));

            if (rugbySeason != null)
            {
                return iMapper.Map<RugbySeason, RugbySeasonEntity>(rugbySeason);
            }

            return null;
        }

        public async Task<RugbyTeamEntity> GetTeamById(Guid id)
        {
            var rugbyTeam = await Task.FromResult(_publicSportDataUnitOfWork.RugbyTeams.FirstOrDefault(
                                                        team => team.Id == id));

            if (rugbyTeam != null)
            {
                return iMapper.Map<RugbyTeam, RugbyTeamEntity>(rugbyTeam);
            }

            return null;
        }

        public async Task<RugbyPlayerEntity> GetPlayerById(Guid id)
        {
            var rugbyPlayer = await Task.FromResult(_publicSportDataUnitOfWork.RugbyPlayers.FirstOrDefault(
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
                var rugbyTournament = (await Task.FromResult(_publicSportDataUnitOfWork.RugbyTournaments.FirstOrDefault(
                                                            tournament => tournament.Id == id)));

                if (rugbyTournament != null)
                {
                    /** Only accept these values hard coded below **/
                    rugbyTournament.NameCmsOverride = !String.IsNullOrEmpty(rugbyTournamentEntity.NameCmsOverride?.Trim()) ? rugbyTournamentEntity.NameCmsOverride : null;
                    rugbyTournament.IsEnabled = rugbyTournamentEntity.IsEnabled;
                    rugbyTournament.IsLiveScored = rugbyTournamentEntity.IsLiveScored;
                    rugbyTournament.Slug = !String.IsNullOrEmpty(rugbyTournamentEntity.Slug?.Trim()) ? Regex.Replace(rugbyTournamentEntity.Slug, @"[^A-Za-z0-9_\.~]+", "-") : null;

                    _publicSportDataUnitOfWork.RugbyTournaments.Update(rugbyTournament);
                    await _publicSportDataUnitOfWork.SaveChangesAsync();
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
                var rugbyFixture = (await Task.FromResult(_publicSportDataUnitOfWork.RugbyFixtures.FirstOrDefault(
                                                            fixture => fixture.Id == id)));

                if (rugbyFixture != null)
                {
                    /** Only accept these values hard coded below **/
                    rugbyFixture.IsDisabledInbound = rugbyFixtureEntity.IsDisabledInbound;
                    rugbyFixture.IsDisabledOutbound = rugbyFixtureEntity.IsDisabledOutbound;
                    rugbyFixture.IsLiveScored = rugbyFixtureEntity.IsLiveScored;
                    rugbyFixture.CmsOverrideModeIsActive = rugbyFixtureEntity.CmsOverrideModeIsActive;

                    if (rugbyFixtureEntity.CmsOverrideModeIsActive == true)
                    {
                        rugbyFixture.TeamAScore = rugbyFixtureEntity.TeamAScore;
                        rugbyFixture.TeamBScore = rugbyFixtureEntity.TeamBScore;
                        rugbyFixture.StartDateTime = rugbyFixtureEntity.StartDateTime;
                        rugbyFixture.RugbyFixtureStatus = rugbyFixtureEntity.RugbyFixtureStatus;
                    }


                    _publicSportDataUnitOfWork.RugbyFixtures.Update(rugbyFixture);
                    await _publicSportDataUnitOfWork.SaveChangesAsync();
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
                var rugbySeason = (await Task.FromResult(_publicSportDataUnitOfWork.RugbySeasons.FirstOrDefault(
                                                            season => season.Id == id)));

                if (rugbySeason != null)
                {
                    /** Only accept these values hard coded below **/
                    rugbySeason.IsCurrent = rugbySeasonEntity.IsCurrent;
                    rugbySeason.CurrentRoundNumber = rugbySeasonEntity.CurrentRoundNumber;
                    rugbySeason.CurrentRoundNumberCmsOverride = rugbySeasonEntity.CurrentRoundNumberCmsOverride;

                    _publicSportDataUnitOfWork.RugbySeasons.Update(rugbySeason);
                    await _publicSportDataUnitOfWork.SaveChangesAsync();
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
                var rugbyTeam = (await Task.FromResult(_publicSportDataUnitOfWork.RugbyTeams.FirstOrDefault(
                                                            team => team.Id == id)));

                if (rugbyTeam != null)
                {
                    /** Only accept these values hard coded below **/
                    rugbyTeam.NameCmsOverride = !String.IsNullOrEmpty(rugbyteamEntity.NameCmsOverride?.Trim()) ? rugbyteamEntity.NameCmsOverride : null;

                    _publicSportDataUnitOfWork.RugbyTeams.Update(rugbyTeam);
                    await _publicSportDataUnitOfWork.SaveChangesAsync();
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
                var rugbyPlayer = (await Task.FromResult(_publicSportDataUnitOfWork.RugbyPlayers.FirstOrDefault(
                                                            player => player.Id == id)));

                if (rugbyPlayer != null)
                {
                    /** Only accept these values hard coded below **/
                    rugbyPlayer.DisplayNameCmsOverride = !String.IsNullOrEmpty(rugbyPlayerEntity.DisplayNameCmsOverride?.Trim()) ? rugbyPlayerEntity.DisplayNameCmsOverride : null;

                    _publicSportDataUnitOfWork.RugbyPlayers.Update(rugbyPlayer);
                    await _publicSportDataUnitOfWork.SaveChangesAsync();
                    success = true;
                }

            }
            return success;
        }

        protected async Task<PagedResultsEntity<TReturn>> CreatePagedResults<T, TReturn>(IEnumerable<T> queryable, int pageIndex, int pageSize, string abpath, string query)
        {
            var skipAmount = pageSize * (pageIndex - 1);

            var projection = queryable.Skip(skipAmount).Take(pageSize).ToList();
            var totalNumberOfRecords = await Task.FromResult(queryable.Count());

            var mod = totalNumberOfRecords % pageSize;
            var totalPageCount = (totalNumberOfRecords / pageSize) + (mod == 0 ? 0 : 1);

            var nextPageUrl = pageIndex == totalPageCount ? null : abpath + string.Format("?pageIndex={0}&pageSize={1}", pageIndex + 1, pageSize)
                                                    + (!String.IsNullOrEmpty(query) ? "&query=" + query : "");

            var results = iMapper.Map<IEnumerable<T>, IEnumerable<TReturn>>(projection);
            return new PagedResultsEntity<TReturn>
            {
                Results = results,
                PageNumber = pageIndex,
                PageSize = pageSize,
                TotalNumberOfPages = totalPageCount,
                TotalNumberOfRecords = totalNumberOfRecords,
                NextPageUrl = nextPageUrl
            };
        }
    }
}
