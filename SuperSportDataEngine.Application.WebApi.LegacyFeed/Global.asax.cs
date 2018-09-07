using System.Web.Configuration;
using SuperSportDataEngine.Application.WebApi.LegacyFeed.Handlers;
using SuperSportDataEngine.Application.WebApi.LegacyFeed.RequestHandlers;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed
{
    using System.Net.Http.Formatting;
    using System.Net.Http.Headers;
    using System.Web.Http;
    using System.Web.Mvc;
    using System.Web.Optimization;

    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            ConfigureFomatters();
            ConfigureRequestHandlers();
            ConfigureApplicationInsightGlobalSettings();
        }

        private static void ConfigureApplicationInsightGlobalSettings()
        {
            try
            {
                var iKey = WebConfigurationManager.AppSettings["AppInsightsInstrumentationKey"];
                if (iKey != null)
                {
                    Microsoft.ApplicationInsights.Extensibility.TelemetryConfiguration.Active.InstrumentationKey = iKey;
                }
            }
            catch (System.Exception)
            {
                Microsoft.ApplicationInsights.Extensibility.TelemetryConfiguration.Active.InstrumentationKey = string.Empty;
            }
        }

        private static void ConfigureRequestHandlers()
        {
            GlobalConfiguration.Configuration.MessageHandlers.Add(new FeedRequestHandler());
            GlobalConfiguration.Configuration.MessageHandlers.Add(new BufferNonStreamedContentHandler());
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