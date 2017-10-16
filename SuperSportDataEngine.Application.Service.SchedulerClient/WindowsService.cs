namespace SuperSportDataEngine.Application.Service.SchedulerClient
{
    using Hangfire;
    using Microsoft.Owin.Hosting;
    using Microsoft.Practices.Unity;
    using SuperSportDataEngine.Application.Service.Common.Hangfire.Configuration;
    using SuperSportDataEngine.Application.Service.Common.Hangfire.Filters;
    using SuperSportDataEngine.Application.Service.Common.Interfaces;
    using SuperSportDataEngine.Application.Service.SchedulerClient.FixedSchedule;
    using System.Configuration;
    using System.Threading;
    using SuperSportDataEngine.Application.Service.SchedulerClient.Manager;
    using System.Threading.Tasks;
    using System;

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
            Task.Run(() => { DoServiceWork(); });
        }

        private void DoServiceWork()
        {
            GlobalConfiguration.Configuration.UseStorage(HangfireConfiguration.JobStorage);
            GlobalJobFilters.Filters.Add(new ExpirationTimeAttribute());

            var options = new StartOptions();
            options.Urls.Add(ConfigurationManager.AppSettings["HangfireDashboardUrl"]);
            options.Urls.Add("http://localhost:9622");
            options.Urls.Add("http://127.0.0.1:9622");
            options.Urls.Add(string.Format("http://{0}:9622", Environment.MachineName));

            using (WebApp.Start<StartUp>(options))
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