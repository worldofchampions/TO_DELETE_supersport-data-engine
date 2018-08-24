using Unity;

namespace SuperSportDataEngine.Application.Service.SchedulerClient.FixedSchedule
{
    using System;
    using System.Configuration;
    using System.Threading;
    using Hangfire;
    using Hangfire.Common;
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

            UpdateRecurringJobDefinition_CleanupSchedulerTrackingTable();
        }

        private void UpdateRecurringJobDefinition_CleanupSchedulerTrackingTable()
        {
            var eventsAgeInDays =
                int.Parse(ConfigurationManager.AppSettings["MotorsportFixedScheduleJob_Cleanup_Monthly_EventsAgeInDays"]);

            _recurringJobManager.AddOrUpdate(
                ConfigurationManager.AppSettings["MotorsportFixedScheduleJob_Cleanup_Monthly_JobId"],
                Job.FromExpression(() => _container.Resolve<IMotorsportService>().CleanupSchedulerTrackingTable(eventsAgeInDays)),
                ConfigurationManager.AppSettings["MotorsportFixedScheduleJob_Cleanup_Monthly_JobCronExpression"],
                new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.Local,
                    QueueName = HangfireQueueConfiguration.NormalPriority
                });
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
            var numberOfDaysEventsEnded =
                int.Parse(ConfigurationManager.AppSettings["MotorsportFixedScheduleJob_ActiveLeaguesRaceEventsGrids_NumberOfDaysRaceEventEnded"]);

            _recurringJobManager.AddOrUpdate(
                ConfigurationManager.AppSettings["MotorsportFixedScheduleJob_ActiveLeaguesRaceEventsGrids_JobId"],
                Job.FromExpression(() => _container.Resolve<IMotorsportIngestWorkerService>().IngestRacesEventGridForRecentlyEndedRaces(numberOfDaysEventsEnded)),
                ConfigurationManager.AppSettings["MotorsportFixedScheduleJob_ActiveLeaguesRaceEventsGrids_JobCronExpression"],
                new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.Local,
                    QueueName = HangfireQueueConfiguration.NormalPriority
                });
        }

        private void UpdateRecurringJobDefinition_RaceEventsResults()
        {
            ScheduleNightlyRecurringJobForRaceEventResults();

            ScheduleTimeConfiguredRecurringJobForRaceEventResults();
        }

        private void UpdateRecurringJobDefinition_DriverStandings()
        {
            ScheduleNightlyRecurringJobForDriverStandings();

            ScheduleTimeConfiguredRecurringJobForDriverStandings();
        }
        
        private void UpdateRecurringJobDefinition_TeamStandings()
        {
            ScheduleNightlyRecurringJobForTeamStandings();

            ScheduleTimeConfiguredRecurringJobForTeamStandings();
        }

        private void ScheduleTimeConfiguredRecurringJobForTeamStandings()
        {
            var hoursAnEventEnded =
                int.Parse(ConfigurationManager.AppSettings["MotorsportFixedScheduleJob_TeamStandings_Minutely_NumberOfHoursRaceEventEnded"]);

            var recurringJobId = ConfigurationManager.AppSettings["MotorsportFixedScheduleJob_TeamStandings_Minutely_JobId"];

            var cronExpression = ConfigurationManager.AppSettings["MotorsportFixedScheduleJob_TeamStandings_Minutely_JobCronExpression"];

            var recurringJobOptions = new RecurringJobOptions
            {
                TimeZone = TimeZoneInfo.Local,
                QueueName = HangfireQueueConfiguration.NormalPriority
            };

            _recurringJobManager.AddOrUpdate(
                recurringJobId,
                Job.FromExpression(() =>
                    _container.Resolve<IMotorsportIngestWorkerService>().IngestTeamStandingsForRecentlyEndedRaces(hoursAnEventEnded)),
                cronExpression,
                recurringJobOptions);
        }

        private void ScheduleNightlyRecurringJobForTeamStandings()
        {
            var hoursAnEventEnded =
                int.Parse(ConfigurationManager.AppSettings["MotorsportFixedScheduleJob_TeamStandings_NumberOfHoursRaceEventEnded"]);

            var recurringJobId = ConfigurationManager.AppSettings["MotorsportFixedScheduleJob_TeamStandings_JobId"];

            var cronExpression = ConfigurationManager.AppSettings["MotorsportFixedScheduleJob_TeamStandings_JobCronExpression"];

            var recurringJobOptions = new RecurringJobOptions
            {
                TimeZone = TimeZoneInfo.Local,
                QueueName = HangfireQueueConfiguration.NormalPriority
            };

            _recurringJobManager.AddOrUpdate(
                recurringJobId,
                Job.FromExpression(() =>
                _container.Resolve<IMotorsportIngestWorkerService>().IngestTeamStandingsForRecentlyEndedRaces(hoursAnEventEnded)),
                cronExpression,
                recurringJobOptions);
        }

        private void ScheduleTimeConfiguredRecurringJobForDriverStandings()
        {
            var hoursAnEventEnded =
                int.Parse(ConfigurationManager.AppSettings["MotorsportFixedScheduleJob_DriverStandings_Minutely_NumberOfHoursRaceEventEnded"]);

            var recurringJobId = ConfigurationManager.AppSettings["MotorsportFixedScheduleJob_DriverStandings_Minutely_JobId"];

            var cronExpression = ConfigurationManager.AppSettings["MotorsportFixedScheduleJob_DriverStandings_Minutely_JobCronExpression"];

            var recurringJobOptions = new RecurringJobOptions
            {
                TimeZone = TimeZoneInfo.Local,
                QueueName = HangfireQueueConfiguration.NormalPriority
            };

            _recurringJobManager.AddOrUpdate(
                recurringJobId,
                Job.FromExpression(() =>
                    _container.Resolve<IMotorsportIngestWorkerService>().IngestDriverStandingsForRecentlyEndedRaces(hoursAnEventEnded)),
                cronExpression,
                recurringJobOptions);
        }

        private void ScheduleNightlyRecurringJobForDriverStandings()
        {
            var hoursAnEventEnded =
                int.Parse(ConfigurationManager.AppSettings["MotorsportFixedScheduleJob_DriverStandings_NumberOfHoursRaceEventEnded"]);

            var recurringJobId = ConfigurationManager.AppSettings["MotorsportFixedScheduleJob_DriverStandings_JobId"];

            var cronExpression = ConfigurationManager.AppSettings["MotorsportFixedScheduleJob_DriverStandings_JobCronExpression"];

            var recurringJobOptions = new RecurringJobOptions
            {
                TimeZone = TimeZoneInfo.Local,
                QueueName = HangfireQueueConfiguration.NormalPriority
            };

            _recurringJobManager.AddOrUpdate(
                recurringJobId,
                Job.FromExpression(() =>
                _container.Resolve<IMotorsportIngestWorkerService>().IngestDriverStandingsForRecentlyEndedRaces(hoursAnEventEnded)),
                cronExpression,
                recurringJobOptions);
        }

        private void ScheduleTimeConfiguredRecurringJobForRaceEventResults()
        {
            var numberOfDaysEventsEnded =
                int.Parse(ConfigurationManager.AppSettings["MotorsportFixedScheduleJob_RaceEventsResults_Minutely_NumberOfHoursRaceEventEnded"]);

            var recurringJobId = ConfigurationManager.AppSettings["MotorsportFixedScheduleJob_RaceEventsResults_Minutely_JobId"];

            var cronExpression = ConfigurationManager.AppSettings["MotorsportFixedScheduleJob_RaceEventsResults_Minutely_JobCronExpression"];

            var recurringJobOptions = new RecurringJobOptions
            {
                TimeZone = TimeZoneInfo.Local,
                QueueName = HangfireQueueConfiguration.NormalPriority
            };

            _recurringJobManager.AddOrUpdate(
                recurringJobId,
                Job.FromExpression(() => _container.Resolve<IMotorsportIngestWorkerService>().IngestResultsForRecentlyEndendRaces(numberOfDaysEventsEnded)),
                cronExpression,
                recurringJobOptions);
        }

        private void ScheduleNightlyRecurringJobForRaceEventResults()
        {
            var numberOfDaysEventsEnded =
                int.Parse(ConfigurationManager.AppSettings["MotorsportFixedScheduleJob_RaceEventsResults_NumberOfHoursRaceEventEnded"]);

            var recurringJobId = ConfigurationManager.AppSettings["MotorsportFixedScheduleJob_RaceEventsResults_JobId"];

            var cronExpression = ConfigurationManager.AppSettings["MotorsportFixedScheduleJob_RaceEventsResults_JobCronExpression"];

            var recurringJobOptions = new RecurringJobOptions
            {
                TimeZone = TimeZoneInfo.Local,
                QueueName = HangfireQueueConfiguration.NormalPriority
            };

            _recurringJobManager.AddOrUpdate(
                recurringJobId,
                Job.FromExpression(() => _container.Resolve<IMotorsportIngestWorkerService>().IngestResultsForRecentlyEndendRaces(numberOfDaysEventsEnded)),
                cronExpression,
                recurringJobOptions);
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