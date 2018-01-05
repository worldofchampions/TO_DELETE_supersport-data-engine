using System;
using System.Configuration;
using System.Threading;
using Hangfire;
using Hangfire.Common;
using Microsoft.Practices.Unity;
using SuperSportDataEngine.Application.Service.Common.Hangfire.Configuration;
using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
using SuperSportDataEngine.Common.Logging;

namespace SuperSportDataEngine.Application.Service.SchedulerClient.FixedSchedule
{
    public class MotorFixedScheduledJob
    {
        private readonly IRecurringJobManager _recurringJobManager;
        private readonly IUnityContainer _container;
        private readonly ILoggingService _logger;

        public MotorFixedScheduledJob(IUnityContainer container)
        {
            _container = container;
            _logger = _container.Resolve<ILoggingService>();
            _recurringJobManager = _container.Resolve<IRecurringJobManager>();
        }

        public void UpdateRecurringJobDefinitions()
        {
            UpdateRecurringJobDefinition_ReferenceData();
        }

        private void UpdateRecurringJobDefinition_ReferenceData()
        {
            _recurringJobManager.AddOrUpdate(
                ConfigurationManager.AppSettings["MotorFixedScheduledJob_ReferenceData_JobId"],
                Job.FromExpression(() => _container.Resolve<IMotorIngestWorkerService>().IngestReferenceData(CancellationToken.None)),
                ConfigurationManager.AppSettings["MotorFixedScheduledJob_ReferenceData_JobCronExpression"],
                new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.Local,
                    QueueName = HangfireQueueConfiguration.NormalPriority
                });
        }
    }
}