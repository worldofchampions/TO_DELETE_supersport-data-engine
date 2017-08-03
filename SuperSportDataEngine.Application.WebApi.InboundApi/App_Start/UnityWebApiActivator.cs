[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(SuperSportDataEngine.Application.WebApi.InboundApi.App_Start.UnityWebApiActivator), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethod(typeof(SuperSportDataEngine.Application.WebApi.InboundApi.App_Start.UnityWebApiActivator), "Shutdown")]

namespace SuperSportDataEngine.Application.WebApi.InboundApi.App_Start
{
    using Microsoft.Practices.Unity.WebApi;
    using System.Web.Http;

    /// <summary>Provides the bootstrapping for integrating Unity with WebApi when it is hosted in ASP.NET.</summary>
    public static class UnityWebApiActivator
    {
        /// <summary>Integrates Unity when the application starts.</summary>
        public static void Start()
        {
            var resolver = new UnityDependencyResolver(UnityConfig.GetConfiguredContainer());
            GlobalConfiguration.Configuration.DependencyResolver = resolver;
        }

        /// <summary>Disposes the Unity container when the application is shut down.</summary>
        public static void Shutdown()
        {
            var container = UnityConfig.GetConfiguredContainer();
            container.Dispose();
        }
    }
}