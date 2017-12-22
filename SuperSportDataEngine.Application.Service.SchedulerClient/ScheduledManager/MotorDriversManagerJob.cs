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
                    var seasonId = await _childContainer.Resolve<IMotorService>().GetCurrentProviderSeasonIdForLeague(league.Id, CancellationToken.None);

                    if (await _childContainer.Resolve<IMotorService>().GetSchedulerStateForManagerJobPolling(league.Id) == SchedulerStateForManagerJobPolling.NotRunning)
                    {
                        var jobId = ConfigurationManager.AppSettings["ScheduleManagerJob_Logs_CurrentTournaments_JobIdPrefix"] + league.Name;
                        var jobCronExpression = ConfigurationManager.AppSettings["ScheduleManagerJob_Logs_CurrentTournaments_JobCronExpression_OneMinute"];

                        AddOrUpdateHangfireJob(league.ProviderSlug, seasonId, jobId, jobCronExpression);

                        //QueueJobForLowFrequencyPolling(tournament.Id, tournament.ProviderTournamentId, seasonId, jobId);

                        var season =
                            (await unitOfWork.SchedulerTrackingRugbySeasons.AllAsync())
                            .FirstOrDefault(s =>
                                s.RugbySeasonStatus == RugbySeasonStatus.InProgress &&
                                s.TournamentId == league.Id &&
                                s.SchedulerStateForManagerJobPolling == SchedulerStateForManagerJobPolling.NotRunning);

                        if (season != null)
                        {
                            season.SchedulerStateForManagerJobPolling = SchedulerStateForManagerJobPolling.Running;
                            unitOfWork.SchedulerTrackingRugbySeasons.Update(season);
                        }
                    }
                }

                return await unitOfWork.SaveChangesAsync();
            }
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
            var jobInvokedMethod = _childContainer.Resolve<IMotorIngestWorkerService>()
                .IngestDriversForActiveTournaments(new MotorDriverRequestEntity(providerSlug, providerSeasonId), CancellationToken.None);

            _recurringJobManager.AddOrUpdate(
                jobId,
                Job.FromExpression(() => jobInvokedMethod),
                jobCronExpression,
                new RecurringJobOptions()
                {
                    TimeZone = TimeZoneInfo.Local,
                    QueueName = HangfireQueueConfiguration.HighPriority
                });
        }
    }
}