namespace SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models.Enums;

    public interface IMotorsportService
    {
        Task<IEnumerable<MotorsportLeague>> GetActiveLeagues();
        Task<int> GetProviderSeasonIdForLeague(Guid leagueId, CancellationToken cancellationToken);
        Task<SchedulerStateForManagerJobPolling> GetSchedulerStateForManagerJobPolling(Guid leagueId);
        Task<IEnumerable<MotorsportRace>> GetLeagueRacesByProviderSeasonId(Guid leagueId, int providerSeasonId);
    }
}
