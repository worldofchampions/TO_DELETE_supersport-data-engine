using Hangfire;
using Microsoft.Practices.Unity;
using SuperSportDataEngine.Application.Service.Common.Hangfire.Configuration;
using SuperSportDataEngine.ApplicationLogic.Services;
using System.Configuration;
using System.Threading;
using System;
using Hangfire.Common;
using SuperSportDataEngine.Application.Service.Common.Interfaces;

namespace SuperSportDataEngine.Application.Service.SchedulerClient.FixedSchedule
{
    public class FixedScheduledJob
    {
        private readonly IRugbyIngestWorkerService _ingestService;
        private readonly IRecurringJobManager _recurringJobManager;

        public FixedScheduledJob(IUnityContainer container)
        {
            _ingestService = container.Resolve<IRugbyIngestWorkerService>();
            _recurringJobManager = container.Resolve<IRecurringJobManager>();
        }

        public void UpdateRecurringJobDefinitions()
        {
            UpdateRecurringJobDefinition_ReferenceData();
            UpdateRecurringJobDefinition_Fixtures();
            UpdateRecurringJobDefinition_LogsForActiveTournaments();
            UpdateRecurringJobDefinition_LogsForCurrentTournaments();
            UpdateRecurringJobDefinition_AllFixtures();
            UpdateRecurringJobDefinition_ResultsForCurrentDayFixtures();
            UpdateRecurringJobDefinition_ResultsForFixturesInResultsState();
        }

        /// <summary>
        /// Runs hourly for the next 3 days to fetch results for ended matches.
        /// </summary>
        private void UpdateRecurringJobDefinition_ResultsForFixturesInResultsState()
        {
            _recurringJobManager.AddOrUpdate(
                ConfigurationManager.AppSettings["FixedScheduledJob_ResultsData_Hourly_JobId"],
                Job.FromExpression(() => _ingestService.IngestResultsForFixturesInResultsState(CancellationToken.None)),
                ConfigurationManager.AppSettings["FixedScheduledJob_ResultsData_Hourly_PollingExpression"],
                new RecurringJobOptions() {
                    TimeZone = TimeZoneInfo.Utc,
                    QueueName = HangfireQueueConfiguration.HighPriority
                });
        }

        private void UpdateRecurringJobDefinition_ReferenceData()
        {
            // Create a schedule for getting the 
            // reference data from the provider.
            _recurringJobManager.AddOrUpdate(
                ConfigurationManager.AppSettings["FixedScheduledJob_ReferenceData_JobId"],
                Job.FromExpression(() => _ingestService.IngestReferenceData(CancellationToken.None)),
                ConfigurationManager.AppSettings["FixedScheduledJob_ReferenceData_JobCronExpression"],
                new RecurringJobOptions()
                {
                    TimeZone = TimeZoneInfo.Utc,
                    QueueName = HangfireQueueConfiguration.HighPriority
                });
        }

        private void UpdateRecurringJobDefinition_Fixtures()
        {
            // Create a schedule for getting the
            // fixtures for the active tournaments for the current year from the provider.
            _recurringJobManager.AddOrUpdate(
                ConfigurationManager.AppSettings["FixedScheduledJob_FixturesData_JobId"],
                Job.FromExpression(() => _ingestService.IngestFixturesForActiveTournaments(CancellationToken.None)),
                ConfigurationManager.AppSettings["FixedScheduledJob_FixturesData_JobCronExpression"],
                new RecurringJobOptions()
                {
                    TimeZone = TimeZoneInfo.Utc,
                    QueueName = HangfireQueueConfiguration.NormalPriority
                });

            UpdateRecurringJobDefinition_AllFixtures();
        }

        private void UpdateRecurringJobDefinition_LogsForActiveTournaments()
        {
            // Create a schedule for getting the
            // logs for active tournaments for the current year from the provider.
            _recurringJobManager.AddOrUpdate(
                ConfigurationManager.AppSettings["FixedScheduledJob_LogsData_ActiveTournaments_JobId"],
                Job.FromExpression(() => _ingestService.IngestLogsForActiveTournaments(CancellationToken.None)),
                ConfigurationManager.AppSettings["FixedScheduledJob_LogsData_ActiveTournaments_JobCronExpression"],
                new RecurringJobOptions()
                {
                    TimeZone = TimeZoneInfo.Utc,
                    QueueName = HangfireQueueConfiguration.NormalPriority
                });
        }

        private void UpdateRecurringJobDefinition_LogsForCurrentTournaments()
        {
            // Create a schedule for getting the
            // logs for current tournaments for the current season from the provider.
            _recurringJobManager.AddOrUpdate(
                ConfigurationManager.AppSettings["FixedScheduledJob_LogsData_CurrentTournaments_JobId"],
                Job.FromExpression(() => _ingestService.IngestLogsForCurrentTournaments(CancellationToken.None)),
                ConfigurationManager.AppSettings["FixedScheduledJob_LogsData_CurrentTournaments_JobCronExpression"],
                TimeZoneInfo.Utc,
                HangfireQueueConfiguration.NormalPriority);
        }

        /// <summary>
        /// Creates or updates a fixed schedule job for fetcthing results data from the provider.
        /// </summary>
        private void UpdateRecurringJobDefinition_AllFixtures()
        {
            _recurringJobManager.AddOrUpdate(
                ConfigurationManager.AppSettings["FixedScheduledJob_ResultsData_Daily_JobId"],
                Job.FromExpression(() => _ingestService.IngestResultsForAllFixtures(CancellationToken.None)),
                ConfigurationManager.AppSettings["FixedScheduledJob_ResultsData_Daily_PollingExpression"],
                new RecurringJobOptions()
                {
                    TimeZone = TimeZoneInfo.Utc,
                    QueueName = HangfireQueueConfiguration.NormalPriority
                });
        }

        /// <summary>
        /// Runs every 15 minutes to fetch match results on fixture day.
        /// </summary>
        private void UpdateRecurringJobDefinition_ResultsForCurrentDayFixtures()
        {
            _recurringJobManager.AddOrUpdate(
                ConfigurationManager.AppSettings["FixedScheduledJob_ResultsData_OnFixtureDay_JobId"],
                Job.FromExpression(() => _ingestService.IngestResultsForCurrentDayFixtures(CancellationToken.None)),
                ConfigurationManager.AppSettings["FixedScheduledJob_ResultsData_OnFixtureDay_PollingExpression"],
                new RecurringJobOptions()
                {
                    TimeZone = TimeZoneInfo.Utc,
                    QueueName = HangfireQueueConfiguration.HighPriority
                });
        }
    }
}
