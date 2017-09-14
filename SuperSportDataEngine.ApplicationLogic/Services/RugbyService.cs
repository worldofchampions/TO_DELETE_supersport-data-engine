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
        private readonly IBaseEntityFrameworkRepository<RugbyTournament> _rugbyTournamentRepository;
        private readonly IBaseEntityFrameworkRepository<RugbySeason> _rugbySeasonRepository;

        public RugbyService(
            IBaseEntityFrameworkRepository<Log> logRepository,
            IBaseEntityFrameworkRepository<RugbyTournament> rugbyTournamentRepository,
            IBaseEntityFrameworkRepository<RugbySeason> rugbySeasonRepository)
        {
            _logRepository = logRepository;
            _rugbyTournamentRepository = rugbyTournamentRepository;
            _rugbySeasonRepository = rugbySeasonRepository;
        }

        public IEnumerable<LogEntity> GetLogs(string tournamentName)
        {
            var logs = _logRepository.All().Select(log => Mapper.Map<LogEntity>(log));

            return logs;
        }

        public IEnumerable<RugbyTournament> GetActiveTournaments()
        {
            return _rugbyTournamentRepository.Where(c => c.IsEnabled);
        }

        public IEnumerable<RugbyTournament> GetCurrentTournaments()
        {
            // TODO
            return _rugbyTournamentRepository.Where(c => c.IsEnabled);
        }
    }
}