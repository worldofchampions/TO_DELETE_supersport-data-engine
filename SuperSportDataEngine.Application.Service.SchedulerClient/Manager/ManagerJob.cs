namespace SuperSportDataEngine.Application.Service.SchedulerClient.Manager
{
    using Microsoft.Practices.Unity;
    using System;
    using System.Timers;
    using SuperSportDataEngine.Application.Service.SchedulerClient.ScheduledManager;
    using Hangfire;
    using SuperSportDataEngine.Application.Container;
    using SuperSportDataEngine.Common.Logging;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Services;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models;

    internal class ManagerJob
    {
        private Timer _timer;
        private ILoggingService _logger;
        private IRecurringJobManager _recurringJobManager;
        private IUnityContainer _container;
        private IRugbyService _rugbyService;
        private IRugbyIngestWorkerService _rugbyIngestWorkerService;
        private IBaseEntityFrameworkRepository<SchedulerTrackingRugbyFixture> _schedulerTrackingRugbyFixtures;
        private FixturesManagerJob _fixturesManagerJob;
        private LiveManagerJob _liveManagerJob;
        private LogsManagerJob _logsManagerJob;

        public ManagerJob()
        {

            ConfigureTimer();
            ConfigureDepenencies();
        }

        private void ConfigureDepenencies()
        {
            if (_container != null)
                _container.Dispose();

            _container = new UnityContainer();

            UnityConfigurationManager.RegisterTypes(_container, Container.Enums.ApplicationScope.ServiceSchedulerClient);

            _logger = _container.Resolve<ILoggingService>();
            _recurringJobManager = _container.Resolve<IRecurringJobManager>();
            _rugbyService = _container.Resolve<IRugbyService>();
            _rugbyIngestWorkerService = _container.Resolve<IRugbyIngestWorkerService>();
            _schedulerTrackingRugbyFixtures = _container.Resolve<IBaseEntityFrameworkRepository<SchedulerTrackingRugbyFixture>>();

            _fixturesManagerJob =
                new FixturesManagerJob(
                    _recurringJobManager,
                    _container,
                    _logger);

            _liveManagerJob = new LiveManagerJob(
                _logger,
                _recurringJobManager,
                _rugbyService,
                _rugbyIngestWorkerService,
                _schedulerTrackingRugbyFixtures);

            _logsManagerJob =
                new LogsManagerJob(
                    _recurringJobManager,
                    new UnityContainer(),
                    _logger);
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
            try
            {
                _logger.Debug("Do work for ManagerJob's.");

                ConfigureDepenencies();

                await _liveManagerJob.DoWorkAsync();
                await _fixturesManagerJob.DoWorkAsync();
                await _logsManagerJob.DoWorkAsync();

                _timer.Start();
            }
            catch (Exception exception)
            {
                _logger.Error(exception.StackTrace);
            }
        }
    }
}