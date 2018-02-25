using System;
using System.Data.Entity;
using System.Threading.Tasks;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.UnitOfWork;
using SuperSportDataEngine.Repository.EntityFramework.Common.Repositories.Base;

namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.UnitOfWork
{
    public class PublicSportDataUnitOfWork : IPublicSportDataUnitOfWork, IDisposable
    {
        private readonly DbContext _context;

        public IBaseEntityFrameworkRepository<RugbyCommentary> RugbyCommentaries { get; private set; }
        public IBaseEntityFrameworkRepository<RugbyEventType> RugbyEventTypes { get; private set; }
        public IBaseEntityFrameworkRepository<RugbyEventTypeProviderMapping> RugbyEventTypeProviderMappings { get; private set; }
        public IBaseEntityFrameworkRepository<RugbyFixture> RugbyFixtures { get; private set; }
        public IBaseEntityFrameworkRepository<RugbyFlatLog> RugbyFlatLogs { get; private set; }
        public IBaseEntityFrameworkRepository<RugbyGroupedLog> RugbyGroupedLogs { get; private set; }
        public IBaseEntityFrameworkRepository<RugbyLogGroup> RugbyLogGroups { get; private set; }
        public IBaseEntityFrameworkRepository<RugbyMatchEvent> RugbyMatchEvents { get; private set; }
        public IBaseEntityFrameworkRepository<RugbyMatchStatistics> RugbyMatchStatistics { get; private set; }
        public IBaseEntityFrameworkRepository<RugbyPlayer> RugbyPlayers { get; private set; }
        public IBaseEntityFrameworkRepository<RugbyPlayerStatistics> RugbyPlayerStatistics { get; private set; }
        public IBaseEntityFrameworkRepository<RugbyPlayerLineup> RugbyPlayerLineups { get; private set; }
        public IBaseEntityFrameworkRepository<RugbySeason> RugbySeasons { get; private set; }
        public IBaseEntityFrameworkRepository<RugbyTeam> RugbyTeams { get; private set; }
        public IBaseEntityFrameworkRepository<RugbyTournament> RugbyTournaments { get; private set; }
        public IBaseEntityFrameworkRepository<RugbyVenue> RugbyVenues { get; private set; }


        public IBaseEntityFrameworkRepository<MotorsportDriver> MotorsportDrivers { get; set; }
        public IBaseEntityFrameworkRepository<MotorsportLeague> MotorsportLeagues { get; set; }
        public IBaseEntityFrameworkRepository<MotorsportRace> MotorsportRaces { get; set; }
        public IBaseEntityFrameworkRepository<MotorsportDriverStanding> MotorsportDriverStandings { get; set; }
        public IBaseEntityFrameworkRepository<MotorsportTeamStanding> MotorsportTeamStandings { get; set; }
        public IBaseEntityFrameworkRepository<MotorsportSeason> MotorsportSeasons { get; set; }
        public IBaseEntityFrameworkRepository<MotorsportTeam> MotortsportTeams { get; set; }
        public IBaseEntityFrameworkRepository<MotorsportRaceResult> MotorsportRaceResults { get; set; }
        public IBaseEntityFrameworkRepository<MotorsportRaceGrid> MotorsportRaceGrids { get; set; }
        public IBaseEntityFrameworkRepository<MotorsportRaceCalendar> MotorsportRaceCalendars { get; set; }

        public PublicSportDataUnitOfWork(
            DbContext context)
        {
            _context = context;
            RugbyCommentaries = new BaseEntityFrameworkRepository<RugbyCommentary>(_context);
            RugbyEventTypeProviderMappings = new BaseEntityFrameworkRepository<RugbyEventTypeProviderMapping>(_context);
            RugbyEventTypes = new BaseEntityFrameworkRepository<RugbyEventType>(_context);
            RugbyFixtures = new BaseEntityFrameworkRepository<RugbyFixture>(_context);
            RugbyFlatLogs = new BaseEntityFrameworkRepository<RugbyFlatLog>(_context);
            RugbyGroupedLogs = new BaseEntityFrameworkRepository<RugbyGroupedLog>(_context);
            RugbyLogGroups = new BaseEntityFrameworkRepository<RugbyLogGroup>(_context);
            RugbyMatchEvents = new BaseEntityFrameworkRepository<RugbyMatchEvent>(_context);
            RugbyMatchStatistics = new BaseEntityFrameworkRepository<RugbyMatchStatistics>(_context);
            RugbyPlayerLineups = new BaseEntityFrameworkRepository<RugbyPlayerLineup>(_context);
            RugbyPlayers = new BaseEntityFrameworkRepository<RugbyPlayer>(_context);
            RugbyPlayerStatistics = new BaseEntityFrameworkRepository<RugbyPlayerStatistics>(_context);
            RugbySeasons = new BaseEntityFrameworkRepository<RugbySeason>(_context);
            RugbyTeams = new BaseEntityFrameworkRepository<RugbyTeam>(_context);
            RugbyTournaments = new BaseEntityFrameworkRepository<RugbyTournament>(_context);
            RugbyVenues = new BaseEntityFrameworkRepository<RugbyVenue>(_context);

            MotorsportDrivers = new BaseEntityFrameworkRepository<MotorsportDriver>(_context);
            MotorsportDriverStandings = new BaseEntityFrameworkRepository<MotorsportDriverStanding>(_context);
            MotorsportRaceCalendars = new BaseEntityFrameworkRepository<MotorsportRaceCalendar>(_context);
            MotorsportRaceGrids = new BaseEntityFrameworkRepository<MotorsportRaceGrid>(_context);
            MotorsportLeagues = new BaseEntityFrameworkRepository<MotorsportLeague>(_context);
            MotorsportRaces = new BaseEntityFrameworkRepository<MotorsportRace>(_context);
            MotorsportRaceResults = new BaseEntityFrameworkRepository<MotorsportRaceResult>(_context);
            MotorsportSeasons = new BaseEntityFrameworkRepository<MotorsportSeason>(_context);
            MotortsportTeams = new BaseEntityFrameworkRepository<MotorsportTeam>(_context);
            MotorsportTeamStandings = new BaseEntityFrameworkRepository<MotorsportTeamStanding>(_context);
        }
        
        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        public Task<int> SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
