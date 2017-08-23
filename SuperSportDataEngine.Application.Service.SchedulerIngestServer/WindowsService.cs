namespace SuperSportDataEngine.Application.Service.SchedulerIngestServer
{
    using Hangfire;
    using Hangfire.SqlServer;
    using Microsoft.Practices.Unity;
    using SuperSportDataEngine.Application.Service.Common.Hangfire.Filters;
    using SuperSportDataEngine.Application.Service.Common.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Services;
    using System;
    using System.Configuration;

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

            using (var server = new BackgroundJobServer())
            {
                JobStorage.Current = JOB_STORAGE;
                // Processing of jobs happen here.

                Console.ReadLine();
            }
        }

        public void StopService()
        {
            // TODO: Implement resource disposal/clean-up here.
        }
    }
}