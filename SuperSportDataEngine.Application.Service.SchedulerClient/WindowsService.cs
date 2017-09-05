namespace SuperSportDataEngine.Application.Service.SchedulerClient
{
    using Hangfire;
    using Microsoft.Owin.Hosting;
    using Microsoft.Practices.Unity;
    using SuperSportDataEngine.Application.Service.Common.Hangfire.Configuration;
    using SuperSportDataEngine.Application.Service.Common.Hangfire.Filters;
    using SuperSportDataEngine.Application.Service.Common.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Services;
    using System.Configuration;
    using System.Threading;

    internal class WindowsService : IWindowsServiceContract
    {
        private readonly UnityContainer _container;

        public WindowsService(UnityContainer container)
        {
            _container = container;
        }

        public void StartService()
        {
            var ingestService = _container.Resolve<IRugbyIngestWorkerService>();

            GlobalConfiguration.Configuration.UseStorage(HangfireConfiguration.JobStorage);
            GlobalJobFilters.Filters.Add(new ExpirationTimeAttribute());

            using (WebApp.Start<StartUp>(ConfigurationManager.AppSettings["HangfireDashboardUrl"]))
            {
                while (true)
                {
                    JobStorage.Current = HangfireConfiguration.JobStorage;

                    // Schedule CRON jobs here.

                    // Get reference data
                    RecurringJob.AddOrUpdate("ingestReferenceData", () => ingestService.IngestRugbyReferenceData(), Cron.Minutely(), System.TimeZoneInfo.Utc, HangfireQueueConfiguration.NormalPriority);

                    // Pause execution.
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