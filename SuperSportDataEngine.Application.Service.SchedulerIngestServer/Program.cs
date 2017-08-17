using System;
using Hangfire;
using Hangfire.SqlServer;
using System.Configuration;
using SuperSportDataEngine.Application.Service.Common.Hangfire.Filters;
using SuperSportDataEngine.ApplicationLogic.Services;
using SuperSportDataEngine.Application.Container;
using Microsoft.Practices.Unity;

namespace SuperSportDataEngine.Application.Service.SchedulerIngestServer
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            SqlServerStorageOptions Options = new SqlServerStorageOptions { PrepareSchemaIfNecessary = false };

            JobStorage JOB_STORAGE =
            new SqlServerStorage(
                    ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString,
                    Options
                );

            var container = new UnityContainer();
            UnityConfigurationManager.RegisterTypes(container);
            var ingestService = container.Resolve<IIngestWorkerService>();

            GlobalConfiguration.Configuration.UseStorage(JOB_STORAGE);
            GlobalJobFilters.Filters.Add(new ExpirationTimeAttribute());

            using (var server = new BackgroundJobServer())
            {
                JobStorage.Current = JOB_STORAGE;
                // Processing of jobs happen here.

                Console.ReadLine();
            }
        }
    }
}