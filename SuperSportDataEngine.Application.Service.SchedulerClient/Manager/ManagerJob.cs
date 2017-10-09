namespace SuperSportDataEngine.Application.Service.SchedulerClient.Manager
{
    using Microsoft.Practices.Unity;
    using System;
    using System.Timers;
    using SuperSportDataEngine.Application.Service.SchedulerClient.ScheduledManager;
    using Hangfire;

    internal class ManagerJob
    {
        private Timer _timer;
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
            _recurringJobManager = container.Resolve<IRecurringJobManager>();

            _fixturesManagerJob =
                new FixturesManagerJob(
                    _recurringJobManager,
                    container.CreateChildContainer());

            _liveManagerJob =
                new LiveManagerJob(
                    _recurringJobManager,
                    container.CreateChildContainer());

            _logsManagerJob =
                new LogsManagerJob(
                    _recurringJobManager,
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