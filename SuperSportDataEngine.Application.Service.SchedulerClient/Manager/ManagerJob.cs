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

        public ManagerJob(UnityContainer container)
        {
            ConfigureTimer();
            ConfigureDepenencies(container);
        }

        private void ConfigureDepenencies(UnityContainer container)
        {
            _logger = container.Resolve<ILoggingService>();

            _recurringJobManager = container.Resolve<IRecurringJobManager>();

            var childContainer = container.CreateChildContainer();
            UnityConfigurationManager.RegisterTypes(childContainer, Container.Enums.ApplicationScope.ServiceSchedulerClient);

            _fixturesManagerJob =
                new FixturesManagerJob(
                    _recurringJobManager,
                    childContainer);

            _liveManagerJob =
                new LiveManagerJob(
                    _recurringJobManager,
                    childContainer);

            _logsManagerJob =
                new LogsManagerJob(
                    _recurringJobManager,
                    childContainer);
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
            _logger.Info("Do work for ManagerJob's.");

            await _liveManagerJob.DoWorkAsync();
            await _fixturesManagerJob.DoWorkAsync();
            await _logsManagerJob.DoWorkAsync();

            _timer.Start();
        }
    }
}