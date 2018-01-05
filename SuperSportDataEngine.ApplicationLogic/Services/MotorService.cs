using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.UnitOfWork;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models.Enums;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.UnitOfWork;

namespace SuperSportDataEngine.ApplicationLogic.Services
{
    public class MotorService: IMotorService
    {

        private readonly IPublicSportDataUnitOfWork _publicSportDataUnitOfWork;
        private readonly ISystemSportDataUnitOfWork _systemSportDataUnitOfWork;

        public MotorService(
            IPublicSportDataUnitOfWork publicSportDataUnitOfWork,
            ISystemSportDataUnitOfWork systemSportDataUnitOfWork)
        {
            _publicSportDataUnitOfWork = publicSportDataUnitOfWork;
            _systemSportDataUnitOfWork = systemSportDataUnitOfWork;
        }

        public async Task<IEnumerable<MotorLeague>> GetActiveLeagues()
        {
            //TODO
            var res = await _publicSportDataUnitOfWork.MotorLeagues.WhereAsync(l => l.IsEnabled);
            return new List<MotorLeague> {new MotorLeague
            {
                Name = "Test-F1",
                IsEnabled = true,
                ProviderSlug = "f1",
            }};
        }

        public async Task<int> GetProviderSeasonIdForLeague(Guid leagueId, CancellationToken cancellationToken)
        {
            return await Task.FromResult(2017);
        }

        public async Task<SchedulerStateForManagerJobPolling> GetSchedulerStateForManagerJobPolling(Guid leagueId)
        {
            return await  Task.FromResult(SchedulerStateForManagerJobPolling.NotRunning);
        }
    }
}
