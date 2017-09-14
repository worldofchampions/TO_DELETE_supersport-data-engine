using Hangfire;
using Microsoft.Practices.Unity;
using SuperSportDataEngine.Application.Service.Common.Hangfire.Configuration;
using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models.Enums;
using SuperSportDataEngine.ApplicationLogic.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace SuperSportDataEngine.Application.Service.SchedulerClient.ScheduledManager
{
    internal class FixturesScheduledManagerJob
    {
        private System.Timers.Timer _timer;
        private IRugbyService _rugbyService;
        private IRugbyIngestWorkerService _rugbyIngestService;

        public FixturesScheduledManagerJob(UnityContainer container)
        {
            // This timer will run on a separate thread.
            // Every minute a method will be called.
            _timer = new System.Timers.Timer
            {
                AutoReset = false,
                Interval = TimeSpan.FromMinutes(1).TotalMilliseconds
            };

            _timer.Elapsed += new ElapsedEventHandler(UpdateManagerJobs);
            _timer.Start();
            

            _rugbyService = container.Resolve<IRugbyService>();
            _rugbyIngestService = container.Resolve<IRugbyIngestWorkerService>();
        }

        private async void UpdateManagerJobs(object sender, ElapsedEventArgs e)
        {
            await CreateChildJobsForTournamentsWithSeasonsInProgress();
            await DeleteChildJobsForTournamentsWithSeasonsEnded();
            await DeleteChildJobsForInactiveTournaments();

            _timer.Start();
        }

        private async Task DeleteChildJobsForInactiveTournaments()
        {
            var inactiveTournaments =
                _rugbyService.GetInactiveTournaments();

            await DeleteJobsForTournaments(inactiveTournaments);
        }

        private async Task DeleteChildJobsForTournamentsWithSeasonsEnded()
        {
            var endedTournaments =
                _rugbyService.GetEndedTournaments();

            await DeleteJobsForTournaments(endedTournaments);
        }

        private async Task DeleteJobsForTournaments(IEnumerable<RugbyTournament> tournaments)
        {
            foreach (var tournament in tournaments)
            {
                var jobId = ConfigurationManager.AppSettings["ScheduleMangerJob_Fixtures_JobIdPrefix"] + tournament.Name;
                RecurringJob.RemoveIfExists(jobId);
                await _rugbyService.SetSchedulerStatusPollingForTournamentToNotRunning(tournament.Id);
            }
        }

        private async Task CreateChildJobsForTournamentsWithSeasonsInProgress()
        {
            var currentTournaments =
                    _rugbyService
                    .GetCurrentTournaments();

            foreach (var tournament in currentTournaments)
            {
                if (_rugbyService.GetSchedulerStateForManagerJobPolling(tournament.Id) == SchedulerStateForManagerJobPolling.NotRunning)
                {
                    var jobId = ConfigurationManager.AppSettings["ScheduleMangerJob_Fixtures_JobIdPrefix"] + tournament.Name;
                    var jobCronExpression = ConfigurationManager.AppSettings["ScheduleMangerJob_Fixtures_JobCronExpression"];
                    
                    RecurringJob.AddOrUpdate(
                        jobId,
                        () => _rugbyIngestService.IngestFixturesForTournamentSeason(CancellationToken.None, tournament.ProviderTournamentId, /* seasonId */ 2017),
                        jobCronExpression,
                        TimeZoneInfo.Utc,
                        HangfireQueueConfiguration.HighPriority);

                    await _rugbyService.SetSchedulerStatusPollingForTournamentToRunning(tournament.Id);
                }
            }
        }
    }
}