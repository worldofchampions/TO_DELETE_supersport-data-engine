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
            UpdateRecurringJobDefinition_Races();
            UpdateRecurringJobDefinition_Calendars();
            UpdateRecurringJobDefinition_Drivers();
            UpdateRecurringJobDefinition_Teams();
            UpdateRecurringJobDefinition_Grids();
            UpdateRecurringJobDefinition_Results();
            UpdateRecurringJobDefinition_TeamStandings();
            UpdateRecurringJobDefinition_DriverStandings();
            UpdateRecurringJobDefinition_Results();

            CreateManualJobDefinition_HistoricGrid();
            CreateManualJobDefinition_HistoricRaces();
            CreateManualJobDefinition_HistoricCalendars();
            CreateManualJobDefinition_HistoricResults();
            CreateManualJobDefinition_HistoricTeamStandings();
            CreateManualJobDefinition_HistoricDriverStandings();
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

        private void UpdateRecurringJobDefinition_Seasons()
        {
            _recurringJobManager.AddOrUpdate(
                ConfigurationManager.AppSettings["MotorsportFixedScheduledJob_Seasons_JobId"],
                Job.FromExpression(() => _container.Resolve<IMotorsportIngestWorkerService>().IngestSeasons(CancellationToken.None)),
                ConfigurationManager.AppSettings["MotorsportFixedScheduledJob_Seasons_JobCronExpression"],
                new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.Local,
                    QueueName = HangfireQueueConfiguration.NormalPriority
                });
        }

        private void UpdateRecurringJobDefinition_Races()
        {
            _recurringJobManager.AddOrUpdate(
                ConfigurationManager.AppSettings["MotorsportFixedScheduledJob_Races_JobId"],
                Job.FromExpression(() => _container.Resolve<IMotorsportIngestWorkerService>().IngestRacesForActiveLeagues(CancellationToken.None)),
                ConfigurationManager.AppSettings["MotorsportFixedScheduledJob_Races_JobCronExpression"],
                new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.Local,
                    QueueName = HangfireQueueConfiguration.NormalPriority
                });
        }

        private void UpdateRecurringJobDefinition_Calendars()
        {
            _recurringJobManager.AddOrUpdate(
                ConfigurationManager.AppSettings["MotorsportFixedScheduledJob_Calendar_JobId"],
                Job.FromExpression(() => _container.Resolve<IMotorsportIngestWorkerService>().IngestRacesEvents(CancellationToken.None)),
                ConfigurationManager.AppSettings["MotorsportFixedScheduledJob_Calendar_JobCronExpression"],
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

        private void UpdateRecurringJobDefinition_Teams()
        {
            _recurringJobManager.AddOrUpdate(
                ConfigurationManager.AppSettings["MotorsportFixedScheduledJob_Teams_JobId"],
                Job.FromExpression(() => _container.Resolve<IMotorsportIngestWorkerService>().IngestTeamsForActiveLeagues(CancellationToken.None)),
                ConfigurationManager.AppSettings["MotorsportFixedScheduledJob_Teams_JobCronExpression"],
                new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.Local,
                    QueueName = HangfireQueueConfiguration.NormalPriority
                });
        }

        private void UpdateRecurringJobDefinition_Grids()
        {
            _recurringJobManager.AddOrUpdate(
                ConfigurationManager.AppSettings["MotorsportFixedScheduledJob_ActiveLeaguesGrids_JobId"],
                Job.FromExpression(() => _container.Resolve<IMotorsportIngestWorkerService>().IngestRacesEventsGrids(CancellationToken.None)),
                ConfigurationManager.AppSettings["MotorsportFixedScheduledJob_ActiveLeaguesGrids_JobCronExpression"],
                new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.Local,
                    QueueName = HangfireQueueConfiguration.NormalPriority
                });
        }

        private void UpdateRecurringJobDefinition_Results()
        {
            _recurringJobManager.AddOrUpdate(
                ConfigurationManager.AppSettings["MotorsportFixedScheduledJob_Results_JobId"],
                Job.FromExpression(() => _container.Resolve<IMotorsportIngestWorkerService>().IngestResultsForActiveLeagues(CancellationToken.None)),
                ConfigurationManager.AppSettings["MotorsportFixedScheduledJob_Results_JobCronExpression"],
                new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.Local,
                    QueueName = HangfireQueueConfiguration.NormalPriority
                });
        }

        private void UpdateRecurringJobDefinition_DriverStandings()
        {
            _recurringJobManager.AddOrUpdate(
                ConfigurationManager.AppSettings["MotorsportFixedScheduledJob_DriverStandings_JobId"],
                Job.FromExpression(() => _container.Resolve<IMotorsportIngestWorkerService>().IngestDriverStandingsForActiveLeagues(CancellationToken.None)),
                ConfigurationManager.AppSettings["MotorsportFixedScheduledJob_DriverStandings_JobCronExpression"],
                new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.Local,
                    QueueName = HangfireQueueConfiguration.NormalPriority
                });
        }

        private void UpdateRecurringJobDefinition_TeamStandings()
        {
            _recurringJobManager.AddOrUpdate(
                ConfigurationManager.AppSettings["MotorsportFixedScheduledJob_TeamStandings_JobId"],
                Job.FromExpression(() => _container.Resolve<IMotorsportIngestWorkerService>().IngestTeamStandingsForActiveLeagues(CancellationToken.None)),
                ConfigurationManager.AppSettings["MotorsportFixedScheduledJob_TeamStandings_JobCronExpression"],
                new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.Local,
                    QueueName = HangfireQueueConfiguration.NormalPriority
                });
        }

        private void CreateManualJobDefinition_HistoricGrid()
        {
            _recurringJobManager.AddOrUpdate(
                ConfigurationManager.AppSettings["MotorsportFixedScheduledJob_HistoricGrids_JobId"],
                Job.FromExpression(() => _container.Resolve<IMotorsportIngestWorkerService>().IngestHistoricEventsGrids(CancellationToken.None)),
                ConfigurationManager.AppSettings["MotorsportFixedScheduledJob_HistoricGrids_JobCronExpression"],
                new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.Local,
                    QueueName = HangfireQueueConfiguration.NormalPriority
                });
        }

        private void CreateManualJobDefinition_HistoricCalendars()
        {
            _recurringJobManager.AddOrUpdate(
                ConfigurationManager.AppSettings["MotorsportFixedScheduledJob_HistoricCalendars_JobId"],
                Job.FromExpression(() => _container.Resolve<IMotorsportIngestWorkerService>().IngestHistoricRaceEvents(CancellationToken.None)),
                ConfigurationManager.AppSettings["MotorsportFixedScheduledJob_HistoricCalendars_JobCronExpression"],
                new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.Local,
                    QueueName = HangfireQueueConfiguration.NormalPriority
                });
        }

        private void CreateManualJobDefinition_HistoricRaces()
        {
            _recurringJobManager.AddOrUpdate(
                ConfigurationManager.AppSettings["MotorsportFixedScheduledJob_HistoricRaces_JobId"],
                Job.FromExpression(() => _container.Resolve<IMotorsportIngestWorkerService>().IngestHistoricRaces(CancellationToken.None)),
                ConfigurationManager.AppSettings["MotorsportFixedScheduledJob_HistoricRaces_JobCronExpression"],
                new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.Local,
                    QueueName = HangfireQueueConfiguration.NormalPriority
                });
        }

        private void CreateManualJobDefinition_HistoricResults()
        {
            _recurringJobManager.AddOrUpdate(
                ConfigurationManager.AppSettings["MotorsportFixedScheduledJob_HistoricResults_JobId"],
                Job.FromExpression(() => _container.Resolve<IMotorsportIngestWorkerService>().IngestHistoricEventsResults(CancellationToken.None)),
                ConfigurationManager.AppSettings["MotorsportFixedScheduledJob_HistoricResults_JobCronExpression"],
                new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.Local,
                    QueueName = HangfireQueueConfiguration.NormalPriority
                });
        }

        private void CreateManualJobDefinition_HistoricTeamStandings()
        {
            _recurringJobManager.AddOrUpdate(
                ConfigurationManager.AppSettings["MotorsportFixedScheduledJob_HistoricTeamStandings_JobId"],
                Job.FromExpression(() => _container.Resolve<IMotorsportIngestWorkerService>().IngestHistoricTeamStandings(CancellationToken.None)),
                ConfigurationManager.AppSettings["MotorsportFixedScheduledJob_HistoricTeamStandings_JobCronExpression"],
                new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.Local,
                    QueueName = HangfireQueueConfiguration.NormalPriority
                });
        }

        private void CreateManualJobDefinition_HistoricDriverStandings()
        {
            _recurringJobManager.AddOrUpdate(
                ConfigurationManager.AppSettings["MotorsportFixedScheduledJob_HistoricDriverStandings_JobId"],
                Job.FromExpression(() => _container.Resolve<IMotorsportIngestWorkerService>().IngestHistoricDriverStandings(CancellationToken.None)),
                ConfigurationManager.AppSettings["MotorsportFixedScheduledJob_HistoricDriverStandings_JobCronExpression"],
                new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.Local,
                    QueueName = HangfireQueueConfiguration.NormalPriority
                });
        }
    }
}