using System.Timers;
using SuperSportDataEngine.Common.Logging;
using Unity;

namespace SuperSportDataEngine.Application.Service.SchedulerClient
{
    using Hangfire;
    using Microsoft.Owin.Hosting;
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
        private readonly MotorsportFixedScheduledJob _motorFixedScheduledJob;
        private readonly ManagerJob _jobManager;
        private readonly ILoggingService _logger;
        private System.Timers.Timer _timer;

        public WindowsService(IUnityContainer container)
        {
            _logger = container.Resolve<ILoggingService>();

            _fixedManagerJob = new FixedScheduledJob(container.CreateChildContainer());
            _motorFixedScheduledJob = new MotorsportFixedScheduledJob(container.CreateChildContainer());
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

#if (!DEBUG)
                ConfigureTimer();
#endif

                while (true)
                {
                    try
                    {
                        var rugbyEnabled = bool.Parse(ConfigurationManager.AppSettings["RugbyIngestEnabled"]);
                        if (rugbyEnabled)
                            _fixedManagerJob.UpdateRecurringJobDefinitions();

                        var motorEnabled = bool.Parse(ConfigurationManager.AppSettings["MotorsportIngestEnabled"]);
                        if (motorEnabled)
                            _motorFixedScheduledJob.UpdateRecurringJobDefinitions();
                    }
                    catch (Exception exception)
                    {
                        _logger?.Fatal(
                            "LegacyException." + exception.Message,
                            exception,
                            $"Message: {exception.Message}\n" +
                            $"StackTrace: {exception.StackTrace}" +
                            $"Inner Exception {exception.InnerException}");
                    }

                    Thread.Sleep(2000);
                }
            }
        }

        private void ConfigureTimer()
        {
            _timer = new System.Timers.Timer
            {
                AutoReset = false,
                Interval = TimeSpan.FromMinutes(20).TotalMilliseconds
            };

            _timer.Elapsed += LogProcessingJobsTakingTooLong;
            _timer.Start();
        }

        private void LogProcessingJobsTakingTooLong(object o, EventArgs args)
        {
            // This will only run if DEBUG is not defined.
            // Won't run for Debug builds (development)
            // Runs on QA, Staging and Production.
#if (!DEBUG)
            var monitorApi = JobStorage.Current.GetMonitoringApi();
            var processingJobs = monitorApi.ProcessingJobs(0, (int)monitorApi.ProcessingCount(null), null);

            foreach (var job in processingJobs)
            {
                if (job.Value.StartedAt == null)
                    continue;

                var timespanDuration = DateTime.UtcNow - job.Value.StartedAt;
                var jobName = monitorApi.JobDetails(job.Key).Properties["RecurringJobId"];

                var key = "HangfireJobProcessingTime." + jobName;

                var warningThresholdInMinutes =
                    int.Parse(ConfigurationManager.AppSettings["WarningThresholdForJobsTakingTooLongInMinutes"]);
                var errorThresholdInMinutes =
                    int.Parse(ConfigurationManager.AppSettings["ErrorThresholdForJobsTakingTooLongInMinutes"]);

                if (jobName.ToLower().Contains("motorsport"))
                {
                    warningThresholdInMinutes =
                        int.Parse(ConfigurationManager.AppSettings["MotorsportWarningThresholdForJobsTakingTooLongInMinutes"]);

                    errorThresholdInMinutes =
                        int.Parse(ConfigurationManager.AppSettings["MotorsportErrorThresholdForJobsTakingTooLongInMinutes"]);
                }

                if (timespanDuration > TimeSpan.FromMinutes(errorThresholdInMinutes))
                {
                    _logger.Error(
                        key,
                        null, 
                        $"{jobName} is taking too long to process. Error threshold is " +
                        $"{errorThresholdInMinutes} minutes. " +
                        $"Taking {timespanDuration.Value.TotalMinutes} minutes.");
                }
                else if (timespanDuration > TimeSpan.FromMinutes(warningThresholdInMinutes))
                {
                    _logger.Warn(
                        key,
                        null,
                        $"{jobName} is taking too long to process. Warning threshold is " +
                        $"{warningThresholdInMinutes} minutes. Taking " +
                        $"{timespanDuration.Value.TotalMinutes} minutes.");
                }
            }

            _timer.Start();
#endif
        }

        public void StopService()
        {
        }
    }
}