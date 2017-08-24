using AutoMapper;
using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models;
using SuperSportDataEngine.ApplicationLogic.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SuperSportDataEngine.ApplicationLogic.Services
{
    public class RugbyService : IRugbyService
    {
        private readonly IBaseEntityFrameworkRepository<Log> _logRepository;
        private readonly IBaseEntityFrameworkRepository<SportTournament> _sportTournamentRepository;

        public RugbyService(
            IBaseEntityFrameworkRepository<Log> logRepository,
            IBaseEntityFrameworkRepository<SportTournament> sportTournamentRepository)
        {
            _logRepository = logRepository;
            _sportTournamentRepository = sportTournamentRepository;
        }

        public IEnumerable<LogEntity> GetLogs(string tournamentName)
        {
            var logs = _logRepository.All().Select(log => Mapper.Map<LogEntity>(log));

            return logs;
        }

        public Task<IEnumerable<SportTournament>> GetActiveTournaments()
        {
            return _sportTournamentRepository.WhereAsync(c => c.IsEnabled);
        }
    }
}