namespace SuperSportDataEngine.Application.Service.SchedulerClient.Manager
{
    using Microsoft.Practices.Unity;
    using System;
    using System.Timers;
    using SuperSportDataEngine.Application.Service.SchedulerClient.ScheduledManager;
    using Hangfire;
    using SuperSportDataEngine.Application.Container;
    using SuperSportDataEngine.Common.Logging;

    internal class ManagerJob
    {
        private Timer _timer;
        private ILoggingService _logger;
        private IRecurringJobManager _recurringJobManager;

        private FixturesManagerJob _fixturesManagerJob;
        private LiveManagerJob _liveManagerJob;
        private LogsManagerJob _logsManagerJob;

        public ManagerJob(IUnityContainer container)
        {
            ConfigureTimer();
            ConfigureDepenencies(container);
        }

        private void ConfigureDepenencies(IUnityContainer container)
        {
            var childContainer = container.CreateChildContainer();
            UnityConfigurationManager.RegisterTypes(childContainer, Container.Enums.ApplicationScope.ServiceSchedulerClient);

            _logger = container.Resolve<ILoggingService>();
            _recurringJobManager = container.Resolve<IRecurringJobManager>();

            _fixturesManagerJob =
                new FixturesManagerJob(
                    _recurringJobManager,
                    childContainer,
                    _logger);

            _liveManagerJob =
                new LiveManagerJob();

            _logsManagerJob =
                new LogsManagerJob(
                    _recurringJobManager,
                    childContainer,
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
            _logger.Debug("Do work for ManagerJob's.");

            await _liveManagerJob.DoWorkAsync();
            await _fixturesManagerJob.DoWorkAsync();
            await _logsManagerJob.DoWorkAsync();

            _timer.Start();
        }
    }
}