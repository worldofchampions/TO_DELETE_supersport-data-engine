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
            // Create a schedule for getting the 
            // reference data from the provider.
            RecurringJob.AddOrUpdate(
                ConfigurationManager.AppSettings["FixedScheduledJob_ReferenceData_JobId"],
                () => _ingestService.IngestRugbyReferenceData(CancellationToken.None),
                ConfigurationManager.AppSettings["FixedScheduledJob_ReferenceData_JobCronExpression"],
                System.TimeZoneInfo.Utc,
                HangfireQueueConfiguration.NormalPriority);

            // Create a schedule for getting the
            // fixtures for the active tournaments from the provider.
            RecurringJob.AddOrUpdate(
                ConfigurationManager.AppSettings["FixedScheduledJob_FixturesData_JobId"],
                () => _ingestService.IngestFixturesForActiveTournaments(CancellationToken.None),
                ConfigurationManager.AppSettings["FixedScheduledJob_FixturesData_JobCronExpression"],
                System.TimeZoneInfo.Utc,
                HangfireQueueConfiguration.NormalPriority);
        }
    }
}
