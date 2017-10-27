namespace SuperSportDataEngine.Application.Service.SchedulerIngestServer
{
    using Hangfire;
    using Microsoft.Practices.Unity;
    using SuperSportDataEngine.Application.Service.Common.Hangfire.Configuration;
    using SuperSportDataEngine.Application.Service.Common.Hangfire.Filters;
    using SuperSportDataEngine.Application.Service.Common.Interfaces;
    using System;
    using System.Configuration;

    internal class WindowsService : IWindowsServiceContract
    {
        private readonly UnityContainer _container;
        private BackgroundJobServer _jobServer;

        public WindowsService(UnityContainer container)
        {
            _container = container;
        }

        public void StartService()
        {
            GlobalConfiguration.Configuration.UseStorage(HangfireConfiguration.JobStorage);
            GlobalConfiguration.Configuration.UseUnityActivator(_container);

            ApplyCustomRetryFilterAttribute();

            _jobServer = new BackgroundJobServer(HangfireConfiguration.JobServerOptions);
        }

        public void StopService()
        {
            _jobServer.Dispose();
        }

        private void ApplyCustomRetryFilterAttribute()
        {
            RemoveDefaultRetryFilterAttribute();
            var retryPeriodInSeconds = Convert.ToInt32(ConfigurationManager.AppSettings["RetryPeriodInSeconds"]);
            GlobalJobFilters.Filters.Add(new CustomRetryFilterAttribute(retryPeriodInSeconds));
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
    }
}