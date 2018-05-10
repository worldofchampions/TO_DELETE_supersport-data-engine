using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
using SuperSportDataEngine.Common.Logging;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Filters
{
    public class LogUserAgentFilterAttribute : ActionFilterAttribute
    {
        public override async Task OnActionExecutingAsync(HttpActionContext context, CancellationToken cancellationToken)
        {
            if (!(context.Request.GetDependencyScope().GetService(typeof(IRugbyService)) is IRugbyService rugbyService))
                return;

            var controllerParameters = context.ActionArguments;
            var hasCategory = controllerParameters.ContainsKey("category");
            var category = 
                     hasCategory ?
                        (string) controllerParameters["category"] :
                        null;

            if (category == null)
                return;

            var isInvalidSlug = 
                await rugbyService.GetTournamentId(category) == Guid.Empty &&
                !rugbyService.IsNationalTeamSlug(category);

            var userAgent = context.Request.Headers.UserAgent;
            var queryDictionary = HttpUtility.ParseQueryString(context.Request.RequestUri.Query);
            var authKey = queryDictionary.Get("auth")?.Substring(0, 5);

            if (!(context.Request.GetDependencyScope().GetService(typeof(ILoggingService)) is ILoggingService logger))
                return;

            if (isInvalidSlug)
            {
                await logger.Warn("InvalidSlugName.UserAgent." + category,
                    "Invalid tournament slug =\"" + category + "\". Request coming " +
                    "from User Agent = " + userAgent + 
                    " with Auth Key = " + authKey,
                    TimeSpan.FromDays(7));
            }
        }
    }
}