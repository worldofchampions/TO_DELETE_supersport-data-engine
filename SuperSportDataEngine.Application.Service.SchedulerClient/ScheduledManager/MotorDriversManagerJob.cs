namespace SuperSportDataEngine.Application.Service.SchedulerClient.ScheduledManager
{
    using System;
    using System.Configuration;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Hangfire;
    using Microsoft.Practices.Unity;
    using SuperSportDataEngine.Application.Container;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models.Enums;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.UnitOfWork;

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


        private async Task<int> CreateChildJobsForFetchingRaceDrivers()
        {
            using (var unitOfWork = _childContainer.Resolve<ISystemSportDataUnitOfWork>())
            {
                var leagues = await _childContainer.Resolve<IMotorsportService>().GetActiveLeagues();

                foreach (var league in leagues)
                {
                    var seasonId = await _childContainer.Resolve<IMotorsportService>().GetProviderSeasonIdForLeague(league.Id, CancellationToken.None);

                    if (await _childContainer.Resolve<IMotorsportService>().GetSchedulerStateForManagerJobPolling(league.Id) == SchedulerStateForManagerJobPolling.NotRunning)
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
            //TODO
        }



        private void AddOrUpdateHangfireJob(string providerSlug, int providerSeasonId, string jobId, string jobCronExpression)
        {
            //TODO
        }
    }
}