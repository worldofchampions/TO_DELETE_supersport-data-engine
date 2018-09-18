using System.Threading;
using System.Threading.Tasks;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces
{
    public interface ITennisIngestWorkerService
    {
        Task IngestReferenceData(CancellationToken cancellationToken);
        Task IngestCalendarsForEnabledLeagues(CancellationToken cancellationToken);
        Task IngestCalendarForLeague(int providerLeagueId, CancellationToken cancellationToken);

        Task IngestRankingsForEnabledLeagues_Helper(CancellationToken cancellationToken);
        Task IngestRaceRankingsForEnabledLeagues(CancellationToken cancellationToken);
        Task IngestResultsForEnabledLeagues(CancellationToken cancellationToken);
        Task IngestHistoricCalendarsForEnabledLeagues(CancellationToken cancellationToken);
        Task IngestHistoricRankingsForEnabledLeagues(CancellationToken cancellationToken);
        Task IngestHistoricRaceRankingsForEnabledLeagues(CancellationToken cancellationToken);

        Task IngestHistoricResults(CancellationToken none);
        Task IngestResultsForEvent(string providerLeagueSlug, int leagueProviderLeagueId, CancellationToken none);
        Task IngestRankingsForLeague(string providerLeagueSlug, CancellationToken none);
        Task IngestRaceRankingsForLeague(string providerLeagueSlug, CancellationToken none);
        Task IngestResultsForMatch(string providerLeagueSlug, int tennisEventProviderEventId, int tennisMatchProviderMatchId, CancellationToken none);
    }
}
