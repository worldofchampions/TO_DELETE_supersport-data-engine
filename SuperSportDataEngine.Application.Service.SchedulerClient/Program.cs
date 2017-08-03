namespace SuperSportDataEngine.Application.Service.SchedulerClient
{
    using Microsoft.Practices.Unity;
    using SuperSportDataEngine.Application.Container;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
    using System;

    internal class Program
    {
        private static void Main(string[] args)
        {
            // TODO: [Davide] Finalize the DI handling here after integrating with TopShelf, Hangfire etc.
            var container = new UnityContainer();
            UnityConfigurationManager.RegisterTypes(container);

            var temporaryExampleService = container.Resolve<ITemporaryExampleService>();

            Console.WriteLine(temporaryExampleService.HelloMessage());
            Console.ReadLine();
        }
    }
}