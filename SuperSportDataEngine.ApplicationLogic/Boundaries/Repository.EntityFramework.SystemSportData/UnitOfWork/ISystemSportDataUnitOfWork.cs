using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.UnitOfWork
{
    public interface ISystemSportDataUnitOfWork : IDisposable
    {
        IBaseEntityFrameworkRepository<SchedulerDashboardUser> SchedulerDashboardUsers { get; set; }
        IBaseEntityFrameworkRepository<LegacyAuthFeedConsumer> LegacyAuthFeedConsumers { get; set; }
        IBaseEntityFrameworkRepository<LegacyZoneSite> LegacyZoneSites { get; set; }
        IBaseEntityFrameworkRepository<SchedulerTrackingRugbyFixture> SchedulerTrackingRugbyFixtures { get; set; }
        IBaseEntityFrameworkRepository<SchedulerTrackingRugbySeason> SchedulerTrackingRugbySeasons { get; set; }
        IBaseEntityFrameworkRepository<SchedulerTrackingRugbyTournament> SchedulerTrackingRugbyTournaments { get; set; }

        int SaveChanges();
        Task<int> SaveChangesAsync();
    }
}
