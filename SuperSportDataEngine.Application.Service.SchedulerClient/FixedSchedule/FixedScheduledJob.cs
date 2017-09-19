using Hangfire;
using Microsoft.Practices.Unity;
using SuperSportDataEngine.Application.Service.Common.Hangfire.Configuration;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models.Enums;
using SuperSportDataEngine.ApplicationLogic.Services;
using System.Configuration;
using System.Threading;
using System;

namespace SuperSportDataEngine.Application.Service.SchedulerClient.FixedSchedule
{
    class FixedScheduledJob
    {
        private readonly IRugbyIngestWorkerService _ingestService;

        public FixedScheduledJob(UnityContainer container)
        {
            _ingestService = container.Resolve<IRugbyIngestWorkerService>();
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
            RecurringJob.AddOrUpdate(
                recurringJobId: ConfigurationManager.AppSettings["FixedScheduledJob_ResultsData_Hourly_JobId"],
                methodCall: () => _ingestService.IngestResultsForFixturesInResultsState(CancellationToken.None),
                cronExpression: ConfigurationManager.AppSettings["FixedScheduledJob_ResultsData_Hourly_PollingExpression"],
                timeZone: TimeZoneInfo.Utc,
                queue: HangfireQueueConfiguration.HighPriority);
        }

        private void UpdateRecurringJobDefinition_ReferenceData()
        {
            // Create a schedule for getting the 
            // reference data from the provider.
            RecurringJob.AddOrUpdate(
                ConfigurationManager.AppSettings["FixedScheduledJob_ReferenceData_JobId"],
                () => _ingestService.IngestReferenceData(CancellationToken.None),
                ConfigurationManager.AppSettings["FixedScheduledJob_ReferenceData_JobCronExpression"],
                System.TimeZoneInfo.Utc,
                HangfireQueueConfiguration.NormalPriority);
        }

        private void UpdateRecurringJobDefinition_Fixtures()
        {
            // Create a schedule for getting the
            // fixtures for the active tournaments for the current year from the provider.
            RecurringJob.AddOrUpdate(
                ConfigurationManager.AppSettings["FixedScheduledJob_FixturesData_JobId"],
                () => _ingestService.IngestFixturesForActiveTournaments(CancellationToken.None),
                ConfigurationManager.AppSettings["FixedScheduledJob_FixturesData_JobCronExpression"],
                System.TimeZoneInfo.Utc,
                HangfireQueueConfiguration.NormalPriority);

            UpdateRecurringJobDefinition_AllFixtures();
        }

        private void UpdateRecurringJobDefinition_LogsForActiveTournaments()
        {
            // Create a schedule for getting the
            // logs for active tournaments for the current year from the provider.
            RecurringJob.AddOrUpdate(
                ConfigurationManager.AppSettings["FixedScheduledJob_LogsData_ActiveTournaments_JobId"],
                () => _ingestService.IngestLogsForActiveTournaments(CancellationToken.None),
                ConfigurationManager.AppSettings["FixedScheduledJob_LogsData_ActiveTournaments_JobCronExpression"],
                System.TimeZoneInfo.Utc,
                HangfireQueueConfiguration.NormalPriority);
        }

        private void UpdateRecurringJobDefinition_LogsForCurrentTournaments()
        {
            // Create a schedule for getting the
            // logs for current tournaments for the current season from the provider.
            RecurringJob.AddOrUpdate(
                ConfigurationManager.AppSettings["FixedScheduledJob_LogsData_CurrentTournaments_JobId"],
                () => _ingestService.IngestLogsForCurrentTournaments(CancellationToken.None),
                ConfigurationManager.AppSettings["FixedScheduledJob_LogsData_CurrentTournaments_JobCronExpression"],
                System.TimeZoneInfo.Utc,
                HangfireQueueConfiguration.NormalPriority);
        }

        /// <summary>
        /// Creates or updates a fixed schedule job for fetcthing results data from the provider.
        /// </summary>
        private void UpdateRecurringJobDefinition_AllFixtures()
        {
            RecurringJob.AddOrUpdate(
                recurringJobId: ConfigurationManager.AppSettings["FixedScheduledJob_ResultsData_Daily_JobId"],
                methodCall: () => _ingestService.IngestResultsForAllFixtures(CancellationToken.None),
                cronExpression: ConfigurationManager.AppSettings["FixedScheduledJob_ResultsData_Daily_PollingExpression"],
                timeZone: TimeZoneInfo.Utc,
                queue: HangfireQueueConfiguration.NormalPriority);
        }

        /// <summary>
        /// Runs every 15 minutes to fetch match results on fixture day.
        /// </summary>
        private void UpdateRecurringJobDefinition_ResultsForCurrentDayFixtures()
        {
            RecurringJob.AddOrUpdate(
                recurringJobId: ConfigurationManager.AppSettings["FixedScheduledJob_ResultsData_OnFixtureDay_JobId"],
                methodCall: () => _ingestService.IngestResultsForCurrentDayFixtures(CancellationToken.None),
                cronExpression: ConfigurationManager.AppSettings["FixedScheduledJob_ResultsData_OnFixtureDay_PollingExpression"],
                timeZone: TimeZoneInfo.Utc,
                queue: HangfireQueueConfiguration.HighPriority);
        }
    }
}
