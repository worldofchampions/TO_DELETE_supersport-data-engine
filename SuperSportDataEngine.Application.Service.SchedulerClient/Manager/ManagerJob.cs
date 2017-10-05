namespace SuperSportDataEngine.Application.Service.SchedulerClient.Manager
{
    using Microsoft.Practices.Unity;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Services;
    using System;
    using System.Timers;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models;
    using SuperSportDataEngine.Application.Service.SchedulerClient.ScheduledManager;
    using Hangfire;

    internal class ManagerJob
    {
        private Timer _timer;
        private IRugbyService _rugbyService;
        private IRugbyIngestWorkerService _rugbyIngestService;
        private IRecurringJobManager _recurringJobManager;
        private IBaseEntityFrameworkRepository<SchedulerTrackingRugbyFixture> _schedulerTrackingRugbyFixtureRepository;
        private IBaseEntityFrameworkRepository<SchedulerTrackingRugbySeason> _schedulerTrackingRugbySeasonRepository;
        private IBaseEntityFrameworkRepository<SchedulerTrackingRugbyTournament> _schedulerTrackingRugbyTournamentRepository;

        private FixturesManagerJob _fixturesManagerJob;
        private LiveManagerJob _liveManagerJob;
        private LogsManagerJob _logsManagerJob;

        public ManagerJob(UnityContainer container)
        {
            ConfigureTimer();
            ConfigureDepenencies(container);
        }

        private void ConfigureDepenencies(UnityContainer container)
        {
            _rugbyService = container.Resolve<IRugbyService>();
            _rugbyIngestService = container.Resolve<IRugbyIngestWorkerService>();
            _schedulerTrackingRugbyFixtureRepository = container.Resolve<IBaseEntityFrameworkRepository<SchedulerTrackingRugbyFixture>>();
            _schedulerTrackingRugbySeasonRepository = container.Resolve<IBaseEntityFrameworkRepository<SchedulerTrackingRugbySeason>>();
            _schedulerTrackingRugbyTournamentRepository = container.Resolve<IBaseEntityFrameworkRepository<SchedulerTrackingRugbyTournament>>();
            _recurringJobManager = container.Resolve<IRecurringJobManager>();

            _fixturesManagerJob =
                new FixturesManagerJob(
                    _schedulerTrackingRugbyTournamentRepository,
                    _schedulerTrackingRugbySeasonRepository,
                    container.CreateChildContainer());

            _liveManagerJob =
                new LiveManagerJob(
                    _recurringJobManager,
                    _schedulerTrackingRugbyFixtureRepository,
                    container.CreateChildContainer());

            _logsManagerJob =
                new LogsManagerJob(
                    _rugbyService,
                    _rugbyIngestService,
                    _recurringJobManager,
                    _schedulerTrackingRugbySeasonRepository,
                    container.CreateChildContainer());
        }

        private void ConfigureTimer()
        {
            _timer = new Timer
            {
                AutoReset = false,
                Interval = TimeSpan.FromMinutes(1).TotalMilliseconds
            };

            _timer.Elapsed += new ElapsedEventHandler(UpdateManagerJobs);
            _timer.Start();
        }

        private async void UpdateManagerJobs(object sender, ElapsedEventArgs e)
        {
            await _liveManagerJob.DoWorkAsync();
            await _fixturesManagerJob.DoWorkAsync();
            await _logsManagerJob.DoWorkAsync();

            _timer.Start();
        }
    }
}