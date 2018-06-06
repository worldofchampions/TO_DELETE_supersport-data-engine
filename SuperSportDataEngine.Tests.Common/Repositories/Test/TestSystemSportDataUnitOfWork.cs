namespace SuperSportDataEngine.Tests.Common.Repositories.Test
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.UnitOfWork;

    public class TestSystemSportDataUnitOfWork : ISystemSportDataUnitOfWork
    {
        public IBaseEntityFrameworkRepository<SchedulerDashboardUser> SchedulerDashboardUsers { get; set; }
        public IBaseEntityFrameworkRepository<LegacyAuthFeedConsumer> LegacyAuthFeedConsumers { get; set; }
        public IBaseEntityFrameworkRepository<LegacyZoneSite> LegacyZoneSites { get; set; }
        public IBaseEntityFrameworkRepository<SchedulerTrackingRugbyFixture> SchedulerTrackingRugbyFixtures { get; set; }
        public IBaseEntityFrameworkRepository<SchedulerTrackingRugbySeason> SchedulerTrackingRugbySeasons { get; set; }
        public IBaseEntityFrameworkRepository<SchedulerTrackingRugbyTournament> SchedulerTrackingRugbyTournaments { get; set; }
        public IBaseEntityFrameworkRepository<SchedulerTrackingMotorsportSeason> SchedulerTrackingMotorsportSeasons { get; }
        public IBaseEntityFrameworkRepository<SchedulerTrackingMotorsportRaceEvent> SchedulerTrackingMotorsportRaceEvents { get; set; }

        public TestSystemSportDataUnitOfWork()
        {
            SchedulerDashboardUsers = new TestEntityFrameworkRepository<SchedulerDashboardUser>(new List<SchedulerDashboardUser>());
            SchedulerTrackingRugbyFixtures = new TestEntityFrameworkRepository<SchedulerTrackingRugbyFixture>(new List<SchedulerTrackingRugbyFixture>());
            LegacyAuthFeedConsumers = new TestEntityFrameworkRepository<LegacyAuthFeedConsumer>(new List<LegacyAuthFeedConsumer>());
            LegacyZoneSites = new TestEntityFrameworkRepository<LegacyZoneSite>(new List<LegacyZoneSite>());
            SchedulerTrackingRugbySeasons = new TestEntityFrameworkRepository<SchedulerTrackingRugbySeason>(new List<SchedulerTrackingRugbySeason>());
            SchedulerTrackingRugbyTournaments = new TestEntityFrameworkRepository<SchedulerTrackingRugbyTournament>(new List<SchedulerTrackingRugbyTournament>());
            SchedulerTrackingMotorsportSeasons = new TestEntityFrameworkRepository<SchedulerTrackingMotorsportSeason>(new List<SchedulerTrackingMotorsportSeason>());
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
            SchedulerDashboardUsers?.Dispose();
            LegacyAuthFeedConsumers?.Dispose();
            LegacyZoneSites?.Dispose();
            SchedulerTrackingRugbyFixtures?.Dispose();
            SchedulerTrackingRugbySeasons?.Dispose();
            SchedulerTrackingRugbyTournaments?.Dispose();
            SchedulerTrackingMotorsportSeasons?.Dispose();
        }
    }
}
