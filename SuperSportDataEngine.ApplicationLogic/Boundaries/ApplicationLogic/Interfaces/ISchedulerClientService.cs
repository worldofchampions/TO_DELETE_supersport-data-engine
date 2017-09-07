using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models;
using System.Collections.Generic;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces
{
    public interface ISchedulerClientService
    {
        IEnumerable<SchedulerDashboardUser> GetSchedulerDashboardUsers();
    }
}
