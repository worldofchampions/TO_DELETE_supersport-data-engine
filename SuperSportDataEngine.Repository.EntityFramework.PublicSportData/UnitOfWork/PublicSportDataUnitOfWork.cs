using System;
using System.Data.Entity;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.UnitOfWork;
using SuperSportDataEngine.Repository.EntityFramework.Common.Repositories.Base;
using SuperSportDataEngine.Repository.EntityFramework.PublicSportData.Context;

namespace SuperSportDataEngine.Repository.EntityFramework.PublicSportData.UnitOfWork
{
    public class PublicSportDataUnitOfWork : IPublicSportDataUnitOfWork, IDisposable
    {
        private readonly DbContext _context;

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
        public IBaseEntityFrameworkRepository<RugbyPlayerLineup> RugbyPlayerLineups { get; set; }
        public IBaseEntityFrameworkRepository<RugbySeason> RugbySeasons { get; set; }
        public IBaseEntityFrameworkRepository<RugbyTeam> RugbyTeams { get; set; }
        public IBaseEntityFrameworkRepository<RugbyTournament> RugbyTournaments { get; set; }
        public IBaseEntityFrameworkRepository<RugbyVenue> RugbyVenues { get; set; }

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
        }
        
        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
