using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using SuperSportDataEngine.Common.Logging;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Filters
{
    public class LogTimeFilter : ActionFilterAttribute
    {
        private readonly Stopwatch _stopwatch;

        public LogTimeFilter()
        {
            _stopwatch = new Stopwatch();
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            _stopwatch.Reset();
            _stopwatch.Start();
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            _stopwatch.Stop();
            var loggerService = actionExecutedContext.Request.GetDependencyScope().GetService(typeof(ILoggingService)) as ILoggingService;
            loggerService?.Info(actionExecutedContext.ActionContext.ActionDescriptor.ActionName + ": Took " + _stopwatch.ElapsedMilliseconds + "ms.");
        }
    }
}