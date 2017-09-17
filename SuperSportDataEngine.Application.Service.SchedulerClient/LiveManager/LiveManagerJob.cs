using Microsoft.Practices.Unity;
using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Services;
using System;
using System.Threading.Tasks;
using System.Timers;
using System.Linq;
using System.Configuration;
using Hangfire;
using SuperSportDataEngine.Application.Service.Common.Hangfire.Configuration;
using System.Threading;

namespace SuperSportDataEngine.Application.Service.SchedulerClient.LiveManager
{
    internal class LiveManagerJob
    {
        private System.Timers.Timer _timer;
        private IRugbyService _rugbyService;
        private IRugbyIngestWorkerService _rugbyIngestService;

        public LiveManagerJob(UnityContainer container)
        {
            // This timer will run on a separate thread.
            // Every minute a method will be called.
            _timer = new System.Timers.Timer
            {
                AutoReset = false,
                Interval = TimeSpan.FromSeconds(10).TotalMilliseconds
            };

            _timer.Elapsed += new ElapsedEventHandler(UpdateManagerJobs);
            _timer.Start();

            _rugbyService = container.Resolve<IRugbyService>();
            _rugbyIngestService = container.Resolve<IRugbyIngestWorkerService>();
        }

        private async void UpdateManagerJobs(object sender, ElapsedEventArgs e)
        {
            await CreateChildJobsForLiveGamesFetchingMatchStats();

            _timer.Start();
        }

        private async Task CreateChildJobsForLiveGamesFetchingMatchStats()
        {
            var currentTournaments =
                    _rugbyService.GetCurrentTournaments();

            foreach (var tournament in currentTournaments)
            {
                var liveFixtures =
                    _rugbyService.GetLiveFixturesForCurrentTournament(tournament.Id);

                foreach (var fixture in liveFixtures)
                {
                    var jobId = ConfigurationManager.AppSettings["LiveMangerJob_LiveMatch_JobIdPrefix"] + fixture.Id;
                    var jobCronExpression = ConfigurationManager.AppSettings["LiveMangerJob_LiveMatch_JobCronExpression"];

                    RecurringJob.AddOrUpdate(
                        jobId,
                        () => DoTempLiveJobLogic(),
                        jobCronExpression,
                        TimeZoneInfo.Utc,
                        HangfireQueueConfiguration.HighPriority);

                    await _rugbyService.SetSchedulerStatusPollingForFixtureToLivePolling(fixture.Id);
                }
            }
        }

        public void DoTempLiveJobLogic()
        {

        }
    }
}
