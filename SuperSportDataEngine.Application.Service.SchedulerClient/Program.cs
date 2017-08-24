namespace SuperSportDataEngine.Application.Service.SchedulerClient
{
    using Microsoft.Practices.Unity;
    using SuperSportDataEngine.Application.Container;
    using Topshelf;

    internal class Program
    {
        private static void Main()
        {
            var container = new UnityContainer();
            UnityConfigurationManager.RegisterTypes(container);

            HostFactory.Run(hostConfigurator =>
            {
                hostConfigurator.Service<WindowsService>(serviceConfigurator =>
                {
                    serviceConfigurator.ConstructUsing(service => new WindowsService(container));
                    serviceConfigurator.WhenStarted(service => service.StartService());
                    serviceConfigurator.WhenStopped(service => service.StopService());
                });

                hostConfigurator.RunAsLocalService();

                hostConfigurator.SetServiceName("SuperSportDataEngine.Application.Service.SchedulerClient");
                hostConfigurator.SetDisplayName("SuperSportDataEngine - Scheduler Client");
                hostConfigurator.SetDescription("Manages the creation of Hangfire scheduled job definitions, hosts the Hangfire Dashboard, etc. This service is to be deployed to and hosted as a single instance on a single server (does not support horizontal scaling).");
            });
        }
    }
}