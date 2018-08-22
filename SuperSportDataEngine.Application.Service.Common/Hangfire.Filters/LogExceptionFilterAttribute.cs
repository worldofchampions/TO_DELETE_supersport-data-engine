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
using Unity;

namespace SuperSportDataEngine.Application.Service.Common.Hangfire.Filters
{
    public class LogExceptionFilterAttribute : JobFilterAttribute, IElectStateFilter
    {
        private readonly ILoggingService _logger;

        public LogExceptionFilterAttribute(IUnityContainer container)
        {
            _logger = container.Resolve<ILoggingService>();
        }

        public void OnStateElection(ElectStateContext context)
        {
            if (context.CandidateState is FailedState failedState)
            {
                _logger.Error(
                    "Job `{0}` has been failed due to an exception `{1}`",
                    failedState.Exception,
                    $"Message: {failedState.Exception.Message}\n" +
                    $"Stack Trace: {failedState.Exception.StackTrace}\n" +
                    $"Inner Exception: {failedState.Exception.InnerException}");
            }
        }
    }
}
