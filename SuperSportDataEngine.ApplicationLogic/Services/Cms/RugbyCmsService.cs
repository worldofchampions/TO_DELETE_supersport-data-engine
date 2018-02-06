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
        IMapper iMapper;

        public RugbyCmsService(IBaseEntityFrameworkRepository<RugbyTournament> rugbyTournamentRepository)
        {
            _rugbyTournamentRepository = rugbyTournamentRepository;

            var config = new MapperConfiguration(cfg => cfg.CreateMap<RugbyTournament, RugbyTournamentEntity>());
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
                var tournamentsEntity = iMapper.Map<IEnumerable<RugbyTournament>, IEnumerable<RugbyTournamentEntity>>(tournaments);

                return tournamentsEntity;
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
                var tournamentEntity = iMapper.Map<RugbyTournament, RugbyTournamentEntity>(rugbyTournament);
                return tournamentEntity;
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
    }
}
