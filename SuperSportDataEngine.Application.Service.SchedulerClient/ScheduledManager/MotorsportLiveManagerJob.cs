namespace SuperSportDataEngine.Application.Service.SchedulerClient.ScheduledManager
{
    using System;
    using System.Linq;
    using System.Configuration;
    using System.Threading;
    using System.Threading.Tasks;
    using Hangfire;
    using Hangfire.Common;
    using System.Collections.Generic;
    using SuperSportDataEngine.Application.Service.Common.Hangfire.Configuration;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.UnitOfWork;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.UnitOfWork;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models.Enums;

    public class MotorsportLiveManagerJob
    {
        private readonly IRecurringJobManager _recurringJobManager;
        private readonly IMotorsportService _motorsportService;
        private readonly IMotorsportIngestWorkerService _motorsportIngestWorkerService;
        private readonly ISystemSportDataUnitOfWork _systemSportDataUnitOfWork;
        private readonly IPublicSportDataUnitOfWork _publicSportDataUnitOfWork;

        public MotorsportLiveManagerJob(
            IRecurringJobManager recurringJobManager,
            IMotorsportService motorsportService,
            IMotorsportIngestWorkerService motorsportIngestWorkerService,
            ISystemSportDataUnitOfWork systemSportDataUnitOfWork,
            IPublicSportDataUnitOfWork publicSportDataUnitOfWork)
        {
            _recurringJobManager = recurringJobManager;
            _motorsportService = motorsportService;
            _motorsportIngestWorkerService = motorsportIngestWorkerService;
            _systemSportDataUnitOfWork = systemSportDataUnitOfWork;
            _publicSportDataUnitOfWork = publicSportDataUnitOfWork;
        }

        public async Task DoWorkAsync()
        {
            await CreateChildJobForRaceEventGridData();

            await CreateChildJobForLeagueCalendarData();

            await CreateChildJobsForLiveRaceData();

            await CreateChildJobsForRaceResultsData();

            await CreateChildJobsForStandingsData();

            await DeleteChildJobsForRaceEventGridData();

            await DeleteChildJobsForLegueCalendarData();

            await DeleteChildJobsForLiveData();

            await DeleteChildJobsForRaceResultsData();

            await DeleteChildJobsForStandingsData();

        }

        private async Task CreateChildJobsForStandingsData()
        {
            var currentLeagues = await _motorsportService.GetActiveLeagues();

            foreach (var league in currentLeagues)
            {
                var raceEvent = (await _motorsportService.GetEndedRaceEventsForLeague(league.Id)).FirstOrDefault();

                UpdateChildJobDefinitionForStandingsData(raceEvent);

                await _systemSportDataUnitOfWork.SaveChangesAsync();
            }
        }

        private async Task CreateChildJobForLeagueCalendarData()
        {
            var numberOfHoursBeforeEventStarts =
                int.Parse(ConfigurationManager.AppSettings["MotorsportChildJob_LeagueCalendar_TimeToStartJobBeforeEventStartsInHours"]);

            var raceEvent = (await _motorsportService.GetPreLiveEventsForActiveLeagues(numberOfHoursBeforeEventStarts)).FirstOrDefault();

            UpdateChildJobDefinitionForLeagueCalendar(raceEvent);

            await _systemSportDataUnitOfWork.SaveChangesAsync();
        }

        private async Task CreateChildJobForRaceEventGridData()
        {
            var numberOfHoursBeforeEventStarts =
               int.Parse(ConfigurationManager.AppSettings["MotorsportChildJob_RaceEventGrid_TimeToStartJobBeforeEventStartsInHours"]);

            var raceEvents = (await _motorsportService.GetPreLiveEventsForActiveLeagues(numberOfHoursBeforeEventStarts)).ToList();

            foreach (var raceEvent in raceEvents)
            {
                if (raceEvent == null) continue;

                UpdateChildJobDefinitionForRaceGrid(raceEvent);
            }

            await _systemSportDataUnitOfWork.SaveChangesAsync();
        }

        private async Task CreateChildJobsForLiveRaceData()
        {
            var currentLeagues = await _motorsportService.GetActiveLeagues();

            foreach (var league in currentLeagues)
            {
                var raceEvent = await _motorsportService.GetLiveEventForLeague(league.Id);

                if (raceEvent == null) continue;

                UpdateChildJobDefinitionForLiveRaceEvent(raceEvent);
            }

            await _systemSportDataUnitOfWork.SaveChangesAsync();
        }

        private async Task CreateChildJobsForRaceResultsData()
        {
            var currentLeagues = await _motorsportService.GetActiveLeagues();

            foreach (var league in currentLeagues)
            {
                var raceEvents = await _motorsportService.GetEndedRaceEventsForLeague(league.Id);

                foreach (var raceEvent in raceEvents)
                {
                    if (raceEvent == null) continue;

                    UpdateChildJobDefinitionForResultsData(raceEvent);
                }

                await _systemSportDataUnitOfWork.SaveChangesAsync();
            }
        }

        private async Task DeleteChildJobsForLiveData()
        {
            var currentLeagues = await _motorsportService.GetActiveLeagues();

            foreach (var league in currentLeagues)
            {
                var raceEvents = await _motorsportService.GetEndedRaceEventsForLeague(league.Id);

                foreach (var raceEvent in raceEvents)
                {
                    if (raceEvent == null) continue;

                    var schedulerEvent =
                        _systemSportDataUnitOfWork.SchedulerTrackingMotorsportRaceEvents.FirstOrDefault(e =>
                            e.MotorsportRaceEventId == raceEvent.Id);

                    if (schedulerEvent == null) continue;

                    schedulerEvent.SchedulerStateForMotorsportRaceEventLivePolling =
                        SchedulerStateForMotorsportRaceEventLivePolling.Completed;

                    var jobId = ConfigurationManager.AppSettings["Motorsport_LiveRaceJob_JobIdPrefix"] +
                                raceEvent.MotorsportRace.RaceName + "→" + raceEvent.LegacyRaceEventId;

                    _recurringJobManager.RemoveIfExists(jobId);

                }

                await _systemSportDataUnitOfWork.SaveChangesAsync();
            }
        }

        private async Task DeleteChildJobsForRaceResultsData()
        {
            var currentLeagues = await _motorsportService.GetActiveLeagues();

            foreach (var league in currentLeagues)
            {
                var schedulerTrackingEvents = await GetSchedulerTrackingEventsToDeleteResultsChildJobsFor(league);

                foreach (var schedulerTrackingEvent in schedulerTrackingEvents)
                {
                    if (schedulerTrackingEvent == null) continue;

                    var raceEvent =
                        _publicSportDataUnitOfWork.MotorsportRaceEvents.FirstOrDefault(e => e.Id == schedulerTrackingEvent.MotorsportRaceEventId);

                    if (raceEvent == null) continue;

                    var jobId =
                        ConfigurationManager.AppSettings["MotorsportChildJob_RaceEventsResults_JobIdPrefix"] +
                        raceEvent.MotorsportRace.RaceName + "→" + raceEvent.LegacyRaceEventId;

                    _recurringJobManager.RemoveIfExists(jobId);

                    schedulerTrackingEvent.SchedulerStateForMotorsportRaceEventResultsPolling =
                        SchedulerStateForMotorsportRaceEventResultsPolling.Completed;

                    _systemSportDataUnitOfWork.SchedulerTrackingMotorsportRaceEvents.Update(schedulerTrackingEvent);
                }
            }

            await _systemSportDataUnitOfWork.SaveChangesAsync();
        }

        private async Task DeleteChildJobsForStandingsData()
        {
            var currentLeagues = await _motorsportService.GetActiveLeagues();

            foreach (var league in currentLeagues)
            {
                var trackingMotorsportRaceEvents = await GetSchedulerEventsToDeleteStandingsChildJobsFor(league);

                foreach (var trackingEvent in trackingMotorsportRaceEvents)
                {
                    if (trackingEvent == null) continue;

                    var raceEvent =
                        _publicSportDataUnitOfWork.MotorsportRaceEvents.FirstOrDefault(e => e.Id == trackingEvent.MotorsportRaceEventId);

                    if (raceEvent == null) continue;

                    var jobId =
                        ConfigurationManager.AppSettings["MotorsportChildJob_DriverStandings_JobIdPrefix"] + raceEvent.MotorsportRace.MotorsportLeague.Name;

                    _recurringJobManager.RemoveIfExists(jobId);

                    jobId =
                        ConfigurationManager.AppSettings["MotorsportChildJob_TeamStandings_JobIdPrefix"] + raceEvent.MotorsportRace.MotorsportLeague.Name;

                    _recurringJobManager.RemoveIfExists(jobId);

                    trackingEvent.SchedulerStateForMotorsportRaceEventStandingsPolling =
                        SchedulerStateForMotorsportRaceEventStandingsPolling.Completed;

                    _systemSportDataUnitOfWork.SchedulerTrackingMotorsportRaceEvents.Update(trackingEvent);

                }

                await _systemSportDataUnitOfWork.SaveChangesAsync();
            }
        }

        private async Task DeleteChildJobsForRaceEventGridData()
        {
            var currentLeagues = await _motorsportService.GetActiveLeagues();

            foreach (var league in currentLeagues)
            {
                var schedulerTrackingEvents = (await GetSchedulerTrackingEventsToDeleteGridChildJobsFor(league)).ToList();

                foreach (var schedulerTrackingEvent in schedulerTrackingEvents)
                {
                    if (schedulerTrackingEvent == null) continue;

                    var raceEvent =
                        _publicSportDataUnitOfWork.MotorsportRaceEvents.FirstOrDefault(e => e.Id == schedulerTrackingEvent.MotorsportRaceEventId);

                    if (raceEvent == null) continue;

                    var jobId =
                        ConfigurationManager.AppSettings["MotorsportChildJob_RaceEventGrid_JobIdPrefix"] +
                        raceEvent.MotorsportRace.MotorsportLeague.Name + "→" + raceEvent.MotorsportRace.RaceName
                        + "→" + raceEvent.LegacyRaceEventId;

                    _recurringJobManager.RemoveIfExists(jobId);

                    schedulerTrackingEvent.SchedulerStateForMotorsportRaceEventGridPolling =
                        SchedulerStateForMotorsportRaceEventGridPolling.Completed;

                    _systemSportDataUnitOfWork.SchedulerTrackingMotorsportRaceEvents.Update(schedulerTrackingEvent);
                }
            }

            await _systemSportDataUnitOfWork.SaveChangesAsync();
        }

        private async Task DeleteChildJobsForLegueCalendarData()
        {
            var currentLeagues = await _motorsportService.GetActiveLeagues();

            foreach (var league in currentLeagues)
            {
                var schedulerTrackingEvents = await GetSchedulerTrackingEventsToDeleteCalendarChildJobsFor(league);

                foreach (var schedulerTrackingEvent in schedulerTrackingEvents)
                {
                    if (schedulerTrackingEvent == null) continue;

                    var raceEvent =
                        _publicSportDataUnitOfWork.MotorsportRaceEvents.FirstOrDefault(e =>
                            e.Id == schedulerTrackingEvent.MotorsportRaceEventId);

                    if (raceEvent == null) continue;

                    var jobId =
                        ConfigurationManager.AppSettings["MotorsportChildJob_LeagueCalendar_JobIdPrefix"] +
                        raceEvent.MotorsportRace.MotorsportLeague.Name;

                    _recurringJobManager.RemoveIfExists(jobId);

                    schedulerTrackingEvent.SchedulerStateForMotorsportRaceEventCalendarPolling =
                        SchedulerStateForMotorsportRaceEventCalendarPolling.Completed;

                    _systemSportDataUnitOfWork.SchedulerTrackingMotorsportRaceEvents.Update(schedulerTrackingEvent);
                }
            }

            await _systemSportDataUnitOfWork.SaveChangesAsync();
        }

        private void UpdateChildJobDefinitionForRaceGrid(MotorsportRaceEvent raceEvent)
        {
            var schedulerEvent =
                _systemSportDataUnitOfWork.SchedulerTrackingMotorsportRaceEvents.FirstOrDefault(e =>
                    e.MotorsportRaceEventId == raceEvent.Id);

            if (!ShouldUpdateChildJobForGridPolling(schedulerEvent)) return;

            var jobId =
                ConfigurationManager.AppSettings["MotorsportChildJob_RaceEventGrid_JobIdPrefix"] +
                raceEvent.MotorsportRace.MotorsportLeague.Name + "→" + raceEvent.MotorsportRace.RaceName
                + "→" + raceEvent.LegacyRaceEventId;

            var jobCronExpression = ConfigurationManager.AppSettings["MotorsportChildJob_RaceEventGrid_JobCronExpression"];

            var threadSleepPollingInSeconds =
                int.Parse(ConfigurationManager.AppSettings["MotorsportChildJob_RaceEventGrid_ThreadSleepInSeconds"]);

            var pollingDurationInMinutes =
                int.Parse(ConfigurationManager.AppSettings["MotorsportChildJob_RaceEventGrid_PollingDurationInMinutes"]);

            var jobOptions = new RecurringJobOptions { TimeZone = TimeZoneInfo.Local, QueueName = HangfireQueueConfiguration.HighPriority };

            _recurringJobManager.AddOrUpdate(
                jobId,
                Job.FromExpression(() =>
                    _motorsportIngestWorkerService.IngestRaceEventGrids(raceEvent, threadSleepPollingInSeconds, pollingDurationInMinutes)),
                jobCronExpression,
                jobOptions);

            schedulerEvent.SchedulerStateForMotorsportRaceEventGridPolling =
                SchedulerStateForMotorsportRaceEventGridPolling.InProgress;

            _systemSportDataUnitOfWork.SchedulerTrackingMotorsportRaceEvents.Update(schedulerEvent);

            _recurringJobManager.Trigger(jobId);

        }

        private void UpdateChildJobDefinitionForLeagueCalendar(MotorsportRaceEvent raceEvent)
        {
            if(raceEvent == null) return;

            var schedulerEvent =
                _systemSportDataUnitOfWork.SchedulerTrackingMotorsportRaceEvents.FirstOrDefault(e =>
                    e.MotorsportRaceEventId == raceEvent.Id);

            if (!ShouldUpdateChildJobForCalendarPolling(schedulerEvent)) return;

            var jobId =
                ConfigurationManager.AppSettings["MotorsportChildJob_LeagueCalendar_JobIdPrefix"] + raceEvent.MotorsportRace.MotorsportLeague.Name;

            var jobCronExpression = ConfigurationManager.AppSettings["MotorsportChildJob_LeagueCalendar_JobCronExpression"];

            var threadSleepPollingInSeconds =
                int.Parse(ConfigurationManager.AppSettings["MotorsportChildJob_LeagueCalendar_ThreadSleepInSeconds"]);

            var pollingDurationInMinutes =
                int.Parse(ConfigurationManager.AppSettings["MotorsportChildJob_LeagueCalendar_PollingDurationInMinutes"]);

            var jobOptions = new RecurringJobOptions { TimeZone = TimeZoneInfo.Local, QueueName = HangfireQueueConfiguration.HighPriority };

            _recurringJobManager.AddOrUpdate(
                jobId,
                Job.FromExpression(() =>
                    _motorsportIngestWorkerService.IngestRaceEventsForLeague(raceEvent, threadSleepPollingInSeconds, pollingDurationInMinutes)),
                jobCronExpression,
                jobOptions);

            schedulerEvent.SchedulerStateForMotorsportRaceEventCalendarPolling =
                SchedulerStateForMotorsportRaceEventCalendarPolling.InProgress;

            _systemSportDataUnitOfWork.SchedulerTrackingMotorsportRaceEvents.Update(schedulerEvent);

            _recurringJobManager.Trigger(jobId);
        }

        private void UpdateChildJobDefinitionForLiveRaceEvent(MotorsportRaceEvent raceEvent)
        {
            var schedulerEvent =
                 _systemSportDataUnitOfWork.SchedulerTrackingMotorsportRaceEvents.FirstOrDefault(e =>
                 e.MotorsportRaceEventId == raceEvent.Id);

            if (ShouldUpdateJobForLivePolling(schedulerEvent))
            {
                var jobId =
                    ConfigurationManager.AppSettings["Motorsport_LiveRaceJob_JobIdPrefix"] +
                    raceEvent.MotorsportRace.RaceName + "→" + raceEvent.LegacyRaceEventId;

                var jobCronExpression = ConfigurationManager.AppSettings["Motorsport_LiveRaceJob_JobCronExpression"];

                var threadSleepPollingInSeconds =
                    int.Parse(ConfigurationManager.AppSettings["Motorsport_LiveRaceJob_ThreadSleepPollingInSeconds"]);

                var jobOptions = new RecurringJobOptions { TimeZone = TimeZoneInfo.Local, QueueName = HangfireQueueConfiguration.HighPriority };

                _recurringJobManager.AddOrUpdate(
                    jobId,
                    Job.FromExpression(() =>
                    _motorsportIngestWorkerService.IngestLiveRaceEventData(raceEvent, threadSleepPollingInSeconds, CancellationToken.None)),
                    jobCronExpression,
                    jobOptions);

                schedulerEvent.SchedulerStateForMotorsportRaceEventLivePolling =
                    SchedulerStateForMotorsportRaceEventLivePolling.InProgress;

                _systemSportDataUnitOfWork.SchedulerTrackingMotorsportRaceEvents.Update(schedulerEvent);

                _recurringJobManager.Trigger(jobId);
            }
        }

        private void UpdateChildJobDefinitionForResultsData(MotorsportRaceEvent raceEvent)
        {
            var schedulerEvent =
                _systemSportDataUnitOfWork.SchedulerTrackingMotorsportRaceEvents.FirstOrDefault(e =>
                e.MotorsportRaceEventId == raceEvent.Id &&
                e.MotorsportRaceEventStatus == MotorsportRaceEventStatus.Result);

            if (ShouldUpdateChildJobForResultsPolling(schedulerEvent))
            {
                var jobId =
                    ConfigurationManager.AppSettings["MotorsportChildJob_RaceEventsResults_JobIdPrefix"] +
                    raceEvent.MotorsportRace.RaceName + "→" + raceEvent.LegacyRaceEventId;

                var jobCronExpression =
                    ConfigurationManager.AppSettings["MotorsportChildJob_RaceEventsResults_JobCronExpression"];

                var threadSleepInSeconds =
                    int.Parse(ConfigurationManager.AppSettings["MotorsportChildJob_RaceEventsResults_ThreadSleepInSeconds"]);

                var pollingDurationInMinutes =
                    int.Parse(ConfigurationManager.AppSettings["MotorsportChildJob_RaceEventsResults_PollingDurationInMinutes"]);

                var jobOptions = new RecurringJobOptions { TimeZone = TimeZoneInfo.Local, QueueName = HangfireQueueConfiguration.HighPriority };

                _recurringJobManager.AddOrUpdate(
                    jobId,
                    Job.FromExpression(() =>
                        _motorsportIngestWorkerService.IngestResultsForRaceEvent(raceEvent, threadSleepInSeconds, pollingDurationInMinutes)),
                    jobCronExpression,
                    jobOptions);

                _recurringJobManager.Trigger(jobId);

                schedulerEvent.SchedulerStateForMotorsportRaceEventResultsPolling =
                    SchedulerStateForMotorsportRaceEventResultsPolling.InProgress;
            }

            _systemSportDataUnitOfWork.SchedulerTrackingMotorsportRaceEvents.Update(schedulerEvent);

        }

        private void UpdateChildJobDefinitionForStandingsData(MotorsportRaceEvent raceEvent)
        {
            if (raceEvent == null) return;

            var schedulerEvent =
                _systemSportDataUnitOfWork.SchedulerTrackingMotorsportRaceEvents.FirstOrDefault(e =>
                    e.MotorsportRaceEventId == raceEvent.Id &&
                    e.MotorsportRaceEventStatus == MotorsportRaceEventStatus.Result);

            if (!ShouldUpdateChildJobForStandingsPolling(schedulerEvent)) return;

            UpdateChildJobDefinitionForDriverStandings(raceEvent);

            UpdateChildJobDefinitionForTeamStandings(raceEvent);

            schedulerEvent.SchedulerStateForMotorsportRaceEventStandingsPolling =
                SchedulerStateForMotorsportRaceEventStandingsPolling.InProgress;

            _systemSportDataUnitOfWork.SchedulerTrackingMotorsportRaceEvents.Update(schedulerEvent);

        }

        private void UpdateChildJobDefinitionForTeamStandings(MotorsportRaceEvent raceEvent)
        {
            var jobId =
                ConfigurationManager.AppSettings["MotorsportChildJob_TeamStandings_JobIdPrefix"] + raceEvent.MotorsportRace.MotorsportLeague.Name;

            var jobCronExpression = ConfigurationManager.AppSettings["MotorsportChildJob_TeamStandings_JobCronExpression"];

            var jobOptions = new RecurringJobOptions { TimeZone = TimeZoneInfo.Local, QueueName = HangfireQueueConfiguration.HighPriority };

            var threadSleepPollingInSeconds =
                int.Parse(ConfigurationManager.AppSettings["MotorsportChildJob_TeamStandings_ThreadSleepInSeconds"]);

            var pollingDurationInMinutes =
                int.Parse(ConfigurationManager.AppSettings["MotorsportChildJob_TeamStandings_PollingDurationInMinutes"]);

            _recurringJobManager.AddOrUpdate(
                jobId,
                Job.FromExpression(() =>
                _motorsportIngestWorkerService.IngestTeamStandingsForLeague(raceEvent, threadSleepPollingInSeconds, pollingDurationInMinutes)),
                jobCronExpression, jobOptions);

            _recurringJobManager.Trigger(jobId);
        }

        private void UpdateChildJobDefinitionForDriverStandings(MotorsportRaceEvent raceEvent)
        {
            var jobId =
                ConfigurationManager.AppSettings["MotorsportChildJob_DriverStandings_JobIdPrefix"] + raceEvent.MotorsportRace.MotorsportLeague.Name;

            var jobCronExpression = ConfigurationManager.AppSettings["MotorsportChildJob_DriverStandings_JobCronExpression"];

            var threadSleepPollingInSeconds =
                int.Parse(ConfigurationManager.AppSettings["MotorsportChildJob_DriverStandings_ThreadSleepInSeconds"]);

            var pollingDurationInMinutes =
                int.Parse(ConfigurationManager.AppSettings["MotorsportChildJob_DriverStandings_PollingDurationInMinutes"]);

            var jobOptions = new RecurringJobOptions { TimeZone = TimeZoneInfo.Local, QueueName = HangfireQueueConfiguration.HighPriority };

            _recurringJobManager.AddOrUpdate(
                jobId,
                Job.FromExpression(() =>
                _motorsportIngestWorkerService.IngestDriverStandingsForLeague(raceEvent, threadSleepPollingInSeconds, pollingDurationInMinutes)),
                jobCronExpression,
                jobOptions);

            _recurringJobManager.Trigger(jobId);
        }

        private static bool ShouldUpdateChildJobForCalendarPolling(SchedulerTrackingMotorsportRaceEvent schedulerEvent)
        {
            return schedulerEvent != null
                   && schedulerEvent.SchedulerStateForMotorsportRaceEventCalendarPolling ==
                   SchedulerStateForMotorsportRaceEventCalendarPolling.PollingNotStarted;
        }

        private static bool ShouldUpdateChildJobForGridPolling(SchedulerTrackingMotorsportRaceEvent schedulerEvent)
        {
            return schedulerEvent != null
                   && schedulerEvent.SchedulerStateForMotorsportRaceEventGridPolling ==
                   SchedulerStateForMotorsportRaceEventGridPolling.PollingNotStarted;
        }

        private static bool ShouldUpdateJobForLivePolling(SchedulerTrackingMotorsportRaceEvent schedulerEvent)
        {
            return schedulerEvent != null &&
                   schedulerEvent.SchedulerStateForMotorsportRaceEventLivePolling ==
                   SchedulerStateForMotorsportRaceEventLivePolling.PollingNotStarted;
        }

        private static bool ShouldUpdateChildJobForResultsPolling(SchedulerTrackingMotorsportRaceEvent schedulerEvent)
        {
            return schedulerEvent != null &&
                   schedulerEvent.SchedulerStateForMotorsportRaceEventResultsPolling ==
                   SchedulerStateForMotorsportRaceEventResultsPolling.PollingNotStarted;
        }

        private static bool ShouldUpdateChildJobForStandingsPolling(SchedulerTrackingMotorsportRaceEvent schedulerEvent)
        {
            return schedulerEvent != null &&
                   schedulerEvent.SchedulerStateForMotorsportRaceEventStandingsPolling ==
                   SchedulerStateForMotorsportRaceEventStandingsPolling.PollingNotStarted;
        }

        private async Task<IEnumerable<SchedulerTrackingMotorsportRaceEvent>> GetSchedulerTrackingEventsToDeleteGridChildJobsFor(MotorsportLeague league)
        {
            var results = (await _systemSportDataUnitOfWork.SchedulerTrackingMotorsportRaceEvents.WhereAsync(e =>
                e.MotorsportLeagueId == league.Id)).Where(IsGridPollingDurationOver);

            return results;
        }

        private async Task<IEnumerable<SchedulerTrackingMotorsportRaceEvent>> GetSchedulerTrackingEventsToDeleteResultsChildJobsFor(MotorsportLeague league)
        {
            var results = (await _systemSportDataUnitOfWork.SchedulerTrackingMotorsportRaceEvents.WhereAsync(e =>
                e.MotorsportLeagueId == league.Id)).Where(IsResultsPollingDurationComplete);

            return results;

        }

        private async Task<IEnumerable<SchedulerTrackingMotorsportRaceEvent>> GetSchedulerEventsToDeleteStandingsChildJobsFor(MotorsportLeague league)
        {
            var eventsToDeleteStandingsChildJobsFor =
                (await _systemSportDataUnitOfWork.SchedulerTrackingMotorsportRaceEvents.WhereAsync(e =>
                e.MotorsportLeagueId == league.Id)).Where(IsStandingsPollingDurationComplete);

            return eventsToDeleteStandingsChildJobsFor;
        }

        private async Task<IEnumerable<SchedulerTrackingMotorsportRaceEvent>> GetSchedulerTrackingEventsToDeleteCalendarChildJobsFor(MotorsportLeague league)
        {
            var eventsToDeleteStandingsChildJobsFor =
            (await _systemSportDataUnitOfWork.SchedulerTrackingMotorsportRaceEvents.WhereAsync(e =>
                e.MotorsportLeagueId == league.Id)).Where(IsCalendarPollingDurationComplete);

            return eventsToDeleteStandingsChildJobsFor;
        }

        private static bool IsCalendarPollingDurationComplete(SchedulerTrackingMotorsportRaceEvent schedulerEvent)
        {
            var pollingDurationInMinutes =
                int.Parse(ConfigurationManager.AppSettings["MotorsportChildJob_LeagueCalendar_PollingDurationInMinutes"]);

            var durationComplete = schedulerEvent.EndedDateTimeUtc != null &&
                                   DateTimeOffset.UtcNow.Subtract(schedulerEvent.EndedDateTimeUtc.Value).TotalMinutes >= pollingDurationInMinutes;

            return durationComplete;
        }

        private static bool IsGridPollingDurationOver(SchedulerTrackingMotorsportRaceEvent schedulerEvent)
        {
            var pollingDurationInMinutes =
                int.Parse(ConfigurationManager.AppSettings["MotorsportChildJob_RaceEventGrid_PollingDurationInMinutes"]);

            var durationComplete = schedulerEvent.EndedDateTimeUtc != null &&
                DateTimeOffset.UtcNow.Subtract(schedulerEvent.EndedDateTimeUtc.Value).TotalMinutes >= pollingDurationInMinutes;

            return durationComplete;
        }

        private static bool IsStandingsPollingDurationComplete(SchedulerTrackingMotorsportRaceEvent schedulerEvent)
        {
            var pollingDurationInMinutes =
                int.Parse(ConfigurationManager.AppSettings["MotorsportChildJob_DriverStandings_PollingDurationInMinutes"]);

            var durationComplete = schedulerEvent.EndedDateTimeUtc != null &&
                DateTimeOffset.UtcNow.Subtract(schedulerEvent.EndedDateTimeUtc.Value).TotalMinutes >= pollingDurationInMinutes;

            return durationComplete;
        }

        private static bool IsResultsPollingDurationComplete(SchedulerTrackingMotorsportRaceEvent raceEvent)
        {
            var pollingDurationInMinutes =
                int.Parse(ConfigurationManager.AppSettings["MotorsportChildJob_RaceEventsResults_PollingDurationInMinutes"]);

            var durationOver = raceEvent.EndedDateTimeUtc != null &&
                DateTimeOffset.UtcNow.Subtract(raceEvent.EndedDateTimeUtc.Value).TotalMinutes >= pollingDurationInMinutes;

            return durationOver;
        }
    }
}