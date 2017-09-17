using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
using SuperSportDataEngine.ApplicationLogic.Entities;
using System.Collections.Generic;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models.Enums;
using System;
using System.Threading.Tasks;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces
{
    public interface IRugbyService
    {
        IEnumerable<LogEntity> GetLogs(string tournamentName);
        IEnumerable<RugbyTournament> GetActiveTournaments();
        IEnumerable<RugbyTournament> GetInactiveTournaments();
        IEnumerable<RugbyTournament> GetCurrentTournaments();
        SchedulerStateForManagerJobPolling GetSchedulerStateForManagerJobPolling(Guid tournamentId);
        IEnumerable<RugbyTournament> GetEndedTournaments();
        Task SetSchedulerStatusPollingForTournamentToNotRunning(Guid tournamentId);
        Task SetSchedulerStatusPollingForTournamentToRunning(Guid tournamentId);
        int GetCurrentProviderSeasonIdForTournament(Guid tournamentId);
        IEnumerable<RugbyFixture> GetLiveFixturesForCurrentTournament(Guid tournamentId);
        Task SetSchedulerStatusPollingForFixtureToLivePolling(Guid fixtureId);
    }
}