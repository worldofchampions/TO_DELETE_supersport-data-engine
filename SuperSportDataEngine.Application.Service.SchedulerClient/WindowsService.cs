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
                JobStorage.Current = HangfireConfiguration.JobStorage;

                while (true)
                {
                    // Schedule CRON jobs here.

                    // Get reference data
                    RecurringJob.AddOrUpdate(
                        "ingestReferenceData", 
                        () => ingestService.IngestRugbyReferenceData(CancellationToken.None), 
                        Cron.Minutely(), 
                        System.TimeZoneInfo.Utc, 
                        HangfireQueueConfiguration.NormalPriority);

                    // Pause execution.
                    // We will end up moving the Sleep to the manager classes.
                    // The manager classes will have the cancellation token passed to them.
                    // eg. FixedManagerJob.RunJobs(cancellationToken);
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