using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.UnitOfWork;
﻿using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
namespace SuperSportDataEngine.Application.Service.SchedulerClient.ScheduledManager
{
    using Hangfire;
    using Hangfire.Common;
    using SuperSportDataEngine.Application.Service.Common.Hangfire.Configuration;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models.Enums;
    using SuperSportDataEngine.ApplicationLogic.Services;
    using SuperSportDataEngine.Common.Logging;
    using System;
    using System.Configuration;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public class LiveManagerJob
    {
        private readonly IRecurringJobManager _recurringJobManager;
        private readonly IRugbyService _rugbyService;
        private readonly IRugbyIngestWorkerService _rugbyIngestWorkerService;
        private readonly ILoggingService _logger;
        private readonly ISystemSportDataUnitOfWork _systemSportDataUnitOfWork;

        public LiveManagerJob(
            IRecurringJobManager recurringJobManager,
            IRugbyService rugbyService,
            IRugbyIngestWorkerService rugbyIngestWorkerService,
            ISystemSportDataUnitOfWork systemSportDataUnitOfWork)
        {
            _recurringJobManager = recurringJobManager;
            _rugbyService = rugbyService;
            _rugbyIngestWorkerService = rugbyIngestWorkerService;
            _systemSportDataUnitOfWork = systemSportDataUnitOfWork;
        }

        public async Task DoWorkAsync()
        {
            await CreateChildJobsForFetchingLiveMatchDataForCurrentFixtures();
            await DeleteChildJobsForFetchingMatchDataForFixturesEnded();
            await DeleteChildJobsForFetchingMatchDataForFixturesTooFarInThePast();
        }

        private async Task<int> DeleteChildJobsForFetchingMatchDataForFixturesTooFarInThePast()
        {
            var postponedFixtures =
                    (await _rugbyService.GetPostponedFixtures()).ToList();

            foreach (var fixture in postponedFixtures)
            {
                if (fixture.TeamA == null) continue;
                if (fixture.TeamB == null) continue;

                var matchName = fixture.TeamA.Name + " vs " + fixture.TeamB.Name;
                var jobId = ConfigurationManager.AppSettings["LiveManagerJob_LiveMatch_JobIdPrefix"] + matchName;

                _recurringJobManager.RemoveIfExists(jobId);

                var fixtureInDb =
                    (await _systemSportDataUnitOfWork.SchedulerTrackingRugbyFixtures.AllAsync()).FirstOrDefault(f => f.FixtureId == fixture.Id);

                if (fixtureInDb != null &&
                    fixtureInDb.SchedulerStateFixtures != SchedulerStateForRugbyFixturePolling.SchedulingNotYetStarted)
                {
                    fixtureInDb.SchedulerStateFixtures = SchedulerStateForRugbyFixturePolling.SchedulingNotYetStarted;
                    _systemSportDataUnitOfWork.SchedulerTrackingRugbyFixtures.Update(fixtureInDb);
                }
            }

            return await _systemSportDataUnitOfWork.SaveChangesAsync();
        }

        private async Task<int> DeleteChildJobsForFetchingMatchDataForFixturesEnded()
        {
            var completedFixtures =
                    await _rugbyService.GetCompletedFixtures();

            foreach (var fixture in completedFixtures)
            {
                if (fixture.TeamA == null) continue;
                if (fixture.TeamB == null) continue;

                var matchName = GetMatchName(fixture);

                var jobId = ConfigurationManager.AppSettings["LiveManagerJob_LiveMatch_JobIdPrefix"] + matchName;

                _recurringJobManager.RemoveIfExists(jobId);

                var fixtureInDb =
                    (await _systemSportDataUnitOfWork.SchedulerTrackingRugbyFixtures.AllAsync()).FirstOrDefault(f => f.FixtureId == fixture.Id);

                if (fixtureInDb != null && 
                    fixtureInDb.SchedulerStateFixtures != SchedulerStateForRugbyFixturePolling.SchedulingCompleted)
                {
                    fixtureInDb.SchedulerStateFixtures = SchedulerStateForRugbyFixturePolling.SchedulingCompleted;
                    _systemSportDataUnitOfWork.SchedulerTrackingRugbyFixtures.Update(fixtureInDb);
                }
            }

            return await _systemSportDataUnitOfWork.SaveChangesAsync();
        }

        private async Task<int> CreateChildJobsForFetchingLiveMatchDataForCurrentFixtures()
        {
            var currentTournaments =
                    (await _rugbyService.GetCurrentTournaments()).ToList();

            foreach (var tournament in currentTournaments)
            {
                var liveFixtures =
                    (await _rugbyService.GetLiveFixturesForCurrentTournament(CancellationToken.None, tournament.Id)).ToList();

                foreach (var fixture in liveFixtures)
                {
                    if (fixture.TeamA == null) continue;
                    if (fixture.TeamB == null) continue;

                    var matchName = GetMatchName(fixture);

                    var jobId = ConfigurationManager.AppSettings["LiveManagerJob_LiveMatch_JobIdPrefix"] + matchName;
                    var jobCronExpression = ConfigurationManager.AppSettings["LiveManagerJob_LiveMatch_JobCronExpression"];

                    _recurringJobManager.AddOrUpdate(
                        jobId,
                        Job.FromExpression(() => _rugbyIngestWorkerService.IngestLiveMatchData(CancellationToken.None, fixture.ProviderFixtureId)),
                        jobCronExpression,
                        new RecurringJobOptions()
                        {
                            TimeZone = TimeZoneInfo.Local,
                            QueueName = HangfireQueueConfiguration.HighPriority
                        });

                    var fixtureInDb =
                        (await _systemSportDataUnitOfWork.SchedulerTrackingRugbyFixtures.AllAsync()).FirstOrDefault(f => f.FixtureId == fixture.Id);

                    if (fixtureInDb != null && fixtureInDb.IsJobRunning != true)
                    {
                        fixtureInDb.IsJobRunning = true;
                        _systemSportDataUnitOfWork.SchedulerTrackingRugbyFixtures.Update(fixtureInDb);
                        _recurringJobManager.Trigger(jobId);
                    }
                }
            }

            return await _systemSportDataUnitOfWork.SaveChangesAsync();
        }

        private static string GetMatchName(RugbyFixture fixture)
        {
            var teamA = fixture.TeamA.Name;
            var teamB = fixture.TeamB.Name;

            return teamA + " vs " + teamB + "→" + fixture.LegacyFixtureId;
        }
    }
}
