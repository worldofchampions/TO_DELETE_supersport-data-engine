using SuperSportDataEngine.Application.Container.Enums;

namespace SuperSportDataEngine.Application.Service.SchedulerClient.Manager
{
    using Microsoft.Practices.Unity;
    using System;
    using System.Timers;
    using ScheduledManager;
    using Hangfire;
    using Container;
    using ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
    using ApplicationLogic.Services;
    using ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Interfaces;
    using ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models;

    internal class ManagerJob
    {
        private Timer _timer;
        private IRecurringJobManager _recurringJobManager;
        private IUnityContainer _container;
        private IRugbyService _rugbyService;
        private IRugbyIngestWorkerService _rugbyIngestWorkerService;
        private IBaseEntityFrameworkRepository<SchedulerTrackingRugbyFixture> _schedulerTrackingRugbyFixtures;
        private FixturesManagerJob _fixturesManagerJob;
        private LiveManagerJob _liveManagerJob;
        private LogsManagerJob _logsManagerJob;
        private PlayerStatisticsManagerJob _playerStatisticsManagerJob;

        public ManagerJob()
        {
            ConfigureTimer();
            ConfigureDepenencies();
        }

        private void ConfigureDepenencies()
        {
            _container?.Dispose();

            _container = new UnityContainer();

            UnityConfigurationManager.RegisterTypes(_container, ApplicationScope.ServiceSchedulerClient);

            _recurringJobManager = _container.Resolve<IRecurringJobManager>();
            _rugbyService = _container.Resolve<IRugbyService>();
            _rugbyIngestWorkerService = _container.Resolve<IRugbyIngestWorkerService>();
            _schedulerTrackingRugbyFixtures = _container.Resolve<IBaseEntityFrameworkRepository<SchedulerTrackingRugbyFixture>>();

            _fixturesManagerJob =
                new FixturesManagerJob(
                    _recurringJobManager,
                    _container);

            _liveManagerJob = new LiveManagerJob(
                _recurringJobManager,
                _rugbyService,
                _rugbyIngestWorkerService,
                _schedulerTrackingRugbyFixtures);

            _logsManagerJob =
                new LogsManagerJob(
                    _recurringJobManager,
                    _container);

            _playerStatisticsManagerJob =
                new PlayerStatisticsManagerJob(
                    _recurringJobManager,
                    _container);
        }

        private void ConfigureTimer()
        {
            _timer = new Timer
            {
                AutoReset = false,
                Interval = TimeSpan.FromMinutes(1).TotalMilliseconds
            };

            _timer.Elapsed += UpdateManagerJobs;
            _timer.Start();
        }

        private async void UpdateManagerJobs(object sender, ElapsedEventArgs e)
        {
            var methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;

            ConfigureDepenencies();
            try
            {
                await _liveManagerJob.DoWorkAsync();
                await _fixturesManagerJob.DoWorkAsync();
                await _logsManagerJob.DoWorkAsync();
                await _playerStatisticsManagerJob.DoWorkAsync();
            }
            catch (Exception)
            {
                // ignored
            }

            _timer.Start();
        }
    }
}