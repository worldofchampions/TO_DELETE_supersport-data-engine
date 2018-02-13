using System.Collections.Generic;
using System.Linq;
using System.Timers;
using SuperSportDataEngine.Application.WebApi.Common.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models;
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
        private readonly IUnityContainer _container;
        private readonly FixedScheduledJob _fixedManagerJob;
        private readonly ManagerJob _jobManager;
        private readonly ILoggingService _logger;
        private System.Timers.Timer _timer;

        public WindowsService(IUnityContainer container)
        {
            _logger = container.Resolve<ILoggingService>();
            _container = container;
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

#if (!DEBUG)
                ConfigureTimer();
#endif

                while (true)
                {
                    try
                    {
                        _fixedManagerJob.UpdateRecurringJobDefinitions();

                        UpdateAuthKeysInCache();
                    }
                    catch (Exception e)
                    {
                        _logger.Info("UpdateRecurringJobDefinitions.ThrowsException", e.StackTrace);
                    }

                    Thread.Sleep(2000);
                }
            }
        }

        private async void UpdateAuthKeysInCache()
        {
            var cache = _container.Resolve<ICache>();
            if (cache == null) return;

            var legacyAuthFeedConsumer = _container.Resolve<IBaseEntityFrameworkRepository<LegacyAuthFeedConsumer>>();

            cache.Add(
                "AUTH_KEYS", 
                legacyAuthFeedConsumer.All().ToList(), 
                TimeSpan.FromDays(
                    int.Parse(
                        ConfigurationManager.AppSettings[
                            "NumberOfDaysToKeepAuthKeys"])));

            var keysFromCache = await cache.GetAsync<List<LegacyAuthFeedConsumer>>("AUTH_KEYS");

            if (keysFromCache == null)
            {
                await _logger.Info(
                    "LogNoAuthKeys.",
                    "No Auth keys added to cache.");
                return;
            }

            foreach (var authFeedConsumer in keysFromCache)
            {
                await _logger.Info(
                    "LogAddedAuthKey." + authFeedConsumer.AuthKey,
                    "Added auth key: " + authFeedConsumer.AuthKey);
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
            var processingJobs = monitorApi.ProcessingJobs(0, (int) monitorApi.ProcessingCount());

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

                if (timespanDuration > TimeSpan.FromMinutes(errorThresholdInMinutes))
                    _logger.Error(key, jobName + "is taking too long to process. Error threshold is " + errorThresholdInMinutes + "minutes. Taking " + timespanDuration.Value.TotalMinutes + " minutes.");
                else if (timespanDuration > TimeSpan.FromMinutes(warningThresholdInMinutes))
                    _logger.Warn(key, jobName + "is taking too long to process. Warning threshold is " + warningThresholdInMinutes + "minutes. Taking " + timespanDuration.Value.TotalMinutes + " minutes.");
            }

            _timer.Start();
#endif
        }

        public void StopService()
        {
        }
    }
}