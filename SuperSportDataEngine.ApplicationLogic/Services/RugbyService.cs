using AutoMapper;
using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models.Enums;
using SuperSportDataEngine.ApplicationLogic.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace SuperSportDataEngine.ApplicationLogic.Services
{
    public class RugbyService : IRugbyService
    {
        private readonly IBaseEntityFrameworkRepository<Log> _logRepository;
        private readonly IBaseEntityFrameworkRepository<RugbyTournament> _rugbyTournamentRepository;
        private readonly IBaseEntityFrameworkRepository<RugbySeason> _rugbySeasonRepository;
        private readonly IBaseEntityFrameworkRepository<SchedulerTrackingRugbySeason> _schedulerTrackingRugbySeasonRepository; 

        public RugbyService(
            IBaseEntityFrameworkRepository<Log> logRepository,
            IBaseEntityFrameworkRepository<RugbyTournament> rugbyTournamentRepository,
            IBaseEntityFrameworkRepository<RugbySeason> rugbySeasonRepository,
            IBaseEntityFrameworkRepository<SchedulerTrackingRugbySeason> schedulerTrackingRugbySeasonRepository)
        {
            _logRepository = logRepository;
            _rugbyTournamentRepository = rugbyTournamentRepository;
            _rugbySeasonRepository = rugbySeasonRepository;
            _schedulerTrackingRugbySeasonRepository = schedulerTrackingRugbySeasonRepository;
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
            var tournamentsGuidsThatAreCurrent = _schedulerTrackingRugbySeasonRepository.Where(s => s.RugbySeasonStatus == RugbySeasonStatus.InProgress).Select(s => s.TournamentId);
            return _rugbyTournamentRepository.Where(t => t.IsEnabled && tournamentsGuidsThatAreCurrent.Contains(t.Id)).Select(t => t);
        }

        public SchedulerStateForManagerJobPolling GetSchedulerStateForManagerJobPolling(Guid tournamentId)
        {
            var season = 
                _schedulerTrackingRugbySeasonRepository
                    .Where(
                        s => s.TournamentId == tournamentId && 
                        ( s.RugbySeasonStatus == RugbySeasonStatus.InProgress ||
                          s.RugbySeasonStatus == RugbySeasonStatus.NotActive ))
                   .FirstOrDefault();

            return season != null ? season.SchedulerStateForManagerJobPolling : SchedulerStateForManagerJobPolling.Undefined;
        }

        public IEnumerable<RugbyTournament> GetEndedTournaments()
        {
            var tournamentGuids = 
                _schedulerTrackingRugbySeasonRepository
                    .Where(
                        season => season.RugbySeasonStatus == RugbySeasonStatus.Ended && 
                        season.SchedulerStateForManagerJobPolling == SchedulerStateForManagerJobPolling.Running)
                    .Select(s => s.TournamentId);

            return 
                _rugbyTournamentRepository
                    .Where(tournament => tournamentGuids.Contains(tournament.Id))
                    .Select(t => t);
        }

        public async Task SetSchedulerStatusPollingForTournamentToNotRunning(Guid tournamentId)
        {
            var season = _schedulerTrackingRugbySeasonRepository
                .Where(
                    s => s.TournamentId == tournamentId && s.SchedulerStateForManagerJobPolling == SchedulerStateForManagerJobPolling.Running)
                .FirstOrDefault();

            if (season != null)
            {
                season.SchedulerStateForManagerJobPolling = SchedulerStateForManagerJobPolling.NotRunning;
                await _schedulerTrackingRugbySeasonRepository.SaveAsync();
            }
        }

        public async Task SetSchedulerStatusPollingForTournamentToRunning(Guid tournamentId)
        {
            var season = _schedulerTrackingRugbySeasonRepository
                .Where(
                    s => 
                     s.RugbySeasonStatus == RugbySeasonStatus.InProgress &&
                     s.TournamentId == tournamentId && 
                     s.SchedulerStateForManagerJobPolling == SchedulerStateForManagerJobPolling.NotRunning)
                .FirstOrDefault();

            if (season != null)
            {
                season.SchedulerStateForManagerJobPolling = SchedulerStateForManagerJobPolling.Running;
                await _schedulerTrackingRugbySeasonRepository.SaveAsync();
            }
        }

        public IEnumerable<RugbyTournament> GetInactiveTournaments()
        {
            return 
                _rugbyTournamentRepository
                    .Where(t => t.IsEnabled == false);
        }
    }
}