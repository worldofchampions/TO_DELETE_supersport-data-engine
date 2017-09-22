using SuperSportDataEngine.Application.WebApi.LegacyFeed.App_Start;
using SuperSportDataEngine.Application.WebApi.LegacyFeed.RequestHandlers;
using System.Web.Http;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed
{
    public static class WebApiConfig
    {
        private static HttpConfiguration httpConfig;
        public static void Register(HttpConfiguration config)
        {
            httpConfig = config;

            httpConfig.Formatters.XmlFormatter.UseXmlSerializer = true;

            ConfigureApiRoutes();

            ConfigureFeedRequestHandler();

            ConfigureFeedMappings();
        }

        private static void ConfigureFeedRequestHandler()
        {
            httpConfig.MessageHandlers.Add(new FeedRequestHandler());
        }

        private static void ConfigureApiRoutes()
        {
            httpConfig.MapHttpAttributeRoutes();

            httpConfig.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{sport}/{category}",
                defaults: new
                {
                    category = RouteParameter.Optional,
                    type = RouteParameter.Optional,
                    id = RouteParameter.Optional
                }
            );
        }

        private static void ConfigureFeedMappings()
        {
            AutoMapperConfig.InitializeMappings();
        }
    }
}