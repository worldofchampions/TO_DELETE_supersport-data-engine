using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.UnitOfWork;
using SuperSportDataEngine.Repository.EntityFramework.Common.Repositories.Base;

namespace SuperSportDataEngine.Repository.EntityFramework.SystemSportData.UnitOfWork
{
    public class SystemSportDataUnitOfWork : ISystemSportDataUnitOfWork
    {
        private DbContext _context;

        public IBaseEntityFrameworkRepository<SchedulerDashboardUser> SchedulerDashboardUsers { get; set; }
        public IBaseEntityFrameworkRepository<LegacyAuthFeedConsumer> LegacyAuthFeedConsumers { get; set; }
        public IBaseEntityFrameworkRepository<LegacyZoneSite> LegacyZoneSites { get; set; }
        public IBaseEntityFrameworkRepository<SchedulerTrackingRugbyFixture> SchedulerTrackingRugbyFixtures { get; set; }
        public IBaseEntityFrameworkRepository<SchedulerTrackingRugbySeason> SchedulerTrackingRugbySeasons { get; set; }
        public IBaseEntityFrameworkRepository<SchedulerTrackingRugbyTournament> SchedulerTrackingRugbyTournaments { get; set; }

        public SystemSportDataUnitOfWork(DbContext context)
        {
            _context = context;
            SchedulerDashboardUsers = new BaseEntityFrameworkRepository<SchedulerDashboardUser>(_context);
            LegacyAuthFeedConsumers = new BaseEntityFrameworkRepository<LegacyAuthFeedConsumer>(_context);
            LegacyZoneSites = new BaseEntityFrameworkRepository<LegacyZoneSite>(_context);
            SchedulerTrackingRugbyFixtures = new BaseEntityFrameworkRepository<SchedulerTrackingRugbyFixture>(_context);
            SchedulerTrackingRugbySeasons = new BaseEntityFrameworkRepository<SchedulerTrackingRugbySeason>(_context);
            SchedulerTrackingRugbyTournaments = new BaseEntityFrameworkRepository<SchedulerTrackingRugbyTournament>(_context);
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        public Task<int> SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
