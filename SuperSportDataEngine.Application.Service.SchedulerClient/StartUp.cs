using Microsoft.Owin;
using Owin;
using Hangfire;

[assembly: OwinStartup(typeof(SuperSportDataEngine.Application.Service.SchedulerClient.StartUp))]

namespace SuperSportDataEngine.Application.Service.SchedulerClient
{
    public class StartUp
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseHangfireDashboard("/Hangfire");
        }
    }
}
