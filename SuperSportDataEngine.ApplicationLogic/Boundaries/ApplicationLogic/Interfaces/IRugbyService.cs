using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
using SuperSportDataEngine.ApplicationLogic.Entities;
using System.Collections.Generic;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models.Enums;
using System;
using System.Threading.Tasks;
using System.Threading;

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
        int GetCurrentProviderSeasonIdForTournament(Guid tournamentId);
        IEnumerable<RugbyFixture> GetLiveFixturesForCurrentTournament(Guid tournamentId);
        IEnumerable<RugbyFixture> GetEndedFixtures();
        bool HasFixtureEnded(long providerFixtureId);
        IEnumerable<RugbyTournament> GetActiveTournamentsForMatchesInResultsState();
        Task CleanupSchedulerTrackingTables(CancellationToken none);
    }
}