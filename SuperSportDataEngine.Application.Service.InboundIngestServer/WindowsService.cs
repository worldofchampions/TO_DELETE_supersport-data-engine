using Unity;

namespace SuperSportDataEngine.Application.Service.InboundIngestServer
{
    using SuperSportDataEngine.Application.Service.Common.Interfaces;

    internal class WindowsService : IWindowsServiceContract
    {
        private readonly UnityContainer _container;

        public WindowsService(UnityContainer container)
        {
            _container = container;
        }

        public void StartService()
        {
            // TODO: Implement service logic here.
        }

        public void StopService()
        {
            // TODO: Implement resource disposal/clean-up here.
        }
    }
}