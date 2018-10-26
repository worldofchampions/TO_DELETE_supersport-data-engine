namespace SuperSportDataEngine.Common.DependencyTracking
{
    using Microsoft.ApplicationInsights.DependencyCollector;
    using Microsoft.ApplicationInsights.Extensibility;

    public class ApplicationInsightsDependencyTrackingModule : IApplicationInsightsDependencyTrackingModule
    {
        private readonly string _appInsightKey;

        private DependencyTrackingTelemetryModule _dependencyTrackingTelemetryModule;

        public ApplicationInsightsDependencyTrackingModule(string appInsightKey)
        {
            _appInsightKey = appInsightKey;

            InitializeDependencyTracker();
        }

        public void Dispose()
        {
            _dependencyTrackingTelemetryModule?.Dispose();
        }

        private void InitializeDependencyTracker()
        {
            if (_appInsightKey == null) return;

            var configuration = TelemetryConfiguration.Active;

            configuration.InstrumentationKey = _appInsightKey;

            configuration.TelemetryInitializers.Add(new OperationCorrelationTelemetryInitializer());

            configuration.TelemetryInitializers.Add(new HttpDependenciesParsingTelemetryInitializer());

            _dependencyTrackingTelemetryModule = new DependencyTrackingTelemetryModule();

            _dependencyTrackingTelemetryModule.Initialize(configuration);
        }
    }
}