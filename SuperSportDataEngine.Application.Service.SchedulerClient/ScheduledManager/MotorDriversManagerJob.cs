using System;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.Common;
using Microsoft.Practices.Unity;
using SuperSportDataEngine.Application.Container;
using SuperSportDataEngine.Application.Service.Common.Hangfire.Configuration;
using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RequestModels;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models.Enums;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.UnitOfWork;

namespace SuperSportDataEngine.Application.Service.SchedulerClient.ScheduledManager
{
    public class MotorDriversManagerJob
    {
        private IRecurringJobManager _recurringJobManager;
        private IUnityContainer _childContainer;


        public MotorDriversManagerJob(IRecurringJobManager recurringJobManager, IUnityContainer childContainer)
        {
            _childContainer = childContainer;
            _recurringJobManager = recurringJobManager;
        }

        public async Task DoWorkAsync()
        {
            CreateContainer();
            ConfigureDependencies();
            await CreateChildJobsForFetchingRaceDrivers();
        }

        private async Task<int> CreateChildJobsForFetchingRaceDrivers()
        {
            using (var unitOfWork = _childContainer.Resolve<ISystemSportDataUnitOfWork>())
            {
                var leagues = await _childContainer.Resolve<IMotorService>().GetActiveLeagues();

                foreach (var league in leagues)
                {
                    var seasonId = await _childContainer.Resolve<IMotorService>().GetProviderSeasonIdForLeague(league.Id, CancellationToken.None);

                    if (await _childContainer.Resolve<IMotorService>().GetSchedulerStateForManagerJobPolling(league.Id) == SchedulerStateForManagerJobPolling.NotRunning)
                    {
                        var jobId = ConfigurationManager.AppSettings["ScheduleManagerJob_Drivers_CurrentLeagues_JobIdPrefix"] + league.Name;
                        var jobCronExpression = ConfigurationManager.AppSettings["ScheduleManagerJob_Drivers_CurrentLeagues_JobCronExpression_OneMinute"];

                        AddOrUpdateHangfireJob(league.ProviderSlug, seasonId, jobId, jobCronExpression);

                        QueueJobForLowFrequencyPolling(league.Id, league.ProviderLeagueId, seasonId, jobId);

                        var season =
                            (await unitOfWork.SchedulerTrackingMotorSeasons.AllAsync())
                            .FirstOrDefault(s =>
                                s.MotorSeasonStatus == MotorSeasonStatus.InProgress &&
                                s.LeagueId == league.Id &&
                                s.SchedulerStateForManagerJobPolling == SchedulerStateForManagerJobPolling.NotRunning);

                        if (season == null) continue;

                        season.SchedulerStateForManagerJobPolling = SchedulerStateForManagerJobPolling.Running;
                        unitOfWork.SchedulerTrackingMotorSeasons.Update(season);
                    }
                }

                return await unitOfWork.SaveChangesAsync();
            }
        }

        private void QueueJobForLowFrequencyPolling(Guid leagueId, int providerLeagueId, int seasonId, string jobId)
        {
            var highFreqExpiryFromConfig = ConfigurationManager.AppSettings["ScheduleManagerJob_Logs_CurrentTournaments_HighFrequencyPolling_ExpiryInMinutes"];

            var udpateJobFrequencyOnThisMinute = int.Parse(highFreqExpiryFromConfig);

            var timer = new System.Timers.Timer
            {
                AutoReset = false,
                Interval = TimeSpan.FromMinutes(udpateJobFrequencyOnThisMinute).TotalMilliseconds
            };

            timer.Elapsed += delegate
            {
                var jobExpiryFromConfig = ConfigurationManager.AppSettings["ScheduleManagerJob_Logs_CurrentTournaments_LowFrequencyPolling_ExpiryInMinutes"];
                var jobCronExpression = ConfigurationManager.AppSettings["ScheduleManagerJob_Logs_CurrentTournaments_LowFrequencyPolling_CronExpression"];

                var deleteJobOnThisMinute = int.Parse(jobExpiryFromConfig);

                AddOrUpdateHangfireJob(providerLeagueId.ToString(), seasonId, jobId, jobCronExpression);

                //TODO: Queue Job for ceanup job

                timer.Stop();
            };

            timer.Start();
        }

        private void ConfigureDependencies()
        {
            _recurringJobManager = _childContainer.Resolve<IRecurringJobManager>();
        }

        private void CreateContainer()
        {
            _childContainer?.Dispose();
            _childContainer = new UnityContainer();
            UnityConfigurationManager.RegisterTypes(_childContainer, Container.Enums.ApplicationScope.ServiceSchedulerClient);
        }


        private void AddOrUpdateHangfireJob(string providerSlug, int providerSeasonId, string jobId, string jobCronExpression)
        {
            _recurringJobManager.AddOrUpdate(
                jobId,
                Job.FromExpression(() => _childContainer.Resolve<IMotorIngestWorkerService>()
                    .IngestDriversForActiveTournaments(new MotorDriverRequestEntity(providerSlug, providerSeasonId), CancellationToken.None)),
                jobCronExpression,
                new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.Local,
                    QueueName = HangfireQueueConfiguration.HighPriority
                });
        }
    }
}