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
        public IBaseEntityFrameworkRepository<RugbyPlayerLineup> RugbyPlayerLineups { get; private set; }
        public IBaseEntityFrameworkRepository<RugbySeason> RugbySeasons { get; private set; }
        public IBaseEntityFrameworkRepository<RugbyTeam> RugbyTeams { get; private set; }
        public IBaseEntityFrameworkRepository<RugbyTournament> RugbyTournaments { get; private set; }
        public IBaseEntityFrameworkRepository<RugbyVenue> RugbyVenues { get; private set; }


        public IBaseEntityFrameworkRepository<MotorDriver> MotorDrivers { get; set; }
        public IBaseEntityFrameworkRepository<MotorLeague> MotorLeagues { get; set; }
        public IBaseEntityFrameworkRepository<MotorRace> MotorRaces { get; set; }
        public IBaseEntityFrameworkRepository<MotorDriverStanding> MotorDriverStandings { get; set; }
        public IBaseEntityFrameworkRepository<MotorTeamStanding> MotorTeamStandings { get; set; }
        public IBaseEntityFrameworkRepository<MotorTeam> MotortTeams { get; set; }
        public IBaseEntityFrameworkRepository<MotorRaceResult> MotorTeamResults { get; set; }
        public IBaseEntityFrameworkRepository<MotorRaceCalendar> MotorCalendars { get; set; }
        public IBaseEntityFrameworkRepository<MotorGrid> MotorGrids { get; set; }

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
            RugbySeasons = new BaseEntityFrameworkRepository<RugbySeason>(_context);
            RugbyTeams = new BaseEntityFrameworkRepository<RugbyTeam>(_context);
            RugbyTournaments = new BaseEntityFrameworkRepository<RugbyTournament>(_context);
            RugbyVenues = new BaseEntityFrameworkRepository<RugbyVenue>(_context);
            MotorDrivers = new BaseEntityFrameworkRepository<MotorDriver>(_context);
            MotorGrids = new BaseEntityFrameworkRepository<MotorGrid>(_context);
            MotorLeagues = new BaseEntityFrameworkRepository<MotorLeague>(_context);
            MotorRaces = new BaseEntityFrameworkRepository<MotorRace>(_context);
            MotorTeamStandings = new BaseEntityFrameworkRepository<MotorTeamStanding>(_context);
            MotorDriverStandings = new BaseEntityFrameworkRepository<MotorDriverStanding>(_context);

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
