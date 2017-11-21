using Microsoft.Owin;
using Owin;
using Hangfire;
using Microsoft.Practices.Unity;
using SuperSportDataEngine.Application.Container;
using SuperSportDataEngine.Application.Container.Enums;
using Hangfire.Dashboard;
using Hangfire.Logging;

[assembly: OwinStartup(typeof(SuperSportDataEngine.Application.Service.SchedulerClient.StartUp))]

namespace SuperSportDataEngine.Application.Service.SchedulerClient
{
    public class StartUp
    {
        public void Configuration(IAppBuilder app)
        {
            var container = new UnityContainer();
            UnityConfigurationManager.RegisterTypes(container, ApplicationScope.ServiceSchedulerClient);

            DashboardRoutes.Routes.AddRazorPage("/recurring2", x => new Hangfire.Dashboard.Pages.RecurringJobsPage());
            NavigationMenu.Items.Add(page => new MenuItem("Recurring Jobs 2", page.Url.To("/recurring2"))
            {
                Active = page.RequestPath.StartsWith("/recurring2"),
                Metric = DashboardMetrics.RecurringJobCount
            });

            var options = new HangfireDashboardConfiguration(container).GetDashboardOptions();
            app.UseHangfireDashboard("/Hangfire", options);

            // Disable logging for internal Hangfire code.
            // This will NOT prevent our own logging from happening.
            LogProvider.SetCurrentLogProvider(null);
        }
    }
}
