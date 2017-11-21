namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.RequestHandlers
{
    using Microsoft.Practices.Unity;
    using SuperSportDataEngine.Application.Container;
    using SuperSportDataEngine.Application.Container.Enums;
    using SuperSportDataEngine.Application.WebApi.Common.Interfaces;
    using SuperSportDataEngine.Application.WebApi.LegacyFeed.Helpers.AppSettings;
    using SuperSportDataEngine.Application.WebApi.LegacyFeed.Models;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;

    public class FeedRequestHandler : DelegatingHandler
    {
        private ILegacyAuthService _legacyAuthService;
        private ICache _cache;

        protected override async Task<HttpResponseMessage> SendAsync(
                        HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var container = new UnityContainer();
            UnityConfigurationManager.RegisterTypes(container, ApplicationScope.WebApiLegacyFeed);
            _legacyAuthService = container.Resolve<ILegacyAuthService>();
            _cache = container.Resolve<ICache>();

            if (!IsRugbyRequest(request) && !IsAuthRequest(request))
            {
                return await ForwardRequestToOldFeed(request, cancellationToken);
            }

            var queryDictionary = HttpUtility.ParseQueryString(request.RequestUri.Query);

            int.TryParse(queryDictionary.Get("site"), out var siteId);

            var auth = queryDictionary.Get("auth");

            var authModel = await _cache.GetAsync<AuthModel>($"auth/{siteId}/{auth}");

            if (authModel == null)
            {
                authModel = new AuthModel
                {
                    Authorised = _legacyAuthService.IsAuthorised(auth, siteId)
                };
                _cache.Add($"auth/{siteId}/{auth}", authModel);
            }

            if (authModel.Authorised) return await base.SendAsync(request, cancellationToken);
            {
                var response = new HttpResponseMessage(HttpStatusCode.Forbidden);
                return response;
            }
        }

        private static async Task<HttpResponseMessage> ForwardRequestToOldFeed(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var newUri = new Uri(
                $"http://{SuperSportDataApplicationSettingsHelper.GetSuperSportFeedHost()}" +
                $"{request.RequestUri.GetComponents(UriComponents.PathAndQuery, UriFormat.Unescaped)}"
                );
            var requestOldFeed = ChangeHostRequest(request, newUri);
            var client = new HttpClient();
            var response = await client.SendAsync(requestOldFeed, cancellationToken);
            return response;
        }

        private static bool IsRugbyRequest(HttpRequestMessage message)
        {
            var requestUrl = message.RequestUri.ToString();

            var testUrl = requestUrl.Remove(requestUrl.IndexOf('?'));

            var matchFixtures = Regex.Match(testUrl, @"\/rugby\/((?:\w+-)+\w+)\/fixtures");

            var matchLogs = Regex.Match(testUrl, @"\/rugby\/((?:\w+-)+\w+)\/logs");

            var matchResults = Regex.Match(testUrl, @"\/rugby\/((?:\w+-)+\w+)\/results");

            var matchDetails = Regex.Match(testUrl, @"\/rugby\/matchdetails\/");

            var liveMatches = Regex.Match(testUrl, @"\/rugby\/live");

            var isNewFeedRequest = matchFixtures.Success | matchLogs.Success | matchResults.Success | matchDetails.Success | liveMatches.Success;

            return isNewFeedRequest;
        }

        private static bool IsAuthRequest(HttpRequestMessage message)
        {
            const string searchTextLowercase = "/auth/";

            var requestUrlLowercase = message.RequestUri.ToString().ToLower();

            var result = requestUrlLowercase.Contains(searchTextLowercase);

            return result;
        }

        private static HttpRequestMessage ChangeHostRequest(HttpRequestMessage requestMessage, Uri newUri)
        {
            var requestMessageClone = new HttpRequestMessage(requestMessage.Method, newUri);

            if (requestMessage.Method != HttpMethod.Get)
            {
                requestMessageClone.Content = requestMessage.Content;
            }
            requestMessageClone.Version = requestMessage.Version;

            foreach (var property in requestMessage.Properties)
            {
                requestMessageClone.Properties.Add(property);
            }

            foreach (var header in requestMessage.Headers)
            {
                requestMessageClone.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            requestMessageClone.Headers.Host = newUri.Authority;

            return requestMessageClone;
        }
    }
}