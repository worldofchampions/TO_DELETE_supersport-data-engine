namespace SuperSportDataEngine.Application.Service.SchedulerClient.FixedSchedule
{
    using System;
    using System.Configuration;
    using System.Threading;
    using Hangfire;
    using Hangfire.Common;
    using Microsoft.Practices.Unity;
    using SuperSportDataEngine.Application.Service.Common.Hangfire.Configuration;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
    using SuperSportDataEngine.Common.Logging;

    public class MotorsportFixedScheduledJob
    {
        private readonly IRecurringJobManager _recurringJobManager;
        private readonly IUnityContainer _container;
        private readonly ILoggingService _logger;

        public MotorsportFixedScheduledJob(IUnityContainer container)
        {
            _container = container;
            _logger = _container.Resolve<ILoggingService>();
            _recurringJobManager = _container.Resolve<IRecurringJobManager>();
        }

        public void UpdateRecurringJobDefinitions()
        {
            UpdateRecurringJobDefinition_Leagues();
            UpdateRecurringJobDefinition_RacesCalendar();
            UpdateRecurringJobDefinition_Drivers();
            CreateManualJobDefinition_HistoricGridData();
        }

        private void UpdateRecurringJobDefinition_RacesCalendar()
        {
            _recurringJobManager.AddOrUpdate(
                ConfigurationManager.AppSettings["MotorsportFixedScheduledJob_Calendar_JobId"],
                Job.FromExpression(() => _container.Resolve<IMotorsportIngestWorkerService>().IngestRacesForActiveLeagues(CancellationToken.None)),
                ConfigurationManager.AppSettings["MotorsportFixedScheduledJob_Calendar_JobCronExpression"],
                new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.Local,
                    QueueName = HangfireQueueConfiguration.NormalPriority
                });
        }

        private void UpdateRecurringJobDefinition_Leagues()
        {
            _recurringJobManager.AddOrUpdate(
                ConfigurationManager.AppSettings["MotorsportFixedScheduledJob_Leagues_JobId"],
                Job.FromExpression(() => _container.Resolve<IMotorsportIngestWorkerService>().IngestLeagues(CancellationToken.None)),
                ConfigurationManager.AppSettings["MotorsportFixedScheduledJob_Leagues_JobCronExpression"],
                new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.Local,
                    QueueName = HangfireQueueConfiguration.NormalPriority
                });
        }

        private void UpdateRecurringJobDefinition_Drivers()
        {
            _recurringJobManager.AddOrUpdate(
                ConfigurationManager.AppSettings["MotorsportFixedScheduledJob_Drivers_JobId"],
                Job.FromExpression(() => _container.Resolve<IMotorsportIngestWorkerService>().IngestDriversForActiveLeagues(CancellationToken.None)),
                ConfigurationManager.AppSettings["MotorsportFixedScheduledJob_Drivers_JobCronExpression"],
                new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.Local,
                    QueueName = HangfireQueueConfiguration.NormalPriority
                });
        }

        private void CreateManualJobDefinition_HistoricGridData()
        {
            _recurringJobManager.AddOrUpdate(
                ConfigurationManager.AppSettings["MotorsportFixedScheduledJob_HistoricGrids_JobId"],
                Job.FromExpression(() => _container.Resolve<IMotorsportIngestWorkerService>().IngestRaceGridsForPastSeasons(CancellationToken.None)),
                ConfigurationManager.AppSettings["MotorsportFixedScheduledJob_HistoricGrids_JobCronExpression"],
                new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.Local,
                    QueueName = HangfireQueueConfiguration.NormalPriority
                });
        }
    }
}