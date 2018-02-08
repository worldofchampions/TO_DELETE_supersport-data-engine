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
        IMapper iMapper;

        public RugbyCmsService(
            IBaseEntityFrameworkRepository<RugbyTournament> rugbyTournamentRepository,
            IBaseEntityFrameworkRepository<RugbyFixture> rugbyFixtureRepository,
            IBaseEntityFrameworkRepository<RugbySeason> rugbySeasonRepository
            )
        {
            _rugbyTournamentRepository = rugbyTournamentRepository;
            _rugbyFixtureRepository = rugbyFixtureRepository;
            _rugbySeasonRepository = rugbySeasonRepository;

            var config = new MapperConfiguration(
                cfg => { cfg.CreateMap<RugbyTournament, RugbyTournamentEntity>();
                        cfg.CreateMap<RugbyFixture, RugbyFixtureEntity>();
                        cfg.CreateMap<RugbySeason, RugbySeasonEntity>();
                });
            iMapper = config.CreateMapper();
        }

        /// <summary>
        /// Fetches all tournaments
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<IEnumerable<RugbyTournamentEntity>> GetAllTournaments(int pageIndex, int pageSize)
        {
            var tournaments = await Task.FromResult(_rugbyTournamentRepository.All().Skip(pageIndex * pageSize).Take(pageSize));

            if (tournaments.Any())
            {
                return iMapper.Map<IEnumerable<RugbyTournament>, IEnumerable<RugbyTournamentEntity>>(tournaments);
            }
            return null;
        }

        /// <summary>
        /// Fetches all fixtures
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<IEnumerable<RugbyFixtureEntity>> GetAllFixtures(int pageIndex, int pageSize)
        {
            var fixtures = await Task.FromResult(_rugbyFixtureRepository.All().Skip(pageIndex * pageSize).Take(pageSize));

            if (fixtures.Any())
            {
                return iMapper.Map<IEnumerable<RugbyFixture>, IEnumerable<RugbyFixtureEntity>>(fixtures);
            }
            return null;
        }

        /// <summary>
        /// Fetches all Seasons
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<IEnumerable<RugbySeasonEntity>> GetAllSeasons(int pageIndex, int pageSize)
        {
            var seasons = await Task.FromResult(_rugbySeasonRepository.All().Skip(pageIndex * pageSize).Take(pageSize));

            if (seasons.Any())
            {
                return iMapper.Map<IEnumerable<RugbySeason>, IEnumerable<RugbySeasonEntity>>(seasons);
            }
            return null;
        }

        /// <summary>
        /// Fetches tournament details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<RugbyTournamentEntity> GetTournamentById(int id)
        {
            var rugbyTournament = await Task.FromResult(_rugbyTournamentRepository.FirstOrDefault(
                                                        tournament => tournament.ProviderTournamentId == id));

            if (rugbyTournament != null)
            {
                return iMapper.Map<RugbyTournament, RugbyTournamentEntity>(rugbyTournament);
            }

            return null;
        }

        /// <summary>
        /// Fetches fixture details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<RugbyFixtureEntity> GetFixtureById(int id)
        {
            var rugbyFixture = await Task.FromResult(_rugbyFixtureRepository.FirstOrDefault(
                                                        fixture => fixture.LegacyFixtureId == id));

            if (rugbyFixture != null)
            {
                return iMapper.Map<RugbyFixture, RugbyFixtureEntity>(rugbyFixture);
            }

            return null;
        }

        /// <summary>
        /// Fetches season details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<RugbySeasonEntity> GetSeasonById(int id)
        {
            var rugbySeason = await Task.FromResult(_rugbySeasonRepository.FirstOrDefault(
                                                        season => season.ProviderSeasonId == id));

            if (rugbySeason != null)
            {
                return iMapper.Map<RugbySeason, RugbySeasonEntity>(rugbySeason);
            }

            return null;
        }

        /// <summary>
        /// Updates tournament table and only accepts values => NameCmsOverride, IsEnabled, IsLiveScored, Slug
        /// </summary>
        /// <param name="tournamentId"></param>
        /// <param name="rugbyTournamentEntity"></param>
        /// <returns></returns>
        public async Task<bool> UpdateTournament(int tournamentId, RugbyTournamentEntity rugbyTournamentEntity)
        {
            var success = false;

            if (rugbyTournamentEntity != null)
            {
                var rugbyTournament = (await Task.FromResult(_rugbyTournamentRepository.FirstOrDefault(
                                                            tournament => tournament.ProviderTournamentId == tournamentId)));

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
        public async Task<bool> UpdateFixture(int fixtureId, RugbyFixtureEntity rugbyFixtureEntity)
        {
            var success = false;

            if (rugbyFixtureEntity != null)
            {
                var rugbyFixture= (await Task.FromResult(_rugbyFixtureRepository.FirstOrDefault(
                                                            fixture => fixture.LegacyFixtureId == fixtureId)));

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
        public async Task<bool> UpdateSeason(int seasonId, RugbySeasonEntity rugbySeasonEntity)
        {
            var success = false;

            if (rugbySeasonEntity != null)
            {
                var rugbySeason = (await Task.FromResult(_rugbySeasonRepository.FirstOrDefault(
                                                            season => season.ProviderSeasonId == seasonId)));

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
    }
}
