namespace SuperSportDataEngine.Application.Service.SchedulerIngestServer
{
    using Microsoft.Practices.Unity;
    using SuperSportDataEngine.Application.Container;
    using SuperSportDataEngine.Application.Container.Enums;
    using Topshelf;

    internal class Program
    {
        private static void Main()
        {
            var container = new UnityContainer();
            UnityConfigurationManager.RegisterTypes(container, ApplicationScope.ServiceSchedulerIngestServer);

            HostFactory.Run(hostConfigurator =>
            {
                hostConfigurator.Service<WindowsService>(serviceConfigurator =>
                {
                    serviceConfigurator.ConstructUsing(service => new WindowsService(container));
                    serviceConfigurator.WhenStarted(service => service.StartService());
                    serviceConfigurator.WhenStopped(service => service.StopService());
                });

                hostConfigurator.RunAsLocalService();

                hostConfigurator.SetServiceName("SuperSportDataEngine.Application.Service.SchedulerIngestServer");
                hostConfigurator.SetDisplayName("SuperSportDataEngine - Scheduler Ingest Server");
                hostConfigurator.SetDescription("Performs the execution of scheduled Hangfire job definitions, hosts the job execution workers, etc. This service can be deployed across multiple servers (supports horizontal scaling).");
            });
        }
    }
}