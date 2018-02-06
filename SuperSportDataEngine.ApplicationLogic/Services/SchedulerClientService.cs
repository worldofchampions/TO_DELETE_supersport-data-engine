using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
using System;
using System.Collections.Generic;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Interfaces;
using System.Diagnostics;
using System.Linq;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.UnitOfWork;

namespace SuperSportDataEngine.ApplicationLogic.Services
{
    public class SchedulerClientService : ISchedulerClientService
    {
        private readonly ISystemSportDataUnitOfWork _systemSportDataUnitOfWork;

        public SchedulerClientService(ISystemSportDataUnitOfWork systemSportDataUnitOfWork)
        {
            _systemSportDataUnitOfWork = systemSportDataUnitOfWork;
        }

        public IEnumerable<SchedulerDashboardUser> GetSchedulerDashboardUsers()
        {
            try
            {
                return _systemSportDataUnitOfWork.SchedulerDashboardUsers.All().ToList();
            }
            catch (Exception)
            {
                Console.WriteLine("Cannot get scheduler dashboard users.");
                throw;
            }
        }
    }
}
