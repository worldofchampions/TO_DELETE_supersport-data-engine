using Hangfire;
using Microsoft.Practices.Unity;
using SuperSportDataEngine.Application.Service.Common.Hangfire.Configuration;
using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Timers;

namespace SuperSportDataEngine.Application.Service.SchedulerClient.ScheduledManager
{
    internal class FixturesScheduledManagerJob
    {
        private Dictionary<int, string> _childJobIdsForCurrentTournaments;
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

            _childJobIdsForCurrentTournaments = new Dictionary<int, string>();

            _rugbyService = container.Resolve<IRugbyService>();
            _rugbyIngestService = container.Resolve<IRugbyIngestWorkerService>();
        }

        // This method will be  responsible for getting a list
        // of current tournaments, and spawning or deleting child jobs.
        private void UpdateManagerJobs(object sender, ElapsedEventArgs e)
        {
            var currentTournaments = 
                _rugbyService
                .GetCurrentTournaments()
                .ToList();

            // Check if there is any tournaments that need to have
            // Child jobs created.
            foreach(var tournament in currentTournaments)
            {
                if(!_childJobIdsForCurrentTournaments.ContainsKey(tournament.ProviderTournamentId))
                {
                    CreateChildJobFor(tournament.ProviderTournamentId, tournament.Name);
                    _childJobIdsForCurrentTournaments[tournament.ProviderTournamentId] = ConfigurationManager.AppSettings["ScheduleMangerJob_Fixtures_JobIdPrefix"] + tournament.Name;
                }
            }
            _timer.Start();
        }

        private void CreateChildJobFor(int tournamentId, string tournamentName)
        {
            var idPrefix = ConfigurationManager.AppSettings["ScheduleMangerJob_Fixtures_JobIdPrefix"];
            var jobCronExpression = ConfigurationManager.AppSettings["ScheduleMangerJob_Fixtures_JobCronExpression"];

            RecurringJob.AddOrUpdate(
                idPrefix + tournamentName, 
                () => _rugbyIngestService.IngestFixturesForActiveTournaments(CancellationToken.None), 
                jobCronExpression, 
                TimeZoneInfo.Utc, 
                HangfireQueueConfiguration.HighPriority);
        }
    }
}