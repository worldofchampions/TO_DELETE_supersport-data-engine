using System.Collections.Generic;
using System.Threading.Tasks;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
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
        public IBaseEntityFrameworkRepository<MotorsportRaceEventGrid> MotorsportRaceEventGrids { get; set; }

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

            //Motorsport
            MotorsportLeagues = new TestEntityFrameworkRepository<MotorsportLeague>(new List<MotorsportLeague>());
            MotorsportRaces = new TestEntityFrameworkRepository<MotorsportRace>(new List<MotorsportRace>());
            MotorsportSeasons = new TestEntityFrameworkRepository<MotorsportSeason>(new List<MotorsportSeason>());
            MotorsportDrivers = new TestEntityFrameworkRepository<MotorsportDriver>(new List<MotorsportDriver>());
            MotortsportTeams = new TestEntityFrameworkRepository<MotorsportTeam>(new List<MotorsportTeam>());
            MotorsportRaces = new TestEntityFrameworkRepository<MotorsportRace>(new List<MotorsportRace>());
            MotorsportRaceEventGrids = new TestEntityFrameworkRepository<MotorsportRaceEventGrid>(new List<MotorsportRaceEventGrid>());
            MotorsportDriverStandings = new TestEntityFrameworkRepository<MotorsportDriverStanding>(new List<MotorsportDriverStanding>());
            MotorsportTeamStandings = new TestEntityFrameworkRepository<MotorsportTeamStanding>(new List<MotorsportTeamStanding>());
            MotorsportRaceEventResults = new TestEntityFrameworkRepository<MotorsportRaceEventResult>(new List<MotorsportRaceEventResult>());
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
