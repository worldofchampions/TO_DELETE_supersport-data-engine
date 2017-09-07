using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
using System;
using System.Collections.Generic;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Interfaces;
using System.Diagnostics;

namespace SuperSportDataEngine.ApplicationLogic.Services
{
    public class SchedulerClientService : ISchedulerClientService
    {
        private readonly IBaseEntityFrameworkRepository<SchedulerDashboardUser> _schedulerClientRepository;

        public SchedulerClientService(IBaseEntityFrameworkRepository<SchedulerDashboardUser> schedulerClientRepository)
        {
            _schedulerClientRepository = schedulerClientRepository;
        }

        public IEnumerable<SchedulerDashboardUser> GetSchedulerDashboardUsers()
        {
            try
            {
#if DEBUG
                var users = new SchedulerDashboardUser[]
                {
                    new SchedulerDashboardUser{Username = "test", PasswordPlain = "test"}
                };
                return users;
#else
                return _schedulerClientRepository.All();
#endif

            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
                throw;
            }
        }
    }
}
