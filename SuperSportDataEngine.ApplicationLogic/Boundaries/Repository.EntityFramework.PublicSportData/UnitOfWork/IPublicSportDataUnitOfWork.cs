using System;
using System.Threading.Tasks;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models.Tennis;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.UnitOfWork
{
    public interface IPublicSportDataUnitOfWork : IDisposable
    {
        IBaseEntityFrameworkRepository<RugbyCommentary> RugbyCommentaries { get; }
        IBaseEntityFrameworkRepository<RugbyEventType> RugbyEventTypes { get; }
        IBaseEntityFrameworkRepository<RugbyEventTypeProviderMapping> RugbyEventTypeProviderMappings { get; }
        IBaseEntityFrameworkRepository<RugbyFixture> RugbyFixtures { get; }
        IBaseEntityFrameworkRepository<RugbyFlatLog> RugbyFlatLogs { get; }
        IBaseEntityFrameworkRepository<RugbyGroupedLog> RugbyGroupedLogs { get; }
        IBaseEntityFrameworkRepository<RugbyLogGroup> RugbyLogGroups { get; }
        IBaseEntityFrameworkRepository<RugbyMatchEvent> RugbyMatchEvents { get; }
        IBaseEntityFrameworkRepository<RugbyMatchStatistics> RugbyMatchStatistics { get; }
        IBaseEntityFrameworkRepository<RugbyPlayer> RugbyPlayers { get; }
        IBaseEntityFrameworkRepository<RugbyPlayerStatistics> RugbyPlayerStatistics { get; }

        IBaseEntityFrameworkRepository<RugbyPlayerLineup> RugbyPlayerLineups { get; }
        IBaseEntityFrameworkRepository<RugbySeason> RugbySeasons { get; }
        IBaseEntityFrameworkRepository<RugbyTeam> RugbyTeams { get; }
        IBaseEntityFrameworkRepository<RugbyTournament> RugbyTournaments { get; }
        IBaseEntityFrameworkRepository<RugbyVenue> RugbyVenues { get; }

        IBaseEntityFrameworkRepository<MotorsportDriver> MotorsportDrivers { get; }
        IBaseEntityFrameworkRepository<MotorsportDriverStanding> MotorsportDriverStandings { get; }
        IBaseEntityFrameworkRepository<MotorsportRaceEventGrid> MotorsportRaceEventGrids { get; }
        IBaseEntityFrameworkRepository<MotorsportLeague> MotorsportLeagues { get; }
        IBaseEntityFrameworkRepository<MotorsportRace> MotorsportRaces { get; }
        IBaseEntityFrameworkRepository<MotorsportRaceEventResult> MotorsportRaceEventResults { get; }
        IBaseEntityFrameworkRepository<MotorsportSeason> MotorsportSeasons { get; }
        IBaseEntityFrameworkRepository<MotorsportTeam> MotortsportTeams { get; }
        IBaseEntityFrameworkRepository<MotorsportTeamStanding> MotorsportTeamStandings { get; }
        IBaseEntityFrameworkRepository<MotorsportRaceEvent> MotorsportRaceEvents { get; set; }

        // Tennis
        IBaseEntityFrameworkRepository<TennisLeague> TennisLeagues { get; set; }
        IBaseEntityFrameworkRepository<TennisTournament> TennisTournaments { get; set; }
        IBaseEntityFrameworkRepository<TennisSeason> TennisSeasons { get; set; }
        IBaseEntityFrameworkRepository<TennisSurfaceType> TennisSurfaceTypes { get; set; }
        IBaseEntityFrameworkRepository<TennisVenue> TennisVenues { get; set; }
        IBaseEntityFrameworkRepository<TennisPlayer> TennisPlayers { get; set; }
        IBaseEntityFrameworkRepository<TennisEvent> TennisEvents { get; set; }
        IBaseEntityFrameworkRepository<TennisEventTennisLeagues> TennisEventTennisLeagues { get; set; }
        IBaseEntityFrameworkRepository<TennisRanking> TennisRankings { get; set; }
        IBaseEntityFrameworkRepository<TennisMatch> TennisMatches { get; set; }
        IBaseEntityFrameworkRepository<TennisEventSeed> TennisEventSeeds { get; set; }
        IBaseEntityFrameworkRepository<TennisSide> TennisSides { get; set; }
        IBaseEntityFrameworkRepository<TennisSet> TennisSets { get; set; }
        IBaseEntityFrameworkRepository<TennisCountry> TennisCountries { get; set; }
        int SaveChanges();
        Task<int> SaveChangesAsync();
    }
}
