using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
using System;
using System.Collections.Generic;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Interfaces;
using System.Diagnostics;
using System.Linq;

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
                return _schedulerClientRepository.All().ToList();
            }
            catch (Exception)
            {
                Console.WriteLine("Cannot get scheduler dashboard users.");
                throw;
            }
        }
    }
}
