using System;
using System.Collections.Generic;
using Hangfire;
using Hangfire.Dashboard;
using Microsoft.Practices.Unity;
using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;

namespace SuperSportDataEngine.Application.Service.SchedulerClient
{
    internal class HangfireDashboardConfiguration
    {
        private readonly UnityContainer _unityContainer;
        private static DashboardOptions _dashboardOptions;
        public HangfireDashboardConfiguration(UnityContainer container)
        {
            _unityContainer = container;
        }

        public DashboardOptions GetDashboardOptions()
        {
            if (_dashboardOptions is null)
            {

                var filter = new BasicAuthAuthorizationFilter(new BasicAuthAuthorizationFilterOptions
                {
                    // Require secure connection for dashboard
                    RequireSsl = false,

                    SslRedirect = false,
                    // Case sensitive login checking
                    LoginCaseSensitive = true,

                    // Users
                    Users = GetHangfireDashboardUsers()
                });

                _dashboardOptions = new DashboardOptions { AuthorizationFilters = new[] { filter } };
            }

            return _dashboardOptions;
        }

        private IEnumerable<BasicAuthAuthorizationUser> GetHangfireDashboardUsers()
        {
            var schedulerClientService = _unityContainer.Resolve<ISchedulerClientService>();
            var usersFromService = schedulerClientService.GetSchedulerDashboardUsers();

            var dashboardUser = new List<BasicAuthAuthorizationUser>();
            foreach (var user in usersFromService)
            {
                dashboardUser.Add(new BasicAuthAuthorizationUser { Login = user.Username, PasswordClear = user.PasswordPlain });
            }

            return dashboardUser;
        }
    }
}