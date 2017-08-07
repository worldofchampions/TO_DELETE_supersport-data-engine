using System;
using Hangfire;
using SuperSportDataEngine.Application.Service.Common.Hangfire.Configuration;
using SuperSportDataEngine.Application.Service.Common.Hangfire.Filters;

namespace SuperSportDataEngine.Application.Service.SchedulerIngestServer
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            GlobalConfiguration.Configuration.UseStorage(HangfireConfigurationSettings.JOB_STORAGE);
            GlobalJobFilters.Filters.Add(new ExpirationTimeAttribute());

            using (var server = new BackgroundJobServer())
            {
                JobStorage.Current = HangfireConfigurationSettings.JOB_STORAGE;

                // Processing of jobs happen here.

                Console.ReadLine();
            }
        }
    }
}