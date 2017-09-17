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

    internal class WindowsService : IWindowsServiceContract
    {
        private readonly UnityContainer _container;
        private readonly FixedScheduledJob _fixedManagerJob;
        private readonly FixturesScheduledManagerJob _fixtureScheduleManagerJob;
        private readonly LiveManagerJob _liveManagerJob;

        public WindowsService(UnityContainer container)
        {
            _container = container;
            _fixedManagerJob = new FixedScheduledJob(_container);
            _fixtureScheduleManagerJob = new FixturesScheduledManagerJob(_container);
            _liveManagerJob = new LiveManagerJob(_container);
        }

        public void StartService()
        {
            var ingestService = _container.Resolve<IRugbyIngestWorkerService>();

            GlobalConfiguration.Configuration.UseStorage(HangfireConfiguration.JobStorage);
            GlobalJobFilters.Filters.Add(new ExpirationTimeAttribute());

            using (WebApp.Start<StartUp>(ConfigurationManager.AppSettings["HangfireDashboardUrl"]))
            {
                JobStorage.Current = HangfireConfiguration.JobStorage;

                while (true)
                {
                    _fixedManagerJob.UpdateRecurringJobDefinitions();

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