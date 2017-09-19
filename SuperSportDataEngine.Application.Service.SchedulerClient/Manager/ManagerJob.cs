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

    internal class ManagerJob
    {
        private System.Timers.Timer _timer;
        private IRugbyService _rugbyService;
        private IRugbyIngestWorkerService _rugbyIngestService;
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

            _fixturesManagerJob =
                new FixturesManagerJob(
                    _rugbyService,
                    _rugbyIngestService,
                    _schedulerTrackingRugbyFixtureRepository,
                    _schedulerTrackingRugbyTournamentRepository,
                    _schedulerTrackingRugbySeasonRepository);

            _liveManagerJob =
                new LiveManagerJob(
                    _rugbyService,
                    _rugbyIngestService,
                    _schedulerTrackingRugbyFixtureRepository);

            _logsManagerJob =
                new LogsManagerJob(
                    _rugbyService,
                    _rugbyIngestService,
                    _schedulerTrackingRugbySeasonRepository);
        }

        private void ConfigureTimer()
        {
            _timer = new System.Timers.Timer
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