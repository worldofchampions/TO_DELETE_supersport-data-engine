using System;
using System.Threading.Tasks;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.UnitOfWork
{
    public interface ISystemSportDataUnitOfWork : IDisposable
    {
        IBaseEntityFrameworkRepository<SchedulerDashboardUser> SchedulerDashboardUsers { get; }
        IBaseEntityFrameworkRepository<LegacyAuthFeedConsumer> LegacyAuthFeedConsumers { get; }
        IBaseEntityFrameworkRepository<LegacyZoneSite> LegacyZoneSites { get; }
        IBaseEntityFrameworkRepository<SchedulerTrackingRugbyFixture> SchedulerTrackingRugbyFixtures { get; }
        IBaseEntityFrameworkRepository<SchedulerTrackingRugbySeason> SchedulerTrackingRugbySeasons { get; }
        IBaseEntityFrameworkRepository<SchedulerTrackingRugbyTournament> SchedulerTrackingRugbyTournaments { get; }
        IBaseEntityFrameworkRepository<SchedulerTrackingMotorsportSeason> SchedulerTrackingMotorsportSeasons { get; }
        IBaseEntityFrameworkRepository<SchedulerTrackingMotorsportRace> SchedulerTrackingMotorsportRaces { get; set; }

        int SaveChanges();
        Task<int> SaveChangesAsync();
    }
}