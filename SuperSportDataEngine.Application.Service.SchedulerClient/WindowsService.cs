namespace SuperSportDataEngine.Application.Service.SchedulerClient
{
    using Hangfire;
    using Microsoft.Owin.Hosting;
    using Microsoft.Practices.Unity;
    using SuperSportDataEngine.Application.Service.Common.Hangfire.Configuration;
    using SuperSportDataEngine.Application.Service.Common.Hangfire.Filters;
    using SuperSportDataEngine.Application.Service.Common.Interfaces;
    using SuperSportDataEngine.Application.Service.SchedulerClient.FixedSchedule;
    using SuperSportDataEngine.ApplicationLogic.Services;
    using System.Configuration;
    using System.Threading;
    using SuperSportDataEngine.Application.Service.SchedulerClient.ScheduledManager;
    using SuperSportDataEngine.Application.Service.SchedulerClient.LiveManager;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
    using System;

    internal class WindowsService : IWindowsServiceContract
    {
        private readonly UnityContainer _container;
        private readonly FixedScheduledJob _fixedManagerJob;
        private readonly FixturesScheduledManagerJob _fixtureScheduleManagerJob;
        private readonly LiveManagerJob _liveManagerJob;
        private readonly LogsScheduledManagerJob _logsScheduledManagerJob;

        public WindowsService(UnityContainer container)
        {
            _container = container;
            _fixedManagerJob = new FixedScheduledJob(_container);
            _fixtureScheduleManagerJob = new FixturesScheduledManagerJob(_container);
            _liveManagerJob = new LiveManagerJob(_container);

            _logsScheduledManagerJob = new LogsScheduledManagerJob(
                rugbyService: _container.Resolve<IRugbyService>(),
                rugbyIngestWorkerService: _container.Resolve<IRugbyIngestWorkerService>(),
                sleepTimeInMinutes: 1);
        }

        public void StartService()
        {
            GlobalConfiguration.Configuration.UseStorage(HangfireConfiguration.JobStorage);
            GlobalJobFilters.Filters.Add(new ExpirationTimeAttribute());

            using (WebApp.Start<StartUp>(ConfigurationManager.AppSettings["HangfireDashboardUrl"]))
            {
                JobStorage.Current = HangfireConfiguration.JobStorage;

                while (true)
                {
                    _fixedManagerJob.UpdateRecurringJobDefinitions();
                    _logsScheduledManagerJob.Start();

                    Thread.Sleep(2000);
                }
            }
        }

        public void StopService()
        {
            // TODO: Implement resource disposal/clean-up here.
        }
    }
}