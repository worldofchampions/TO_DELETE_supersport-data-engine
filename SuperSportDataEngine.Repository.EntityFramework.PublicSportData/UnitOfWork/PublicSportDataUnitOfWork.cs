using System;
using System.Data.Entity;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.UnitOfWork;
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
            DbContext context,
            IBaseEntityFrameworkRepository<RugbyCommentary> rugbyCommentaries,
            IBaseEntityFrameworkRepository<RugbyEventType> rugbyEventTypes,
            IBaseEntityFrameworkRepository<RugbyEventTypeProviderMapping> rugbyEventTypeProviderMappings,
            IBaseEntityFrameworkRepository<RugbyFixture> rugbyFixtures,
            IBaseEntityFrameworkRepository<RugbyFlatLog> rugbyFlatLogs,
            IBaseEntityFrameworkRepository<RugbyGroupedLog> rugbyGroupedLogs,
            IBaseEntityFrameworkRepository<RugbyLogGroup> rugbyLogGroups,
            IBaseEntityFrameworkRepository<RugbyMatchEvent> rugbyMatchEvents,
            IBaseEntityFrameworkRepository<RugbyMatchStatistics> rugbyMatchStatistics,
            IBaseEntityFrameworkRepository<RugbyPlayer> rugbyPlayers,
            IBaseEntityFrameworkRepository<RugbyPlayerLineup> rugbyPlayerLineups,
            IBaseEntityFrameworkRepository<RugbySeason> rugbySeasons,
            IBaseEntityFrameworkRepository<RugbyTeam> rugbyTeams,
            IBaseEntityFrameworkRepository<RugbyTournament> rugbyTournaments,
            IBaseEntityFrameworkRepository<RugbyVenue> rugbyVenues)
        {
            _context = context;
            RugbyCommentaries = rugbyCommentaries;
            RugbyEventTypeProviderMappings = rugbyEventTypeProviderMappings;
            RugbyEventTypes = rugbyEventTypes;
            RugbyFixtures = rugbyFixtures;
            RugbyFlatLogs = rugbyFlatLogs;
            RugbyGroupedLogs = rugbyGroupedLogs;
            RugbyLogGroups = rugbyLogGroups;
            RugbyMatchEvents = rugbyMatchEvents;
            RugbyMatchStatistics = rugbyMatchStatistics;
            RugbyPlayerLineups = rugbyPlayerLineups;
            RugbyPlayers = rugbyPlayers;
            RugbySeasons = rugbySeasons;
            RugbyTeams = rugbyTeams;
            RugbyTournaments = rugbyTournaments;
            RugbyVenues = rugbyVenues;
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
