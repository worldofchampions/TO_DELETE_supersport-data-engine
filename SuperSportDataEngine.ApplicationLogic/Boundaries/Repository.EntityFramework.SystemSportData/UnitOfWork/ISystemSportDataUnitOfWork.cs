namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.UnitOfWork
{
    using System;
    using System.Threading.Tasks;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models;

    public interface ISystemSportDataUnitOfWork : IDisposable
    {
        IBaseEntityFrameworkRepository<SchedulerDashboardUser> SchedulerDashboardUsers { get; }
        IBaseEntityFrameworkRepository<LegacyAuthFeedConsumer> LegacyAuthFeedConsumers { get; }
        IBaseEntityFrameworkRepository<LegacyZoneSite> LegacyZoneSites { get; }
        IBaseEntityFrameworkRepository<SchedulerTrackingRugbyFixture> SchedulerTrackingRugbyFixtures { get; }
        IBaseEntityFrameworkRepository<SchedulerTrackingRugbySeason> SchedulerTrackingRugbySeasons { get; }
        IBaseEntityFrameworkRepository<SchedulerTrackingRugbyTournament> SchedulerTrackingRugbyTournaments { get; }
        IBaseEntityFrameworkRepository<SchedulerTrackingMotorsportRaceEvent> SchedulerTrackingMotorsportRaceEvents { get; }
        int SaveChanges();
        Task<int> SaveChangesAsync();
    }
}