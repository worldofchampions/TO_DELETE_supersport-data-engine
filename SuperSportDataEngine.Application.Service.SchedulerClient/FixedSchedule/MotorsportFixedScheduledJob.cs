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
            UpdateRecurringJobDefinition_Seasons();
            UpdateRecurringJobDefinition_Teams();
            UpdateRecurringJobDefinition_Drivers();
            UpdateRecurringJobDefinition_Races();
            UpdateRecurringJobDefinition_RaceEvents();
            UpdateRecurringJobDefinition_RaceEventsGrids();
            UpdateRecurringJobDefinition_RaceEventsResults();
            UpdateRecurringJobDefinition_TeamStandings();
            UpdateRecurringJobDefinition_DriverStandings();

            UpdateRecurringJobDefinition_SetCurrentRaceEvents();

            CreateManualJobDefinition_HistoricRaces();
            CreateManualJobDefinition_HistoricRaceEvents();
            CreateManualJobDefinition_HistoricRaceEventsGrid();
            CreateManualJobDefinition_HistoricRaceEventsResults();
            CreateManualJobDefinition_HistoricTeamStandings();
            CreateManualJobDefinition_HistoricDriverStandings();
        }

        private void UpdateRecurringJobDefinition_SetCurrentRaceEvents()
        {
            _recurringJobManager.AddOrUpdate(
                ConfigurationManager.AppSettings["MotorsportFixedScheduleJob_SetCurrentEvents_JobId"],
                Job.FromExpression(() => _container.Resolve<IMotorsportService>().SetCurrentRaceEvents()),
                ConfigurationManager.AppSettings["MotorsportFixedScheduleJob_SetCurrentEvents_JobCronExpression"],
                new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.Local,
                    QueueName = HangfireQueueConfiguration.NormalPriority
                });
        }

        private void UpdateRecurringJobDefinition_Leagues()
        {
            _recurringJobManager.AddOrUpdate(
                ConfigurationManager.AppSettings["MotorsportFixedScheduleJob_Leagues_JobId"],
                Job.FromExpression(() => _container.Resolve<IMotorsportIngestWorkerService>().IngestLeagues(CancellationToken.None)),
                ConfigurationManager.AppSettings["MotorsportFixedScheduleJob_Leagues_JobCronExpression"],
                new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.Local,
                    QueueName = HangfireQueueConfiguration.NormalPriority
                });
        }

        private void UpdateRecurringJobDefinition_Seasons()
        {
            _recurringJobManager.AddOrUpdate(
                ConfigurationManager.AppSettings["MotorsportFixedScheduleJob_Seasons_JobId"],
                Job.FromExpression(() => _container.Resolve<IMotorsportIngestWorkerService>().IngestSeasons(CancellationToken.None)),
                ConfigurationManager.AppSettings["MotorsportFixedScheduleJob_Seasons_JobCronExpression"],
                new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.Local,
                    QueueName = HangfireQueueConfiguration.NormalPriority
                });
        }

        private void UpdateRecurringJobDefinition_Races()
        {
            _recurringJobManager.AddOrUpdate(
                ConfigurationManager.AppSettings["MotorsportFixedScheduleJob_Races_JobId"],
                Job.FromExpression(() => _container.Resolve<IMotorsportIngestWorkerService>().IngestRacesForActiveLeagues(CancellationToken.None)),
                ConfigurationManager.AppSettings["MotorsportFixedScheduleJob_Races_JobCronExpression"],
                new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.Local,
                    QueueName = HangfireQueueConfiguration.NormalPriority
                });
        }

        private void UpdateRecurringJobDefinition_RaceEvents()
        {
            _recurringJobManager.AddOrUpdate(
                ConfigurationManager.AppSettings["MotorsportFixedScheduleJob_RaceEvents_JobId"],
                Job.FromExpression(() => _container.Resolve<IMotorsportIngestWorkerService>().IngestRacesEvents(CancellationToken.None)),
                ConfigurationManager.AppSettings["MotorsportFixedScheduleJob_RaceEvents_JobCronExpression"],
                new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.Local,
                    QueueName = HangfireQueueConfiguration.NormalPriority
                });
        }

        private void UpdateRecurringJobDefinition_Drivers()
        {
            _recurringJobManager.AddOrUpdate(
                ConfigurationManager.AppSettings["MotorsportFixedScheduleJob_Drivers_JobId"],
                Job.FromExpression(() => _container.Resolve<IMotorsportIngestWorkerService>().IngestDriversForActiveLeagues(CancellationToken.None)),
                ConfigurationManager.AppSettings["MotorsportFixedScheduleJob_Drivers_JobCronExpression"],
                new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.Local,
                    QueueName = HangfireQueueConfiguration.NormalPriority
                });
        }

        private void UpdateRecurringJobDefinition_Teams()
        {
            _recurringJobManager.AddOrUpdate(
                ConfigurationManager.AppSettings["MotorsportFixedScheduleJob_Teams_JobId"],
                Job.FromExpression(() => _container.Resolve<IMotorsportIngestWorkerService>().IngestTeamsForActiveLeagues(CancellationToken.None)),
                ConfigurationManager.AppSettings["MotorsportFixedScheduleJob_Teams_JobCronExpression"],
                new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.Local,
                    QueueName = HangfireQueueConfiguration.NormalPriority
                });
        }

        private void UpdateRecurringJobDefinition_RaceEventsGrids()
        {
            _recurringJobManager.AddOrUpdate(
                ConfigurationManager.AppSettings["MotorsportFixedScheduleJob_ActiveLeaguesRaceEventsGrids_JobId"],
                Job.FromExpression(() => _container.Resolve<IMotorsportIngestWorkerService>().IngestRacesEventsGrids(CancellationToken.None)),
                ConfigurationManager.AppSettings["MotorsportFixedScheduleJob_ActiveLeaguesRaceEventsGrids_JobCronExpression"],
                new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.Local,
                    QueueName = HangfireQueueConfiguration.NormalPriority
                });
        }

        private void UpdateRecurringJobDefinition_RaceEventsResults()
        {
            _recurringJobManager.AddOrUpdate(
                ConfigurationManager.AppSettings["MotorsportFixedScheduleJob_RaceEventsResults_JobId"],
                Job.FromExpression(() => _container.Resolve<IMotorsportIngestWorkerService>().IngestResultsForActiveLeagues(CancellationToken.None)),
                ConfigurationManager.AppSettings["MotorsportFixedScheduleJob_RaceEventsResults_JobCronExpression"],
                new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.Local,
                    QueueName = HangfireQueueConfiguration.NormalPriority
                });
        }

        private void UpdateRecurringJobDefinition_DriverStandings()
        {
            _recurringJobManager.AddOrUpdate(
                ConfigurationManager.AppSettings["MotorsportFixedScheduleJob_DriverStandings_JobId"],
                Job.FromExpression(() => _container.Resolve<IMotorsportIngestWorkerService>().IngestDriverStandingsForActiveLeagues(CancellationToken.None)),
                ConfigurationManager.AppSettings["MotorsportFixedScheduleJob_DriverStandings_JobCronExpression"],
                new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.Local,
                    QueueName = HangfireQueueConfiguration.NormalPriority
                });
        }

        private void UpdateRecurringJobDefinition_TeamStandings()
        {
            _recurringJobManager.AddOrUpdate(
                ConfigurationManager.AppSettings["MotorsportFixedScheduleJob_TeamStandings_JobId"],
                Job.FromExpression(() => _container.Resolve<IMotorsportIngestWorkerService>().IngestTeamStandingsForActiveLeagues(CancellationToken.None)),
                ConfigurationManager.AppSettings["MotorsportFixedScheduleJob_TeamStandings_JobCronExpression"],
                new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.Local,
                    QueueName = HangfireQueueConfiguration.NormalPriority
                });
        }

        private void CreateManualJobDefinition_HistoricRaceEventsGrid()
        {
            _recurringJobManager.AddOrUpdate(
                ConfigurationManager.AppSettings["MotorsportFixedScheduleJob_HistoricRaceEventsGrids_JobId"],
                Job.FromExpression(() => _container.Resolve<IMotorsportIngestWorkerService>().IngestHistoricEventsGrids(CancellationToken.None)),
                ConfigurationManager.AppSettings["MotorsportFixedScheduleJob_HistoricRaceEventsGrids_JobCronExpression"],
                new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.Local,
                    QueueName = HangfireQueueConfiguration.NormalPriority
                });
        }

        private void CreateManualJobDefinition_HistoricRaceEvents()
        {
            _recurringJobManager.AddOrUpdate(
                ConfigurationManager.AppSettings["MotorsportFixedScheduleJob_HistoricRaceEvents_JobId"],
                Job.FromExpression(() => _container.Resolve<IMotorsportIngestWorkerService>().IngestHistoricRaceEvents(CancellationToken.None)),
                ConfigurationManager.AppSettings["MotorsportFixedScheduleJob_HistoricRaceEvents_JobCronExpression"],
                new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.Local,
                    QueueName = HangfireQueueConfiguration.NormalPriority
                });
        }

        private void CreateManualJobDefinition_HistoricRaces()
        {
            _recurringJobManager.AddOrUpdate(
                ConfigurationManager.AppSettings["MotorsportFixedScheduleJob_HistoricRaces_JobId"],
                Job.FromExpression(() => _container.Resolve<IMotorsportIngestWorkerService>().IngestHistoricRaces(CancellationToken.None)),
                ConfigurationManager.AppSettings["MotorsportFixedScheduleJob_HistoricRaces_JobCronExpression"],
                new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.Local,
                    QueueName = HangfireQueueConfiguration.NormalPriority
                });
        }

        private void CreateManualJobDefinition_HistoricRaceEventsResults()
        {
            _recurringJobManager.AddOrUpdate(
                ConfigurationManager.AppSettings["MotorsportFixedScheduleJob_HistoricRaceEventsResults_JobId"],
                Job.FromExpression(() => _container.Resolve<IMotorsportIngestWorkerService>().IngestHistoricEventsResults(CancellationToken.None)),
                ConfigurationManager.AppSettings["MotorsportFixedScheduleJob_HistoricRaceEventsResults_JobCronExpression"],
                new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.Local,
                    QueueName = HangfireQueueConfiguration.NormalPriority
                });
        }

        private void CreateManualJobDefinition_HistoricTeamStandings()
        {
            _recurringJobManager.AddOrUpdate(
                ConfigurationManager.AppSettings["MotorsportFixedScheduleJob_HistoricTeamStandings_JobId"],
                Job.FromExpression(() => _container.Resolve<IMotorsportIngestWorkerService>().IngestHistoricTeamStandings(CancellationToken.None)),
                ConfigurationManager.AppSettings["MotorsportFixedScheduleJob_HistoricTeamStandings_JobCronExpression"],
                new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.Local,
                    QueueName = HangfireQueueConfiguration.NormalPriority
                });
        }

        private void CreateManualJobDefinition_HistoricDriverStandings()
        {
            _recurringJobManager.AddOrUpdate(
                ConfigurationManager.AppSettings["MotorsportFixedScheduleJob_HistoricDriverStandings_JobId"],
                Job.FromExpression(() => _container.Resolve<IMotorsportIngestWorkerService>().IngestHistoricDriverStandings(CancellationToken.None)),
                ConfigurationManager.AppSettings["MotorsportFixedScheduleJob_HistoricDriverStandings_JobCronExpression"],
                new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.Local,
                    QueueName = HangfireQueueConfiguration.NormalPriority
                });
        }
    }
}