using System.Collections.Generic;
using System.Threading.Tasks;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models.Tennis;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.UnitOfWork;

namespace SuperSportDataEngine.Tests.Common.Repositories.Test
{
    public class TestPublicSportDataUnitOfWork : IPublicSportDataUnitOfWork
    {
        public IBaseEntityFrameworkRepository<RugbyCommentary> RugbyCommentaries { get; set; }
        public IBaseEntityFrameworkRepository<RugbyEventType> RugbyEventTypes { get; set; }
        public IBaseEntityFrameworkRepository<RugbyEventTypeProviderMapping> RugbyEventTypeProviderMappings { get; set; }
        public IBaseEntityFrameworkRepository<RugbyFixture> RugbyFixtures { get; set; }
        public IBaseEntityFrameworkRepository<RugbyFlatLog> RugbyFlatLogs { get; set; }
        public IBaseEntityFrameworkRepository<RugbyGroupedLog> RugbyGroupedLogs { get; set; }
        public IBaseEntityFrameworkRepository<RugbyLogGroup> RugbyLogGroups { get; set; }
        public IBaseEntityFrameworkRepository<RugbyMatchEvent> RugbyMatchEvents { get; set; }
        public IBaseEntityFrameworkRepository<RugbyMatchStatistics> RugbyMatchStatistics { get; set; }
        public IBaseEntityFrameworkRepository<RugbyPlayer> RugbyPlayers { get; set; }
        public IBaseEntityFrameworkRepository<RugbyPlayerStatistics> RugbyPlayerStatistics { get; set; }
        public IBaseEntityFrameworkRepository<RugbyPlayerLineup> RugbyPlayerLineups { get; set; }
        public IBaseEntityFrameworkRepository<RugbySeason> RugbySeasons { get; set; }
        public IBaseEntityFrameworkRepository<RugbyTeam> RugbyTeams { get; set; }
        public IBaseEntityFrameworkRepository<RugbyTournament> RugbyTournaments { get; set; }
        public IBaseEntityFrameworkRepository<RugbyVenue> RugbyVenues { get; set; }
        public IBaseEntityFrameworkRepository<MotorsportDriver> MotorsportDrivers { get; set; }
        public IBaseEntityFrameworkRepository<MotorsportLeague> MotorsportLeagues { get; set; }
        public IBaseEntityFrameworkRepository<MotorsportRace> MotorsportRaces { get; set; }
        public IBaseEntityFrameworkRepository<MotorsportRaceEventResult> MotorsportRaceEventResults { get; set; }
        public IBaseEntityFrameworkRepository<MotorsportSeason> MotorsportSeasons { get; set; }
        public IBaseEntityFrameworkRepository<MotorsportDriverStanding> MotorsportDriverStandings { get; set; }
        public IBaseEntityFrameworkRepository<MotorsportTeamStanding> MotorsportTeamStandings { get; set; }
        public IBaseEntityFrameworkRepository<MotorsportRaceEvent> MotorsportRaceEvents { get; set; }
        public IBaseEntityFrameworkRepository<MotorsportTeam> MotortsportTeams { get; set; }
        public IBaseEntityFrameworkRepository<MotorsportRaceEventResult> MotorTeamResults { get; set; }
        public IBaseEntityFrameworkRepository<MotorsportRaceEventGrid> MotorsportRaceEventGrids { get; set; }

        public IBaseEntityFrameworkRepository<TennisLeague> TennisLeagues { get; set; }
        public IBaseEntityFrameworkRepository<TennisTournament> TennisTournaments { get; set; }
        public IBaseEntityFrameworkRepository<TennisSeason> TennisSeasons { get; set; }
        public IBaseEntityFrameworkRepository<TennisSurfaceType> TennisSurfaceTypes { get; set; }
        public IBaseEntityFrameworkRepository<TennisVenue> TennisVenues { get; set; }
        public IBaseEntityFrameworkRepository<TennisPlayer> TennisPlayers { get; set; }
        public IBaseEntityFrameworkRepository<TennisEvent> TennisEvents { get; set; }
        public IBaseEntityFrameworkRepository<TennisEventTennisLeagues> TennisEventTennisLeagues { get; set; }
        public IBaseEntityFrameworkRepository<TennisRanking> TennisRankings { get; set; }
        public IBaseEntityFrameworkRepository<TennisMatch> TennisMatches { get; set; }
        public IBaseEntityFrameworkRepository<TennisEventSeed> TennisEventSeeds { get; set; }
        public IBaseEntityFrameworkRepository<TennisSide> TennisSides { get; set; }
        public IBaseEntityFrameworkRepository<TennisSet> TennisSets { get; set; }
        public IBaseEntityFrameworkRepository<TennisCountry> TennisCountries { get; set; }

        public TestPublicSportDataUnitOfWork()
        {
            RugbyCommentaries = new TestEntityFrameworkRepository<RugbyCommentary>(new List<RugbyCommentary>());
            RugbyTournaments = new TestEntityFrameworkRepository<RugbyTournament>(new List<RugbyTournament>());
            RugbyEventTypeProviderMappings = new TestEntityFrameworkRepository<RugbyEventTypeProviderMapping>(new List<RugbyEventTypeProviderMapping>());
            RugbyEventTypes = new TestEntityFrameworkRepository<RugbyEventType>(new List<RugbyEventType>());
            RugbyFixtures = new TestEntityFrameworkRepository<RugbyFixture>(new List<RugbyFixture>());
            RugbyFlatLogs = new TestEntityFrameworkRepository<RugbyFlatLog>(new List<RugbyFlatLog>());
            RugbyGroupedLogs = new TestEntityFrameworkRepository<RugbyGroupedLog>(new List<RugbyGroupedLog>());
            RugbyLogGroups = new TestEntityFrameworkRepository<RugbyLogGroup>(new List<RugbyLogGroup>());
            RugbyMatchEvents = new TestEntityFrameworkRepository<RugbyMatchEvent>(new List<RugbyMatchEvent>());
            RugbyMatchStatistics = new TestEntityFrameworkRepository<RugbyMatchStatistics>(new List<RugbyMatchStatistics>());
            RugbyPlayerLineups = new TestEntityFrameworkRepository<RugbyPlayerLineup>(new List<RugbyPlayerLineup>());
            RugbyPlayers = new TestEntityFrameworkRepository<RugbyPlayer>(new List<RugbyPlayer>());
            RugbyPlayerStatistics = new TestEntityFrameworkRepository<RugbyPlayerStatistics>(new List<RugbyPlayerStatistics>());
            RugbySeasons = new TestEntityFrameworkRepository<RugbySeason>(new List<RugbySeason>());
            RugbyTeams = new TestEntityFrameworkRepository<RugbyTeam>(new List<RugbyTeam>());
            RugbyTournaments = new TestEntityFrameworkRepository<RugbyTournament>(new List<RugbyTournament>());
            RugbyVenues = new TestEntityFrameworkRepository<RugbyVenue>(new List<RugbyVenue>());
            TennisLeagues = new TestEntityFrameworkRepository<TennisLeague>(new List<TennisLeague>());
            TennisTournaments = new TestEntityFrameworkRepository<TennisTournament>(new List<TennisTournament>());
            TennisSeasons = new TestEntityFrameworkRepository<TennisSeason>(new List<TennisSeason>());
            TennisSurfaceTypes = new TestEntityFrameworkRepository<TennisSurfaceType>(new List<TennisSurfaceType>());
            TennisVenues = new TestEntityFrameworkRepository<TennisVenue>(new List<TennisVenue>());
            TennisPlayers = new TestEntityFrameworkRepository<TennisPlayer>(new List<TennisPlayer>());
            TennisEvents = new TestEntityFrameworkRepository<TennisEvent>(new List<TennisEvent>());
            TennisEventTennisLeagues = new TestEntityFrameworkRepository<TennisEventTennisLeagues>(new List<TennisEventTennisLeagues>());
            TennisRankings = new TestEntityFrameworkRepository<TennisRanking>(new List<TennisRanking>());
            TennisMatches = new TestEntityFrameworkRepository<TennisMatch>(new List<TennisMatch>());
            TennisEventSeeds = new TestEntityFrameworkRepository<TennisEventSeed>(new List<TennisEventSeed>());
            TennisSides = new TestEntityFrameworkRepository<TennisSide>(new List<TennisSide>());
            TennisSets = new TestEntityFrameworkRepository<TennisSet>(new List<TennisSet>());
            TennisCountries = new TestEntityFrameworkRepository<TennisCountry>(new List<TennisCountry>());
        }

        public int SaveChanges()
        {
            return 1;
        }

        public Task<int> SaveChangesAsync()
        {
            return Task.FromResult(1);
        }

        public void Dispose()
        {
        }
    }
}
