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
        IEnumerable<RugbyFlatLog> GetLogs(string tournamentSlug);
        IEnumerable<RugbyTournament> GetActiveTournaments();
        IEnumerable<RugbyTournament> GetInactiveTournaments();
        IEnumerable<RugbyTournament> GetCurrentTournaments();
        SchedulerStateForManagerJobPolling GetSchedulerStateForManagerJobPolling(Guid tournamentId);
        IEnumerable<RugbyTournament> GetEndedTournaments();
        int GetCurrentProviderSeasonIdForTournament(Guid tournamentId);
        IEnumerable<RugbyFixture> GetLiveFixturesForCurrentTournament(Guid tournamentId);
        int GetLiveFixturesCount();
        IEnumerable<RugbyFixture> GetEndedFixtures();
        bool HasFixtureEnded(long providerFixtureId);
        IEnumerable<RugbyTournament> GetActiveTournamentsForMatchesInResultsState();
        Task CleanupSchedulerTrackingTables(CancellationToken none);
        IEnumerable<RugbyFixture> GetTournamentFixtures(Guid tournamentId, RugbyFixtureStatus fixtureStatus);
        IEnumerable<RugbyFixture> GetTournamentFixtures(string tournamentName);
        IEnumerable<RugbyFixture> GetTournamentResults(string tournamentSlug);
        Guid GetTournamentId(string tournamentSlug);
    }
}