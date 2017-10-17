namespace SuperSportDataEngine.Application.Service.SchedulerIngestServer
{
    using Hangfire;
    using Microsoft.Practices.Unity;
    using SuperSportDataEngine.Application.Service.Common.Hangfire.Configuration;
    using SuperSportDataEngine.Application.Service.Common.Interfaces;
    using System;
    using System.Threading.Tasks;

    internal class WindowsService : IWindowsServiceContract
    {
        private readonly UnityContainer _container;
        private BackgroundJobServer _jobServer;

        public WindowsService(UnityContainer container)
        {
            _container = container;
        }

        public void StartService()
        {
            GlobalConfiguration.Configuration.UseStorage(HangfireConfiguration.JobStorage);
            GlobalConfiguration.Configuration.UseUnityActivator(_container);

            _jobServer = new BackgroundJobServer(HangfireConfiguration.JobServerOptions);
        }

        public void StopService()
        {
            // TODO: Implement resource disposal/clean-up here.
            _jobServer.Dispose();
        }
    }
}