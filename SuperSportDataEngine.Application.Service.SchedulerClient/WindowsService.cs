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
    using SuperSportDataEngine.Application.Service.SchedulerClient.Manager;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;

    internal class WindowsService : IWindowsServiceContract
    {
        private readonly UnityContainer _container;
        private readonly FixedScheduledJob _fixedManagerJob;
        private readonly ManagerJob _jobManager;

        public WindowsService(UnityContainer container)
        {
            _container = container;
            _fixedManagerJob = new FixedScheduledJob(_container);
            _jobManager = new ManagerJob(_container);
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