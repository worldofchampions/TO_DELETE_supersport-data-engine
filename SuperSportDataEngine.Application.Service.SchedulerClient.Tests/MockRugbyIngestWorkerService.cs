using SuperSportDataEngine.ApplicationLogic.Services;
using System.Threading;
using System.Threading.Tasks;

namespace SuperSportDataEngine.Application.Service.SchedulerClient.Tests
{
    public partial class FixedScheduledJob_Test
    {
        public class MockRugbyIngestWorkerService : IRugbyIngestWorkerService
        {
            public Task IngestFixturesForActiveTournaments(CancellationToken cancellationToken)
            {
                throw new System.NotImplementedException();
            }

            public Task IngestFixturesForTournamentSeason(CancellationToken cancellationToken, int tournamentId, int seasonId)
            {
                throw new System.NotImplementedException();
            }

            public Task IngestLogsForActiveTournaments(CancellationToken cancellationToken)
            {
                throw new System.NotImplementedException();
            }

            public Task IngestLogsForCurrentTournaments(CancellationToken cancellationToken)
            {
                throw new System.NotImplementedException();
            }

            public Task IngestLogsForTournamentSeason(CancellationToken cancellationToken, int providerTournamentId, int seasonId)
            {
                throw new System.NotImplementedException();
            }

            public Task IngestMatchStatsForFixture(CancellationToken cancellationToken, long providerFixtureId)
            {
                throw new System.NotImplementedException();
            }

            public Task IngestOneMonthsFixturesForTournament(CancellationToken cancellationToken, int providerTournamentId)
            {
                throw new System.NotImplementedException();
            }

            public Task IngestReferenceData(CancellationToken cancellationToken)
            {
                throw new System.NotImplementedException();
            }

            public Task IngestResultsForAllFixtures(CancellationToken cancellationToken)
            {
                throw new System.NotImplementedException();
            }

            public Task IngestResultsForCurrentDayFixtures(CancellationToken cancellationToken)
            {
                throw new System.NotImplementedException();
            }

            public Task IngestResultsForFixturesInResultsState(CancellationToken cancellationToken)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}
