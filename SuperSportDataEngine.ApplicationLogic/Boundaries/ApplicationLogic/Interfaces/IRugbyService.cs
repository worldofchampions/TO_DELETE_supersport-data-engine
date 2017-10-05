using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
using System.Collections.Generic;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models.Enums;
using System;
using System.Threading.Tasks;
using System.Threading;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces
{
    public interface IRugbyService
    {
        Task<IEnumerable<RugbyFlatLog>> GetLogs(string tournamentSlug);
        Task<IEnumerable<RugbyTournament>> GetActiveTournaments();
        Task<IEnumerable<RugbyTournament>> GetInactiveTournaments();
        Task<IEnumerable<RugbyTournament>> GetCurrentTournaments();
        Task<SchedulerStateForManagerJobPolling> GetSchedulerStateForManagerJobPolling(Guid tournamentId);
        Task<IEnumerable<RugbyTournament>> GetEndedTournaments();
        Task<int> GetCurrentProviderSeasonIdForTournament(CancellationToken cancellationToken, Guid tournamentId);
        Task<IEnumerable<RugbyFixture>> GetLiveFixturesForCurrentTournament(CancellationToken cancellationToken, Guid tournamentId);
        Task<int> GetLiveFixturesCount(CancellationToken cancellationToken);
        Task<IEnumerable<RugbyFixture>> GetEndedFixtures();
        Task<bool> HasFixtureEnded(long providerFixtureId);
        Task<IEnumerable<RugbyTournament>> GetActiveTournamentsForMatchesInResultsState();
        Task CleanupSchedulerTrackingTables(CancellationToken none);
        Task<IEnumerable<RugbyFixture>> GetTournamentFixtures(Guid tournamentId, RugbyFixtureStatus fixtureStatus);
        Task<IEnumerable<RugbyFixture>> GetTournamentFixtures(string tournamentName);
        Task<IEnumerable<RugbyFixture>> GetTournamentResults(string tournamentSlug);
        Task<Guid> GetTournamentId(string tournamentSlug);
        Task<IEnumerable<RugbyFixture>> GetCurrentDayFixturesForActiveTournaments();
    }
}