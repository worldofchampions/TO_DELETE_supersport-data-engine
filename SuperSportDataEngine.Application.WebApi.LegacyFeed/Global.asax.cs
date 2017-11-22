using SuperSportDataEngine.Application.WebApi.LegacyFeed.RequestHandlers;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed
{
    using System.Net.Http.Formatting;
    using System.Net.Http.Headers;
    using System.Web.Http;
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;

    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            ConfigureFomatters();
            ConfigureRequestHandlers();
        }

        private void ConfigureRequestHandlers()
        {
            GlobalConfiguration.Configuration.MessageHandlers.Add(new FeedRequestHandler());
        }

        private static void ConfigureFomatters()
        {
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.MediaTypeMappings.Add(
                new QueryStringMapping("format", "json", new MediaTypeHeaderValue("application/json")));

            GlobalConfiguration.Configuration.Formatters.XmlFormatter.MediaTypeMappings.Add(
                new QueryStringMapping("format", "xml", new MediaTypeHeaderValue("application/xml")));
        }
    }
}