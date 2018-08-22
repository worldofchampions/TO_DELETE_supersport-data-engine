using System.Net.Http.Formatting;
using System.Web.Http;
using SuperSportDataEngine.Application.Container;
using SuperSportDataEngine.Application.WebApi.LegacyFeed.Filters;
using Unity;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed
{
    public static class WebApiConfig
    {
        private static HttpConfiguration _httpConfig;
        public static void Register(HttpConfiguration config)
        {
            _httpConfig = config;

            ConfigureDependencyContainer();

            ConfigureResponseFormatters();

            ConfigureApiRoutes();

            ConfigureFeedMappings();

            RegisterLegacyExceptionFilter();
        }

        private static void ConfigureDependencyContainer()
        {
            var container = new UnityContainer();

            UnityConfigurationManager.RegisterApiGlobalTypes(container, Container.Enums.ApplicationScope.WebApiLegacyFeed);

            _httpConfig.DependencyResolver = new UnityDependencyResolver(container);
        }

        private static void RegisterLegacyExceptionFilter()
        {
            GlobalConfiguration.Configuration.Filters.Add(new LegacyExceptionFilter());
        }

        private static void ConfigureResponseFormatters()
        {
            // For All Media Types
            _httpConfig.Formatters.Clear();

            // For XML
            _httpConfig.Formatters.Add(new XmlMediaTypeFormatter());

            _httpConfig.Formatters.XmlFormatter.UseXmlSerializer = true;

            GlobalConfiguration.Configuration.Formatters.XmlFormatter.Indent = true;

            // For JSON
            _httpConfig.Formatters.Add(new JsonMediaTypeFormatter());

            ((Newtonsoft.Json.Serialization.DefaultContractResolver)_httpConfig
                .Formatters.JsonFormatter.SerializerSettings.ContractResolver).IgnoreSerializableAttribute = true;

            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
        }

        private static void ConfigureApiRoutes()
        {
            _httpConfig.MapHttpAttributeRoutes();

            _httpConfig.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{sport}/{category}/{type}/{id}",
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