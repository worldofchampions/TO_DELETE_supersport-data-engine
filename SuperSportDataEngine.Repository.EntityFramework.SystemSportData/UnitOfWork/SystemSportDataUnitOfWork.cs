namespace SuperSportDataEngine.Repository.EntityFramework.SystemSportData.UnitOfWork
{
    using System.Data.Entity;
    using System.Threading.Tasks;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.UnitOfWork;
    using SuperSportDataEngine.Repository.EntityFramework.Common.Repositories.Base;

    public class SystemSportDataUnitOfWork : ISystemSportDataUnitOfWork
    {
        private readonly DbContext _context;

        public IBaseEntityFrameworkRepository<SchedulerDashboardUser> SchedulerDashboardUsers { get; private set; }
        public IBaseEntityFrameworkRepository<LegacyAuthFeedConsumer> LegacyAuthFeedConsumers { get; private set; }
        public IBaseEntityFrameworkRepository<LegacyZoneSite> LegacyZoneSites { get; private set; }
        public IBaseEntityFrameworkRepository<SchedulerTrackingRugbyFixture> SchedulerTrackingRugbyFixtures { get; private set; }
        public IBaseEntityFrameworkRepository<SchedulerTrackingRugbySeason> SchedulerTrackingRugbySeasons { get; private set; }
        public IBaseEntityFrameworkRepository<SchedulerTrackingRugbyTournament> SchedulerTrackingRugbyTournaments { get; private set; }
        public IBaseEntityFrameworkRepository<SchedulerTrackingMotorsportSeason> SchedulerTrackingMotorsportSeasons { get; }
        public IBaseEntityFrameworkRepository<SchedulerTrackingMotorsportRaceEvent> SchedulerTrackingMotorsportRaceEvents { get; private set; }

        public SystemSportDataUnitOfWork(DbContext context)
        {
            _context = context;
            SchedulerDashboardUsers = new BaseEntityFrameworkRepository<SchedulerDashboardUser>(_context);
            LegacyAuthFeedConsumers = new BaseEntityFrameworkRepository<LegacyAuthFeedConsumer>(_context);
            LegacyZoneSites = new BaseEntityFrameworkRepository<LegacyZoneSite>(_context);
            SchedulerTrackingRugbyFixtures = new BaseEntityFrameworkRepository<SchedulerTrackingRugbyFixture>(_context);
            SchedulerTrackingRugbySeasons = new BaseEntityFrameworkRepository<SchedulerTrackingRugbySeason>(_context);
            SchedulerTrackingRugbyTournaments = new BaseEntityFrameworkRepository<SchedulerTrackingRugbyTournament>(_context);

            SchedulerTrackingMotorsportSeasons = new BaseEntityFrameworkRepository<SchedulerTrackingMotorsportSeason>(_context);
            SchedulerTrackingMotorsportRaceEvents = new BaseEntityFrameworkRepository<SchedulerTrackingMotorsportRaceEvent>(_context);
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
            _context?.Dispose();
        }
    }
}
