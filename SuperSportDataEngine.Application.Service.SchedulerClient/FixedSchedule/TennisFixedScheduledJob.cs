using System;
using System.Configuration;
using System.Threading;
using Hangfire;
using Hangfire.Common;
using SuperSportDataEngine.Application.Service.Common.Hangfire.Configuration;
using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
using Unity;

namespace SuperSportDataEngine.Application.Service.SchedulerClient.FixedSchedule
{
    public class TennisFixedScheduledJob
    {
        private readonly IRecurringJobManager _recurringJobManager;
        private readonly IUnityContainer _container;

        public TennisFixedScheduledJob(
            IUnityContainer container)
        {
            _container = container;
            _recurringJobManager = _container.Resolve<IRecurringJobManager>();
        }

        public void UpdateRecurringJobDefinitions()
        {
            _recurringJobManager.AddOrUpdate(
                ConfigurationManager.AppSettings["TennisFixedScheduledJob_ReferenceData_JobId"],
                Job.FromExpression(() => _container.Resolve<ITennisIngestWorkerService>().IngestReferenceData(CancellationToken.None)),
                ConfigurationManager.AppSettings["TennisFixedScheduledJob_ReferenceData_JobCronExpression"],
                new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.Local,
                    QueueName = HangfireQueueConfiguration.NormalPriority
                });

            _recurringJobManager.AddOrUpdate(
                ConfigurationManager.AppSettings["TennisFixedScheduledJob_CalendarData_JobId"],
                Job.FromExpression(() => _container.Resolve<ITennisIngestWorkerService>().IngestCalendarsForEnabledLeagues(CancellationToken.None)),
                ConfigurationManager.AppSettings["TennisFixedScheduledJob_CalendarData_JobCronExpression"],
                new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.Local,
                    QueueName = HangfireQueueConfiguration.NormalPriority
                });

            _recurringJobManager.AddOrUpdate(
                ConfigurationManager.AppSettings["TennisFixedScheduledJob_ResultsData_JobId"],
                Job.FromExpression(() => _container.Resolve<ITennisIngestWorkerService>().IngestResultsForEnabledLeagues(CancellationToken.None)),
                ConfigurationManager.AppSettings["TennisFixedScheduledJob_ResultsData_JobCronExpression"],
                new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.Local,
                    QueueName = HangfireQueueConfiguration.NormalPriority
                });

            _recurringJobManager.AddOrUpdate(
                ConfigurationManager.AppSettings["TennisFixedScheduledJob_RankingsData_JobId"],
                Job.FromExpression(() => _container.Resolve<ITennisIngestWorkerService>().IngestRankingsForEnabledLeagues_Helper(CancellationToken.None)),
                ConfigurationManager.AppSettings["TennisFixedScheduledJob_RankingsData_JobCronExpression"],
                new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.Local,
                    QueueName = HangfireQueueConfiguration.NormalPriority
                });

            _recurringJobManager.AddOrUpdate(
                ConfigurationManager.AppSettings["TennisFixedScheduledJob_RaceRankingsData_JobId"],
                Job.FromExpression(() => _container.Resolve<ITennisIngestWorkerService>().IngestRaceRankingsForEnabledLeagues(CancellationToken.None)),
                ConfigurationManager.AppSettings["TennisFixedScheduledJob_RaceRankingsData_JobCronExpression"],
                new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.Local,
                    QueueName = HangfireQueueConfiguration.NormalPriority
                });

            _recurringJobManager.AddOrUpdate(
                ConfigurationManager.AppSettings["TennisFixedScheduledJob_HistoricCalendarData_JobId"],
                Job.FromExpression(() => _container.Resolve<ITennisIngestWorkerService>().IngestHistoricCalendarsForEnabledLeagues(CancellationToken.None)),
                ConfigurationManager.AppSettings["TennisFixedScheduledJob_HistoricCalendarData_JobCronExpression"],
                new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.Local,
                    QueueName = HangfireQueueConfiguration.NormalPriority
                });

            _recurringJobManager.AddOrUpdate(
                ConfigurationManager.AppSettings["TennisFixedScheduledJob_HistoricResultsData_JobId"],
                Job.FromExpression(() => _container.Resolve<ITennisIngestWorkerService>().IngestHistoricResults(CancellationToken.None)),
                ConfigurationManager.AppSettings["TennisFixedScheduledJob_HistoricResultsData_JobCronExpression"],
                new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.Local,
                    QueueName = HangfireQueueConfiguration.NormalPriority
                });

            _recurringJobManager.AddOrUpdate(
                ConfigurationManager.AppSettings["TennisFixedScheduledJob_HistoricRankingsData_JobId"],
                Job.FromExpression(() => _container.Resolve<ITennisIngestWorkerService>().IngestHistoricRankingsForEnabledLeagues(CancellationToken.None)),
                ConfigurationManager.AppSettings["TennisFixedScheduledJob_HistoricRankingsData_JobCronExpression"],
                new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.Local,
                    QueueName = HangfireQueueConfiguration.NormalPriority
                });

            _recurringJobManager.AddOrUpdate(
                ConfigurationManager.AppSettings["TennisFixedScheduledJob_HistoricRaceRankingsData_JobId"],
                Job.FromExpression(() => _container.Resolve<ITennisIngestWorkerService>().IngestHistoricRaceRankingsForEnabledLeagues(CancellationToken.None)),
                ConfigurationManager.AppSettings["TennisFixedScheduledJob_HistoricRaceRankingsData_JobCronExpression"],
                new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.Local,
                    QueueName = HangfireQueueConfiguration.NormalPriority
                });
        }
    }
}
