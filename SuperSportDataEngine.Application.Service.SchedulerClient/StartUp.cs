using Microsoft.Owin;
using Owin;
using SuperSportDataEngine.Application.Container;
using SuperSportDataEngine.Application.Container.Enums;
using Hangfire.Dashboard;
using Hangfire.Logging;
using Hangfire;
using Unity;

[assembly: OwinStartup(typeof(SuperSportDataEngine.Application.Service.SchedulerClient.StartUp))]

namespace SuperSportDataEngine.Application.Service.SchedulerClient
{
    public class StartUp
    {
        public void Configuration(IAppBuilder app)
        {
            var container = new UnityContainer();
            UnityConfigurationManager.RegisterTypes(container, ApplicationScope.ServiceSchedulerClient);

            var options = new HangfireDashboardConfiguration(container).GetDashboardOptions();
            app.UseHangfireDashboard("/Hangfire", options);

            // Disable logging for internal Hangfire code.
            // This will NOT prevent our own logging from happening.
            LogProvider.SetCurrentLogProvider(null);
        }
    }
}
