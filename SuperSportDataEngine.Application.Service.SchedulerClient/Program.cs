namespace SuperSportDataEngine.Application.Service.SchedulerClient
{
    using Microsoft.Practices.Unity;
    using SuperSportDataEngine.Application.Container;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
    using System;
    using Hangfire;
    using SuperSportDataEngine.Application.Service.Common.Hangfire.Configuration;
    using SuperSportDataEngine.Application.Service.Common.Hangfire.Filters;
    using System.Threading;
    using SuperSportDataEngine.ApplicationLogic.Services;
    using Hangfire.SqlServer;

    internal class Program
    {
        private static void Main(string[] args)
        {
            // TODO: [Davide] Finalize the DI handling here after integrating with TopShelf, Hangfire etc.
            var container = new UnityContainer();
            UnityConfigurationManager.RegisterTypes(container);            

            var ingestService = container.Resolve<IIngestWorkerService>();
            GlobalConfiguration.Configuration.UseStorage(HangfireConfigurationSettings.JOB_STORAGE);

            GlobalJobFilters.Filters.Add(new ExpirationTimeAttribute());

            while(true)
            {
                JobStorage.Current = HangfireConfigurationSettings.JOB_STORAGE;

                // Schedule CRON jobs here.

                // Get reference data
                RecurringJob.AddOrUpdate("ingestReferenceData", () => ingestService.IngestReferenceData(), Cron.Minutely());
                // Get list of active tournaments

                // Pause execution.
                Thread.Sleep(2000);
            }
        }
    }
}