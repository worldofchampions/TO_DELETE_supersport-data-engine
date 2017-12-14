using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models;
using SuperSportDataEngine.Repository.EntityFramework.SystemSportData.Context;

namespace SuperSportDataEngine.Repository.EntityFramework.SystemSportData.Migrations.Seed
{
    static class SeedSchedulingDashboardUsers
    {
        public static void Seed(SystemSportDataContext context)
        {
            if (context.SchedulingDashboardUsers.Any())
                return;

            context.SchedulingDashboardUsers.AddOrUpdate(new SchedulerDashboardUser()
            {
                Username = "SuperSportDataEngine",
                PasswordPlain = "7Cr8d65335C84sc"
            });

            context.SaveChanges();
        }
    }
}
