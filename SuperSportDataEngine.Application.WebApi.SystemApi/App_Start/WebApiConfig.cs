using Microsoft.Practices.Unity;
using SuperSportDataEngine.Application.Container;
using SuperSportDataEngine.Application.WebApi.SystemApi.App_Start;
using SuperSportDataEngine.Application.WebApi.SystemApi.Authentication;
using System.Web.Http.Cors;
using System.Web.Http;

namespace SuperSportDataEngine.Application.WebApi.SystemApi
{
    public static class WebApiConfig
    {
        private static HttpConfiguration _httpConfig;
        public static void Register(HttpConfiguration config)
        {
            _httpConfig = config;

            var cors = new EnableCorsAttribute("*", "*", "*");
            _httpConfig.EnableCors(cors);

            config.Filters.Add(new BasicAuthenticationAttribute());
            ConfigureDependencyContainer();

            ConfigureApiRoutes();
        }

        private static void ConfigureDependencyContainer()
        {
            var container = new UnityContainer();

            UnityConfigurationManager.RegisterApiGlobalTypes(container, Container.Enums.ApplicationScope.WebApiSystemApi);

            _httpConfig.DependencyResolver = new UnityDependencyResolver(container);
        }

        private static void ConfigureApiRoutes()
        {
            // Web API configuration and services

            // Web API routes
            _httpConfig.MapHttpAttributeRoutes();

            /*_httpConfig.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );*/

            _httpConfig.Routes.MapHttpRoute(
                name: "ActionApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { action = "tournaments", id = RouteParameter.Optional }
            );
        }
    }
}
