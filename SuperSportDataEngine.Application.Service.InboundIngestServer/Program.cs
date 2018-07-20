using Unity;

namespace SuperSportDataEngine.Application.Service.InboundIngestServer
{
    using Container;
    using Container.Enums;
    using Topshelf;

    internal class Program
    {
        private static void Main()
        {
            var container = new UnityContainer();
            UnityConfigurationManager.RegisterTypes(container, ApplicationScope.ServiceInboundIngestServer);

            HostFactory.Run(hostConfigurator =>
            {
                hostConfigurator.Service<WindowsService>(serviceConfigurator =>
                {
                    serviceConfigurator.ConstructUsing(service => new WindowsService(container));
                    serviceConfigurator.WhenStarted(service => service.StartService());
                    serviceConfigurator.WhenStopped(service => service.StopService());
                });

                hostConfigurator.RunAsLocalService();

                hostConfigurator.SetServiceName("SuperSportDataEngine.Application.Service.InboundIngestServer");
                hostConfigurator.SetDisplayName("SuperSportDataEngine - Inbound Ingest Server");
                hostConfigurator.SetDescription("Performs the execution of Hangfire job definitions for inbound (pushed) messages, hosts the job execution workers, etc. This service can be deployed across multiple servers (supports horizontal scaling).");
            });
        }
    }
}