namespace SuperSportDataEngine.Application.Service.SchedulerClient
{
    using Microsoft.Practices.Unity;
    using SuperSportDataEngine.Application.Container;
    using Hangfire;
    using System.Threading;
    using SuperSportDataEngine.ApplicationLogic.Services;
    using Hangfire.SqlServer;
    using Hangfire.Common;
    using System.Configuration;
    using System;
    using SuperSportDataEngine.Application.Service.Common.Hangfire.Filters;

    internal class Program
    {
        private static void Main(string[] args)
        {
            // TODO: [Davide] Finalize the DI handling here after integrating with TopShelf, Hangfire etc.
            var container = new UnityContainer();
            UnityConfigurationManager.RegisterTypes(container);

            SqlServerStorageOptions Options = new SqlServerStorageOptions { PrepareSchemaIfNecessary = true };

            JobStorage JOB_STORAGE =
            new SqlServerStorage(
                    ConfigurationManager.ConnectionStrings["SqlDatabase_Hangfire"].ConnectionString,
                    Options
                );

            var ingestService = container.Resolve<IIngestWorkerService>();
            GlobalConfiguration.Configuration.UseStorage(JOB_STORAGE);

            GlobalJobFilters.Filters.Add(new ExpirationTimeAttribute());

            while(true)
            {
                JobStorage.Current = JOB_STORAGE;

                // Schedule CRON jobs here.

                // Get reference data
                RecurringJob.AddOrUpdate("ingestReferenceData", () => Console.WriteLine(ingestService.IngestReferenceData()), Cron.Minutely());
                //// Get list of active tournaments
                //var list = GetListOfActiveTournaments()

                //    for each item in list
                //    {
                //        Create new job for tournament id 
                //    }

                // Pause execution.
                Thread.Sleep(2000);
            }
        }
    }
}