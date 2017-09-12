using Hangfire;
using Microsoft.Practices.Unity;
using SuperSportDataEngine.Application.Service.Common.Hangfire.Configuration;
using SuperSportDataEngine.ApplicationLogic.Services;
using System.Configuration;
using System.Threading;

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
            UpdateReccuringJobDefinition_ReferenceData();
            UpdateReccuringJobDefinition_Fixtures();
            UpdateReccuringJobDefinition_Logs();
        }

        private void UpdateReccuringJobDefinition_ReferenceData()
        {
            // Create a schedule for getting the 
            // reference data from the provider.
            RecurringJob.AddOrUpdate(
                ConfigurationManager.AppSettings["FixedScheduledJob_ReferenceData_JobId"],
                () => _ingestService.IngestRugbyReferenceData(CancellationToken.None),
                ConfigurationManager.AppSettings["FixedScheduledJob_ReferenceData_JobCronExpression"],
                System.TimeZoneInfo.Utc,
                HangfireQueueConfiguration.NormalPriority);
        }

        private void UpdateReccuringJobDefinition_Fixtures()
        {
            // Create a schedule for getting the
            // fixtures for the active tournaments for the current year from the provider.
            RecurringJob.AddOrUpdate(
                ConfigurationManager.AppSettings["FixedScheduledJob_FixturesData_JobId"],
                () => _ingestService.IngestFixturesForActiveTournaments(CancellationToken.None),
                ConfigurationManager.AppSettings["FixedScheduledJob_FixturesData_JobCronExpression"],
                System.TimeZoneInfo.Utc,
                HangfireQueueConfiguration.NormalPriority);

            UpdateReccuringJobDefinition_AllFixtures();
        }

        private void UpdateReccuringJobDefinition_Logs()
        {
            // Create a schedule for getting the
            // fixtures for active tournaments for the current year from the provider.
            RecurringJob.AddOrUpdate(
                ConfigurationManager.AppSettings["FixedScheduledJob_LogsData_JobId"],
                () => _ingestService.IngestLogsForActiveTournaments(CancellationToken.None),
                ConfigurationManager.AppSettings["FixedScheduledJob_LogsData_JobCronExpression"],
                System.TimeZoneInfo.Utc,
                HangfireQueueConfiguration.NormalPriority);
        }

        /// <summary>
        /// Creates or updates a fixed schedule job for fetcthing results data from the provider.
        /// </summary>
        private void UpdateReccuringJobDefinition_AllFixtures()
        {
            RecurringJob.AddOrUpdate(
                recurringJobId: ConfigurationManager.AppSettings["FixedScheduledJob_ResultsData_JobId"],
                methodCall: () => _ingestService.IngestRugbyResultsForAllFixtures(CancellationToken.None),
                cronExpression: ConfigurationManager.AppSettings["FixedScheduledJob_ResultsData_JobCronExpression"],
                timeZone: System.TimeZoneInfo.Utc,
                queue: HangfireQueueConfiguration.NormalPriority);
        }
    }
}
