namespace SuperSportDataEngine.Application.WebApi.InboundApi.App_Start
{
    using Microsoft.Practices.Unity;
    using SuperSportDataEngine.Application.Container;
    using SuperSportDataEngine.Application.Container.Enums;
    using System;

    /// <summary>Specifies the Unity configuration for the main container.</summary>
    public class UnityConfig
    {
        private static Lazy<IUnityContainer> container = new Lazy<IUnityContainer>(() =>
        {
            var container = new UnityContainer();
            RegisterTypes(container);
            return container;
        });

        public static IUnityContainer GetConfiguredContainer()
        {
            return container.Value;
        }

        private static void RegisterTypes(IUnityContainer container)
        {
            UnityConfigurationManager.RegisterTypes(container, ApplicationScope.WebApiInboundApi);
        }
    }
}