using AutoMapper;
using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
using SuperSportDataEngine.ApplicationLogic.Entities;
using SuperSportDataEngine.Common.Boundaries;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models;
using System.Linq;

namespace SuperSportDataEngine.ApplicationLogic.Services
{
    public class RugbyService : IRugbyService
    {
        private readonly ICache _cache;
        private readonly IBaseEntityFrameworkRepository<Log> _logRepository;
        private readonly IBaseEntityFrameworkRepository<SportTournament> _sportTournamentRepository;

        public RugbyService(ICache cache,
            IBaseEntityFrameworkRepository<Log> logRepository,
            IBaseEntityFrameworkRepository<SportTournament> sportTournamentRepository)
        {
            _cache = cache;
            _logRepository = logRepository;
            _sportTournamentRepository = sportTournamentRepository;
        }

        public async Task<IEnumerable<LogEntity>> GetLogs(string tournamentName)
        {
            var cacheKey = $"rugby/{tournamentName}/logs";
            var logs = await _cache.GetAsync<IEnumerable<LogEntity>>(cacheKey);

            if (logs == null)
            {
                //TODO: Get data from the DB
                logs = _logRepository.All().Select(log => Mapper.Map<LogEntity>(log));

                _cache.Add(cacheKey, logs);
            }
            return logs;
        }

        public Task<IEnumerable<SportTournament>> GetActiveTournaments()
        {
            return _sportTournamentRepository.WhereAsync(c => c.IsEnabled);
        }
    }
}