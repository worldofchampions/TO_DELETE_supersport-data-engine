using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.UnitOfWork;

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
        private ISystemSportDataUnitOfWork _systemSportDataUnitOfWork;
        private FixturesManagerJob _fixturesManagerJob;
        private LiveManagerJob _liveManagerJob;
        private LogsManagerJob _logsManagerJob;
        private MotorDriversManagerJob _driversManagerJob;


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
            _systemSportDataUnitOfWork = _container.Resolve<ISystemSportDataUnitOfWork>();


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
                _systemSportDataUnitOfWork);

            _logsManagerJob =
                new LogsManagerJob(
                    _recurringJobManager,
                    new UnityContainer());

            _driversManagerJob = 
                new MotorDriversManagerJob(
                _recurringJobManager, 
                new UnityContainer());
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
            var methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;

            await _logger.Debug(methodName, "Do work for ManagerJob's.");

            ConfigureDepenencies();
            try
            {
                await _driversManagerJob.DoWorkAsync();
                //await _liveManagerJob.DoWorkAsync();
                //await _fixturesManagerJob.DoWorkAsync();
                //await _logsManagerJob.DoWorkAsync();
            }
            catch (Exception exception)
            {
                await _logger.Info(methodName, exception.StackTrace);
            }

            _timer.Start();
        }
    }
}