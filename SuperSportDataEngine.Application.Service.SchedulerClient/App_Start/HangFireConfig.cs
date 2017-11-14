using HangFire;
using HangFire.Web;

[assembly: WebActivatorEx.PostApplicationStartMethod(
    typeof(SuperSportDataEngine.Application.Service.SchedulerClient.HangFireConfig), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethod(
    typeof(SuperSportDataEngine.Application.Service.SchedulerClient.HangFireConfig), "Stop")]

namespace SuperSportDataEngine.Application.Service.SchedulerClient
{
    public class HangFireConfig
    {
        private static AspNetBackgroundJobServer _server;

        public static void Start()
        {
            _server = new AspNetBackgroundJobServer();
            _server.Start();
        }

        public static void Stop()
        {
            _server.Stop();
        }
    }
}
