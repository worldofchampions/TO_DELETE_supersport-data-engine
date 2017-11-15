using Hangfire.Client;
using Hangfire.Common;
using Hangfire.Server;
using Hangfire.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hangfire.Storage;
using SuperSportDataEngine.Common.Logging;
using Microsoft.Practices.Unity;

namespace SuperSportDataEngine.Application.Service.Common.Hangfire.Filters
{
    public class LogExceptionFilterAttribute : JobFilterAttribute, IElectStateFilter
    {
        private IUnityContainer _container;
        private ILoggingService _logger;

        public LogExceptionFilterAttribute(IUnityContainer container)
        {
            _container = container;
            _logger = _container.Resolve<ILoggingService>();
        }

        public void OnStateElection(ElectStateContext context)
        {
            var failedState = context.CandidateState as FailedState;
            if (failedState != null)
            {
                _logger.Error(
                    "Job `{0}` has been failed due to an exception `{1}`",
                    context.BackgroundJob.Id,
                    failedState.Exception);
            }
        }
    }
}
