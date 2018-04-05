using System.Configuration;
using System.Runtime.CompilerServices;
using SuperSportDataEngine.Application.Container.Enums;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.UnitOfWork;

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

    internal class ManagerJob
    {
        private Timer _timer;
        private IRecurringJobManager _recurringJobManager;
        private IUnityContainer _container;
        private IRugbyService _rugbyService;
        private IRugbyIngestWorkerService _rugbyIngestWorkerService;
        private ISystemSportDataUnitOfWork _systemSportDataUnitOfWork;
        private FixturesManagerJob _fixturesManagerJob;
        private LiveManagerJob _liveManagerJob;
        private LogsManagerJob _logsManagerJob;
        private PlayerStatisticsManagerJob _playerStatisticsManagerJob;
        private MotorsportLiveManagerJob _motorsportLiveManagerJob;
        private IMotorsportIngestWorkerService _motorIngestWorkerService;
        private IMotorsportService _motorsportService;
        private static int _managerLoopTimeInSeconds;

        public ManagerJob()
        {
            _managerLoopTimeInSeconds = 
                int.Parse(
                    ConfigurationManager.AppSettings["ManagerLoopTimeInSeconds"]);

            ConfigureTimer();
            ConfigureDepenencies();
        }

        private void ConfigureDepenencies()
        {
            _container?.Dispose();

            _container = new UnityContainer();

            UnityConfigurationManager.RegisterTypes(_container, ApplicationScope.ServiceSchedulerClient);

            _recurringJobManager = _container.Resolve<IRecurringJobManager>();
            _systemSportDataUnitOfWork = _container.Resolve<ISystemSportDataUnitOfWork>();

            _rugbyService = _container.Resolve<IRugbyService>();
            _rugbyIngestWorkerService = _container.Resolve<IRugbyIngestWorkerService>();

            _motorsportService = _container.Resolve<IMotorsportService>();
            _motorIngestWorkerService = _container.Resolve<IMotorsportIngestWorkerService>();

            _motorsportLiveManagerJob = new MotorsportLiveManagerJob(
                _recurringJobManager,
                _container,
                _motorsportService,
                _motorIngestWorkerService);

            _fixturesManagerJob =
                new FixturesManagerJob(
                    _recurringJobManager,
                    _systemSportDataUnitOfWork,
                    _rugbyService,
                    _rugbyIngestWorkerService);

            _liveManagerJob = new LiveManagerJob(
                _recurringJobManager,
                _rugbyService,
                _rugbyIngestWorkerService,
                _systemSportDataUnitOfWork);

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
                Interval = TimeSpan.FromSeconds(_managerLoopTimeInSeconds).TotalMilliseconds
            };

            _timer.Elapsed += UpdateManagerJobs;
            _timer.Start();
        }

        private async void UpdateManagerJobs(object sender, ElapsedEventArgs e)
        {
            ConfigureDepenencies();

            try
            {
                var rugbyEnabled = bool.Parse(ConfigurationManager.AppSettings["RugbyIngestEnabled"]);

                if (rugbyEnabled)
                {
                    await _liveManagerJob.DoWorkAsync();
                    await _fixturesManagerJob.DoWorkAsync();
                    await _logsManagerJob.DoWorkAsync();
                    await _playerStatisticsManagerJob.DoWorkAsync();
                }

                var motorEnabled = bool.Parse(ConfigurationManager.AppSettings["MotorsportIngestEnabled"]);

                if (motorEnabled)
                {
                    await _motorsportLiveManagerJob.DoWorkAsync();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                // ignored
            }

            _timer.Start();
        }
    }
}