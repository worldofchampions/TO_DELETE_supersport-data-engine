namespace SuperSportDataEngine.Common.DependencyTracking
{
    using Microsoft.ApplicationInsights.DependencyCollector;
    using Microsoft.ApplicationInsights.Extensibility;

    public class ApplicationInsightDependencyTrackingModule : IApplicationInsightDependencyTrackingModule
    {
        private readonly string _appInsightKey;

        private DependencyTrackingTelemetryModule _dependencyTrackingTelemetryModule;

        public ApplicationInsightDependencyTrackingModule(string appInsightKey)
        {
            _appInsightKey = appInsightKey;

            InitializeDependencyTracker();
        }

        public void Dispose()
        {
            _dependencyTrackingTelemetryModule.Dispose();
        }

        private void InitializeDependencyTracker()
        {
            var configuration = TelemetryConfiguration.Active;

            configuration.InstrumentationKey = _appInsightKey;

            configuration.TelemetryInitializers.Add(new OperationCorrelationTelemetryInitializer());

            configuration.TelemetryInitializers.Add(new HttpDependenciesParsingTelemetryInitializer());

            _dependencyTrackingTelemetryModule = new DependencyTrackingTelemetryModule();

            _dependencyTrackingTelemetryModule.Initialize(configuration);
        }
    }
}