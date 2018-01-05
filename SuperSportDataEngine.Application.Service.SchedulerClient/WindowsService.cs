using SuperSportDataEngine.Common.Logging;

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
        private readonly MotorFixedScheduledJob _motorFixedScheduledJob;
        private readonly ManagerJob _jobManager;
        private ILoggingService _logger;

        public WindowsService(UnityContainer container)
        {
            _container = container;
            _logger = container.Resolve<ILoggingService>();

            _fixedManagerJob = new FixedScheduledJob(_container.CreateChildContainer());
            _motorFixedScheduledJob = new MotorFixedScheduledJob(_container.CreateChildContainer());
            _jobManager = new ManagerJob();
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
            options.Urls.Add($"http://{Environment.MachineName}:9622");

            using (WebApp.Start<StartUp>(options))
            {
                JobStorage.Current = HangfireConfiguration.JobStorage;

                while (true)
                {
                    try
                    {
                        _fixedManagerJob.UpdateRecurringJobDefinitions();
                        _motorFixedScheduledJob.UpdateRecurringJobDefinitions();
                    }
                    catch (Exception e)
                    {
                        _logger.Info("UpdateRecurringJobDefinitions.ThrowsException", e.StackTrace);
                    }

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