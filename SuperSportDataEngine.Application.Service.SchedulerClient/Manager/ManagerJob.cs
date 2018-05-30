namespace SuperSportDataEngine.Application.Service.SchedulerClient.Manager
{
    using Microsoft.Practices.Unity;
    using System;
    using System.Timers;
    using System.Threading.Tasks;
    using ScheduledManager;
    using Hangfire;
    using Container;
    using ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
    using ApplicationLogic.Services;
    using System.Configuration;
    using SuperSportDataEngine.Application.Container.Enums;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.UnitOfWork;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.UnitOfWork;
    using SuperSportDataEngine.Common.Logging;

    internal class ManagerJob
    {
        private Timer _timer;
        private IRecurringJobManager _recurringJobManager;
        private IUnityContainer _container;
        private IRugbyService _rugbyService;
        private IRugbyIngestWorkerService _rugbyIngestWorkerService;
        private ISystemSportDataUnitOfWork _systemSportDataUnitOfWork;
        private IPublicSportDataUnitOfWork _publicSportDataUnitOfWork;
        private FixturesManagerJob _fixturesManagerJob;
        private LiveManagerJob _liveManagerJob;
        private LogsManagerJob _logsManagerJob;
        private PlayerStatisticsManagerJob _playerStatisticsManagerJob;
        private MotorsportLiveManagerJob _motorsportLiveManagerJob;
        private IMotorsportIngestWorkerService _motorsportIngestWorkerService;
        private IMotorsportService _motorsportService;
        private static int _managerLoopTimeInSeconds;
        private ILoggingService _logger;

        public ManagerJob()
        {
            _managerLoopTimeInSeconds = int.Parse(ConfigurationManager.AppSettings["ManagerLoopTimeInSeconds"]);

            ConfigureTimer();

            ConfigureDepenencies();
        }

        private void ConfigureDepenencies()
        {
            ConfigureCommonDependencies();

            ConfigureRugbyDependencies();

            ConfigureMotorsportDependencies();
        }

        private void ConfigureCommonDependencies()
        {
            _container?.Dispose();

            _container = new UnityContainer();

            UnityConfigurationManager.RegisterTypes(_container, ApplicationScope.ServiceSchedulerClient);

            _recurringJobManager = _container.Resolve<IRecurringJobManager>();

            _systemSportDataUnitOfWork = _container.Resolve<ISystemSportDataUnitOfWork>();

            _publicSportDataUnitOfWork = _container.Resolve<IPublicSportDataUnitOfWork>();

            _logger = _container.Resolve<ILoggingService>();

        }

        private void ConfigureRugbyDependencies()
        {
            _rugbyService = _container.Resolve<IRugbyService>();

            _rugbyIngestWorkerService = _container.Resolve<IRugbyIngestWorkerService>();
            
            _fixturesManagerJob =
                new FixturesManagerJob(
                    _recurringJobManager,
                    _systemSportDataUnitOfWork,
                    _rugbyService,
                    _rugbyIngestWorkerService,
                    _logger);

            _liveManagerJob =
                new LiveManagerJob(
                _recurringJobManager,
                _rugbyService,
                _rugbyIngestWorkerService,
                _systemSportDataUnitOfWork);

            _logsManagerJob =
                new LogsManagerJob(
                    _recurringJobManager,
                    _rugbyService,
                    _rugbyIngestWorkerService,
                    _systemSportDataUnitOfWork,
                    _publicSportDataUnitOfWork);

            _playerStatisticsManagerJob =
                new PlayerStatisticsManagerJob(
                    _recurringJobManager,
                    _rugbyService,
                    _rugbyIngestWorkerService);
        }

        private void ConfigureMotorsportDependencies()
        {
            _motorsportService = _container.Resolve<IMotorsportService>();

            _motorsportIngestWorkerService = _container.Resolve<IMotorsportIngestWorkerService>();

            _motorsportLiveManagerJob =
                new MotorsportLiveManagerJob(_recurringJobManager, _motorsportService, _motorsportIngestWorkerService, _systemSportDataUnitOfWork, _publicSportDataUnitOfWork);
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
            try
            {
                ConfigureDepenencies();

                var rugbyEnabled = bool.Parse(ConfigurationManager.AppSettings["RugbyIngestEnabled"]);

                if (rugbyEnabled)
                {
                    await ScheduleRugbyJobs();
                }

                var motorsportEnabled = bool.Parse(ConfigurationManager.AppSettings["MotorsportIngestEnabled"]);

                if (motorsportEnabled)
                {
                    await ScheduleMotorsportJobs();
                }
            }
            catch (Exception exception)
            {
                _logger?.Fatal(
                    "LegacyException." + exception.Message,
                    "Message: " + Environment.NewLine + exception.Message + "  " +
                    "StackTrace: " + Environment.NewLine + exception.StackTrace + "  " +
                    "Inner Exception " + Environment.NewLine + exception.InnerException);
            }

            _timer.Start();
        }

        private async Task ScheduleMotorsportJobs()
        {
            await _motorsportLiveManagerJob.DoWorkAsync();
        }

        private async Task ScheduleRugbyJobs()
        {
            await _liveManagerJob.DoWorkAsync();
            await _fixturesManagerJob.DoWorkAsync();
            await _logsManagerJob.DoWorkAsync();
            await _playerStatisticsManagerJob.DoWorkAsync();
        }
    }
}