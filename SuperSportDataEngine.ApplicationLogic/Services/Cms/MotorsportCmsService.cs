using AutoMapper;
using SuperSportDataEngine.ApplicationLogic.Boundaries.CmsLogic.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.UnitOfWork;
using SuperSportDataEngine.ApplicationLogic.Entities.SystemAPI;
using SuperSportDataEngine.ApplicationLogic.Entities.SystemAPI.Motorsport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SuperSportDataEngine.ApplicationLogic.Services.Cms
{
    public class MotorsportCmsService : IMotorsportCmsService
    {
        private readonly IPublicSportDataUnitOfWork _publicSportDataUnitOfWork;
        IMapper iMapper;

        public MotorsportCmsService(IPublicSportDataUnitOfWork publicSportDataUnitOfWork)
        {
            _publicSportDataUnitOfWork = publicSportDataUnitOfWork;

            var config = new MapperConfiguration(
                cfg =>
                {
                    cfg.CreateMap<MotorsportLeague, MotorsportLeagueEntity>();
                    cfg.CreateMap<MotorsportSeason, MotorsportSeasonEntity>();
                    cfg.CreateMap<MotorsportTeam, MotorsportTeamEntity>();
                    cfg.CreateMap<MotorsportRace, MotorsportRaceEntity>();
                    cfg.CreateMap<MotorsportDriver, MotorsportDriverEntity>();
                });
            iMapper = config.CreateMapper();
        }

        public async Task<PagedResultsEntity<MotorsportLeagueEntity>> GetAllLeagues(int pageIndex, int pageSize, string abpath, string query = null)
        {
            var leagues = (PagedResultsEntity<MotorsportLeagueEntity>)null;

            if (!String.IsNullOrEmpty(query))
            {
                leagues = await CreatePagedResults<MotorsportLeague, MotorsportLeagueEntity>(
                                    _publicSportDataUnitOfWork.MotorsportLeagues.Where(q => q.IsEnabled && q.Name.Contains(query)
                                                        || q.NameCmsOverride.Contains(query)).OrderBy(league => league.Slug), pageIndex, pageSize, abpath, query);
            }
            else
            {
                leagues = await CreatePagedResults<MotorsportLeague, MotorsportLeagueEntity>(
                                            _publicSportDataUnitOfWork.MotorsportLeagues.Where(cond => cond.IsEnabled).OrderBy(league => league.Slug), pageIndex, pageSize, abpath, query);
            }

            if (leagues.Results.Any())
            {
                return leagues;
            }
            return null;
        }

        public async Task<PagedResultsEntity<MotorsportTeamEntity>> GetAllTeams(int pageIndex, int pageSize, string abpath, string query = null)
        {
            var teams = (PagedResultsEntity<MotorsportTeamEntity>)null;

            if (!String.IsNullOrEmpty(query))
            {
                teams = await CreatePagedResults<MotorsportTeam, MotorsportTeamEntity>(
                                    _publicSportDataUnitOfWork.MotortsportTeams.Where(q => q.Name.Contains(query)
                                                        || q.NameCmsOverride.Contains(query))
                                                        .OrderBy(field => field.Name), pageIndex, pageSize, abpath, query);
            }
            else
            {
                teams = await CreatePagedResults<MotorsportTeam, MotorsportTeamEntity>(
                                    _publicSportDataUnitOfWork.MotortsportTeams.All()
                                                            .OrderBy(field => field.Name), pageIndex, pageSize, abpath, query);
            }

            if (teams.Results.Any())
            {
                return teams;
            }
            return null;
        }

        public async Task<PagedResultsEntity<MotorsportSeasonEntity>> GetSeasonsForLeague(Guid leagueId, int pageIndex, int pageSize, string abpath, string query = null)
        {
            var leagueSeasons = (PagedResultsEntity<MotorsportSeasonEntity>)null;

            if (!String.IsNullOrEmpty(query))
            {
                leagueSeasons = await CreatePagedResults<MotorsportSeason, MotorsportSeasonEntity>(
                                    _publicSportDataUnitOfWork.MotorsportSeasons.Where(q => q.MotorsportLeague.Id == leagueId && q.Name.Contains(query))
                                                                                    .OrderByDescending(field => field.ProviderSeasonId), pageIndex, pageSize, abpath, query);
            }
            else
            {
                leagueSeasons = await CreatePagedResults<MotorsportSeason, MotorsportSeasonEntity>(_publicSportDataUnitOfWork.MotorsportSeasons.Where(
                                                            season => season.MotorsportLeague.Id == leagueId)
                                                            .OrderByDescending(field => field.ProviderSeasonId), pageIndex, pageSize, abpath, query);
            }

            if (leagueSeasons.Results.Any())
            {
                return leagueSeasons;
            }
            return null;
        }

        public async Task<PagedResultsEntity<MotorsportRaceEntity>> GetRacesForLeague(Guid leagueId, int pageIndex, int pageSize, string abpath, string query = null)
        {
            var leagueRaces = (PagedResultsEntity<MotorsportRaceEntity>)null;

            if (!String.IsNullOrEmpty(query))
            {
                leagueRaces = await CreatePagedResults<MotorsportRace, MotorsportRaceEntity>(
                                    _publicSportDataUnitOfWork.MotorsportRaces.Where(q => q.MotorsportLeague.Id == leagueId
                                                                                    && q.RaceName.Contains(query)
                                                                                    || q.RaceNameCmsOverride.Contains(query)
                                                                                    || q.RaceNameAbbreviation.Contains(query)
                                                                                    || q.RaceNameAbbreviationCmsOverride.Contains(query))
                                                                                    .OrderBy(field => field.RaceName), pageIndex, pageSize, abpath, query);
            }
            else
            {
                leagueRaces = await CreatePagedResults<MotorsportRace, MotorsportRaceEntity>(_publicSportDataUnitOfWork.MotorsportRaces.Where(
                                                            race => race.MotorsportLeague.Id == leagueId)
                                                            .OrderBy(field => field.RaceName), pageIndex, pageSize, abpath, query);
            }

            if (leagueRaces.Results.Any())
            {
                return leagueRaces;
            }
            return null;
        }

        public async Task<PagedResultsEntity<MotorsportDriverEntity>> GetDriversForLeague(Guid leagueId, int pageIndex, int pageSize, string abpath, string query = null)
        {
            var leagueDrivers = (PagedResultsEntity<MotorsportDriverEntity>)null;

            if (!String.IsNullOrEmpty(query))
            {
                leagueDrivers = await CreatePagedResults<MotorsportDriver, MotorsportDriverEntity>(
                                    _publicSportDataUnitOfWork.MotorsportDrivers.Where(q => q.MotorsportLeague.Id == leagueId
                                                                                    && q.FirstName.Contains(query)
                                                                                    || q.LastName.Contains(query)
                                                                                    || q.FullNameCmsOverride.Contains(query))
                                                                                    .OrderBy(field => field.FirstName), pageIndex, pageSize, abpath, query);
            }
            else
            {
                leagueDrivers = await CreatePagedResults<MotorsportDriver, MotorsportDriverEntity>(_publicSportDataUnitOfWork.MotorsportDrivers.Where(
                                                            race => race.MotorsportLeague.Id == leagueId)
                                                            .OrderBy(field => field.FirstName), pageIndex, pageSize, abpath, query);
            }

            if (leagueDrivers.Results.Any())
            {
                return leagueDrivers;
            }
            return null;
        }

        public async Task<MotorsportLeagueEntity> GetLeagueById(Guid id)
        {
            var motorsportLeague = await Task.FromResult(_publicSportDataUnitOfWork.MotorsportLeagues.FirstOrDefault(
                                                        league => league.Id == id));

            if (motorsportLeague != null)
            {
                return iMapper.Map<MotorsportLeague, MotorsportLeagueEntity>(motorsportLeague);
            }

            return null;
        }

        public async Task<MotorsportSeasonEntity> GetSeasonById(Guid id)
        {
            var motorsportSeason = await Task.FromResult(_publicSportDataUnitOfWork.MotorsportSeasons.FirstOrDefault(
                                                        season => season.Id == id));

            if (motorsportSeason != null)
            {
                return iMapper.Map<MotorsportSeason, MotorsportSeasonEntity>(motorsportSeason);
            }

            return null;
        }

        public async Task<MotorsportTeamEntity> GetTeamById(Guid id)
        {
            var motorsportTeam = await Task.FromResult(_publicSportDataUnitOfWork.MotortsportTeams.FirstOrDefault(
                                                        team => team.Id == id));

            if (motorsportTeam != null)
            {
                return iMapper.Map<MotorsportTeam, MotorsportTeamEntity>(motorsportTeam);
            }

            return null;
        }

        public async Task<MotorsportRaceEntity> GetRaceById(Guid id)
        {
            var motorsportRace = await Task.FromResult(_publicSportDataUnitOfWork.MotorsportRaces.FirstOrDefault(
                                                        race => race.Id == id));

            if (motorsportRace != null)
            {
                return iMapper.Map<MotorsportRace, MotorsportRaceEntity>(motorsportRace);
            }

            return null;
        }

        public async Task<MotorsportDriverEntity> GetDriverById(Guid id)
        {
            var motorsportDriver = await Task.FromResult(_publicSportDataUnitOfWork.MotorsportDrivers.FirstOrDefault(
                                                        driver => driver.Id == id));

            if (motorsportDriver != null)
            {
                return iMapper.Map<MotorsportDriver, MotorsportDriverEntity>(motorsportDriver);
            }

            return null;
        }

        /// <summary>
        /// Updates league table and only accepts this value => NameCmsOverride
        /// </summary>
        /// <param name="leagueId"></param>
        /// <param name="motorsportLeagueEntity"></param>
        /// <returns></returns>
        public async Task<bool> UpdateLeague(Guid id, MotorsportLeagueEntity motorsportLeagueEntity)
        {
            var success = false;

            if (motorsportLeagueEntity != null)
            {
                var motorsportLeague = (await Task.FromResult(_publicSportDataUnitOfWork.MotorsportLeagues.FirstOrDefault(
                                                            league => league.Id == id)));

                if (motorsportLeague != null)
                {
                    motorsportLeague.NameCmsOverride = !String.IsNullOrEmpty(motorsportLeagueEntity.NameCmsOverride?.Trim()) ? motorsportLeagueEntity.NameCmsOverride : null;

                    _publicSportDataUnitOfWork.MotorsportLeagues.Update(motorsportLeague);
                    await _publicSportDataUnitOfWork.SaveChangesAsync();
                    success = true;
                }

            }
            return success;
        }

        /// <summary>
        /// Updates season table and only accepts IsCurrent value
        /// </summary>
        /// <param name="seasonId"></param>
        /// <param name="leagueId"></param>
        /// <param name="motorsportSeasonEntity"></param>
        /// <returns></returns>
        public async Task<bool> UpdateSeason(Guid id, Guid leagueId, MotorsportSeasonEntity motorsportSeasonEntity)
        {
            var success = false;
            var _season = _publicSportDataUnitOfWork.MotorsportSeasons.FirstOrDefault(season => season.Id == id);

            if (leagueId != null && _season != null && motorsportSeasonEntity != null)
            {
                var seasons = _publicSportDataUnitOfWork.MotorsportSeasons.Where(season => season.MotorsportLeague.Id == leagueId);

                foreach (var season in seasons)
                {
                    if (season.Id == id)
                    {
                        season.IsCurrent = motorsportSeasonEntity.IsCurrent;
                    }

                    if (season.Id != id && motorsportSeasonEntity.IsCurrent == true)
                    {
                        season.IsCurrent = false;
                    }

                    _publicSportDataUnitOfWork.MotorsportSeasons.Update(season);
                }

                await _publicSportDataUnitOfWork.SaveChangesAsync();
                success = true;
            }
            return success;
        }

        /// <summary>
        /// Updates motorsport teams table and only accepts values => NameCmsOverride
        /// </summary>
        /// <param name="teamId"></param>
        /// <param name="rugbyteamEntity"></param>
        /// <returns></returns>
        public async Task<bool> UpdateTeam(Guid id, MotorsportTeamEntity motorsportteamEntity)
        {
            var success = false;

            if (motorsportteamEntity != null)
            {
                var motorsportTeam = (await Task.FromResult(_publicSportDataUnitOfWork.MotortsportTeams.FirstOrDefault(
                                                            team => team.Id == id)));

                if (motorsportTeam != null)
                {
                    motorsportTeam.NameCmsOverride = !String.IsNullOrEmpty(motorsportteamEntity.NameCmsOverride?.Trim()) ? motorsportteamEntity.NameCmsOverride : null;

                    _publicSportDataUnitOfWork.MotortsportTeams.Update(motorsportTeam);
                    await _publicSportDataUnitOfWork.SaveChangesAsync();
                    success = true;
                }
            }
            return success;
        }

        /// <summary>
        /// Updates motorsport races table and only accepts values => NameCmsOverride
        /// </summary>
        /// <param name="raceId"></param>
        /// <param name="motorsportRaceEntity"></param>
        /// <returns></returns>
        public async Task<bool> UpdateRace(Guid id, MotorsportRaceEntity motorsportRaceEntity)
        {
            var success = false;

            if (motorsportRaceEntity != null)
            {
                var motorsportRace = (await Task.FromResult(_publicSportDataUnitOfWork.MotorsportRaces.FirstOrDefault(
                                                            race => race.Id == id)));

                if (motorsportRace != null)
                {
                    motorsportRace.RaceNameCmsOverride = !String.IsNullOrEmpty(motorsportRaceEntity.RaceNameCmsOverride?.Trim()) ? motorsportRaceEntity.RaceNameCmsOverride : null;
                    motorsportRace.RaceNameAbbreviationCmsOverride = !String.IsNullOrEmpty(motorsportRaceEntity.RaceNameAbbreviationCmsOverride?.Trim()) ? motorsportRaceEntity.RaceNameAbbreviationCmsOverride : null;
                    motorsportRace.IsDisabledInbound = motorsportRaceEntity.IsDisabledInbound;

                    _publicSportDataUnitOfWork.MotorsportRaces.Update(motorsportRace);
                    await _publicSportDataUnitOfWork.SaveChangesAsync();
                    success = true;
                }
            }
            return success;
        }

        /// <summary>
        /// Updates motorsport drivers table and only accepts values => FullNameCmsOverride
        /// </summary>
        /// <param name="driverId"></param>
        /// <param name="motorsportDriverEntity"></param>
        /// <returns></returns>
        public async Task<bool> UpdateDriver(Guid id, MotorsportDriverEntity motorsportDriverEntity)
        {
            var success = false;

            if (motorsportDriverEntity != null)
            {
                var motorsportDriver = (await Task.FromResult(_publicSportDataUnitOfWork.MotorsportDrivers.FirstOrDefault(
                                                            driver => driver.Id == id)));

                if (motorsportDriver != null)
                {
                    motorsportDriver.FullNameCmsOverride = !String.IsNullOrEmpty(motorsportDriverEntity.FullNameCmsOverride?.Trim()) ? motorsportDriverEntity.FullNameCmsOverride : null;

                    _publicSportDataUnitOfWork.MotorsportDrivers.Update(motorsportDriver);
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
