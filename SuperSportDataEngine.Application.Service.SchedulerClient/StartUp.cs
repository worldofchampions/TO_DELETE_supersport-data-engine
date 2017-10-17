using Microsoft.Owin;
using Owin;
using Hangfire;
using SuperSportDataEngine.Application.Service.Common.Hangfire.Configuration;
using Microsoft.Practices.Unity;
using SuperSportDataEngine.Application.Container;
using SuperSportDataEngine.Application.Container.Enums;
using Hangfire.Dashboard;

[assembly: OwinStartup(typeof(SuperSportDataEngine.Application.Service.SchedulerClient.StartUp))]

namespace SuperSportDataEngine.Application.Service.SchedulerClient
{
    public class StartUp
    {
        public void Configuration(IAppBuilder app)
        {
            var container = new UnityContainer();
            UnityConfigurationManager.RegisterTypes(container, ApplicationScope.ServiceSchedulerClient);

            app.UseHangfireDashboard("/Hangfire", options: new HangfireDashboardConfiguration(container).GetDashboardOptions());
        }
    }
}
