namespace SuperSportDataEngine.Application.Service.SchedulerClient
{
    using Hangfire;
    using Hangfire.SqlServer;
    using Microsoft.Owin.Hosting;
    using Microsoft.Practices.Unity;
    using SuperSportDataEngine.Application.Service.Common.Hangfire.Filters;
    using SuperSportDataEngine.Application.Service.Common.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Services;
    using System;
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
            SqlServerStorageOptions Options = new SqlServerStorageOptions { PrepareSchemaIfNecessary = true };

            JobStorage JOB_STORAGE =
            new SqlServerStorage(
                    ConfigurationManager.ConnectionStrings["SqlDatabase_Hangfire"].ConnectionString,
                    Options
                );

            var ingestService = _container.Resolve<IRugbyIngestWorkerService>();
            GlobalConfiguration.Configuration.UseStorage(JOB_STORAGE);

            GlobalJobFilters.Filters.Add(new ExpirationTimeAttribute());

            using (WebApp.Start<StartUp>("http://localhost:9622"))
            {
                while (true)
                {
                    JobStorage.Current = JOB_STORAGE;

                    // Schedule CRON jobs here.

                    // Get reference data
                    RecurringJob.AddOrUpdate("ingestReferenceData", () => ingestService.IngestRugbyReferenceData(), Cron.Minutely());

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