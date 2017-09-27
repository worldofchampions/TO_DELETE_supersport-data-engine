using SuperSportDataEngine.Application.WebApi.LegacyFeed.App_Start;
using SuperSportDataEngine.Application.WebApi.LegacyFeed.RequestHandlers;
using System.Net.Http.Formatting;
using System.Web.Http;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed
{
    public static class WebApiConfig
    {
        private static HttpConfiguration httpConfig;
        public static void Register(HttpConfiguration config)
        {
            httpConfig = config;

            ConfigureResponseFormatters();

            ConfigureApiRoutes();

            ConfigureFeedRequestHandler();

            ConfigureFeedMappings();

            RegisterLegacyExceptionFilter();
        }

        private static void RegisterLegacyExceptionFilter()
        {
            GlobalConfiguration.Configuration.Filters.Add(new Filters.LegacyExceptionFilterAttribute());
        }

        private static void ConfigureResponseFormatters()
        {
            // For All Media Types
            httpConfig.Formatters.Clear();

            //httpConfig.Formatters.Add(new FormUrlEncodedMediaTypeFormatter());

            // For XML
            httpConfig.Formatters.Add(new XmlMediaTypeFormatter());

            httpConfig.Formatters.XmlFormatter.UseXmlSerializer = true;

            GlobalConfiguration.Configuration.Formatters.XmlFormatter.Indent = true;

            // For JSON
            httpConfig.Formatters.Add(new JsonMediaTypeFormatter());

            ((Newtonsoft.Json.Serialization.DefaultContractResolver)httpConfig
                .Formatters.JsonFormatter.SerializerSettings.ContractResolver).IgnoreSerializableAttribute = true;

            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;

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