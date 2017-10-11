namespace SuperSportDataEngine.Application.Service.SchedulerIngestServer
{
    using Hangfire;
    using Microsoft.Practices.Unity;
    using SuperSportDataEngine.Application.Service.Common.Hangfire.Configuration;
    using SuperSportDataEngine.Application.Service.Common.Hangfire.Filters;
    using SuperSportDataEngine.Application.Service.Common.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Services;
    using System;
    using System.Threading.Tasks;

    internal class WindowsService : IWindowsServiceContract
    {
        private readonly UnityContainer _container;

        public WindowsService(UnityContainer container)
        {
            _container = container;
        }

        public void StartService()
        {
            Task.Run(() => { DoServiceWork(); });
        }

        private void DoServiceWork()
        {
            var ingestService = _container.Resolve<IRugbyIngestWorkerService>();

            GlobalConfiguration.Configuration.UseStorage(HangfireConfiguration.JobStorage);
            GlobalConfiguration.Configuration.UseActivator(new ContainerJobActivator(_container));

            using (var server = new BackgroundJobServer(HangfireConfiguration.JobServerOptions))
            {
                JobStorage.Current = HangfireConfiguration.JobStorage;
                // Processing of jobs happen here.

                Console.ReadLine();
            }
        }

        public void StopService()
        {
            // TODO: Implement resource disposal/clean-up here.
        }
    }
}