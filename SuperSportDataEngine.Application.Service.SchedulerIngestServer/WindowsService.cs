namespace SuperSportDataEngine.Application.Service.SchedulerIngestServer
{
    using Common.Hangfire.Configuration;
    using Common.Hangfire.Filters;
    using Common.Interfaces;
    using Hangfire;
    using Hangfire.Logging;
    using Hangfire.Unity;
    using Microsoft.ApplicationInsights.DependencyCollector;
    using Microsoft.ApplicationInsights.Extensibility;
    using SuperSportDataEngine.Common.Logging;
    using System;
    using System.Configuration;
    using System.Threading.Tasks;
    using Unity;

    internal class WindowsService : IWindowsServiceContract
    {
        private readonly UnityContainer _container;
        private BackgroundJobServer _jobServer;
        private readonly ILoggingService _logger;
        private readonly int _concurrentJobTimeoutInSeconds;

        public WindowsService(UnityContainer container)
        {
            _container = container;

            _logger = _container.Resolve<ILoggingService>();

            _concurrentJobTimeoutInSeconds = int.Parse(ConfigurationManager.AppSettings["ConcurrentJobTimeoutInSeconds"]);
        }

        public void StartService()
        {
            Task.Run(() => { DoServiceWork(); });
        }

        private void DoServiceWork()
        {
            try
            {
                var appInsightKey = ConfigurationManager.AppSettings["AppInsightsInstrumentationKey"];

                var temp = GetApplicationInsightDependencyTrackingModule(appInsightKey);

                if (temp != null)
                {
                    using (temp)
                    {
                        RunSever();
                    }
                }
                else
                {
                    RunSever();
                }
            }
            catch (Exception exception)
            {
                _logger.Warn("AppInsightsInstrumentationKey-Missing", exception);

                RunSever();
            }
        }

        private void RunSever()
        {
            GlobalConfiguration.Configuration.UseStorage(HangfireConfiguration.JobStorage);
            GlobalConfiguration.Configuration.UseUnityActivator(_container);

            ApplyCustomRetryFilterAttribute();

            _jobServer = new BackgroundJobServer(HangfireConfiguration.JobServerOptions);

            // Disable logging for internal Hangfire code.
            // This will NOT prevent our own logging from happening.
            LogProvider.SetCurrentLogProvider(null);
        }

        public void StopService()
        {
            _jobServer.Dispose();
        }

        private void ApplyCustomRetryFilterAttribute()
        {
            RemoveDefaultRetryFilterAttribute();
            var retryPeriodInSeconds = Convert.ToInt32(ConfigurationManager.AppSettings["RetryPeriodInSeconds"]);

            // Removing this logging attrbute.
            // Too much noise.
            //GlobalJobFilters.Filters.Add(new LogExceptionFilterAttribute(_container));
            GlobalJobFilters.Filters.Add(new CustomRetryFilterAttribute(_container, retryPeriodInSeconds));
            GlobalJobFilters.Filters.Add(new SkipConcurrentExecutionAttribute(_concurrentJobTimeoutInSeconds));
            GlobalJobFilters.Filters.Add(new SkipWhenPreviousJobIsRunningAttribute());
        }

        private void RemoveDefaultRetryFilterAttribute()
        {
            object automaticRetryAttribute = null;

            foreach (var filter in GlobalJobFilters.Filters)
                if (filter.Instance is AutomaticRetryAttribute)
                    automaticRetryAttribute = filter.Instance;

            // ok now let's remove it
            if (automaticRetryAttribute == null)
                throw new Exception("Didn't find hangfire automaticRetryAttribute something very wrong");

            GlobalJobFilters.Filters.Remove(automaticRetryAttribute);
        }

        private static IDisposable GetApplicationInsightDependencyTrackingModule(string appInsightKey)
        {
            var configuration = TelemetryConfiguration.Active;

            configuration.InstrumentationKey = appInsightKey;

            configuration.TelemetryInitializers.Add(new OperationCorrelationTelemetryInitializer());

            configuration.TelemetryInitializers.Add(new HttpDependenciesParsingTelemetryInitializer());

            var dependencyTrackingTelemetryModule = new DependencyTrackingTelemetryModule();

            dependencyTrackingTelemetryModule.Initialize(configuration);

            return dependencyTrackingTelemetryModule;
        }
    }
}