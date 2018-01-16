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
        private readonly FixedScheduledJob _fixedManagerJob;
        private readonly ManagerJob _jobManager;
        private readonly ILoggingService _logger;

        public WindowsService(IUnityContainer container)
        {
            _logger = container.Resolve<ILoggingService>();

            _fixedManagerJob = new FixedScheduledJob(container.CreateChildContainer());
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
                    }
                    catch (Exception e)
                    {
                        _logger.Info("UpdateRecurringJobDefinitions.ThrowsException", e.StackTrace);
                    }

                    LogProcessingJobsTakingTooLong();

                    Thread.Sleep(2000);
                }
            }
        }

        private void LogProcessingJobsTakingTooLong()
        {
            // This will only run if DEBUG is not defined.
            // Won't run for Debug builds (development)
            // Runs on QA, Staging and Production.
#if (!DEBUG)
            var monitorApi = JobStorage.Current.GetMonitoringApi();
            var processingJobs = monitorApi.ProcessingJobs(0, (int) monitorApi.ProcessingCount());

            foreach (var job in processingJobs)
            {
                if (job.Value.StartedAt == null)
                    continue;

                var timespanDuration = DateTime.UtcNow - job.Value.StartedAt;
                var key = job.Value.Job.Method + "HangfireJobProcessingTime";
                var jobName = monitorApi.JobDetails(job.Key).Properties["RecurringJobId"];
                var message = jobName + "is taking too long to process. Taking " +
                              timespanDuration.Value.TotalMinutes + " minutes.";

                var warningThresholdInMinutes =
                    int.Parse(ConfigurationManager.AppSettings["WarningThresholdForJobsTakingTooLongInMinutes"]);
                var errorThresholdInMinutes =
                    int.Parse(ConfigurationManager.AppSettings["ErrorThresholdForJobsTakingTooLongInMinutes"]);

                if (timespanDuration > TimeSpan.FromMinutes(errorThresholdInMinutes))
                    _logger.Error(key, message);
                else if (timespanDuration > TimeSpan.FromMinutes(warningThresholdInMinutes))
                    _logger.Warn(key, message);
            }
#endif
        }

        public void StopService()
        {
            // TODO: Implement resource disposal/clean-up here.
        }
    }
}