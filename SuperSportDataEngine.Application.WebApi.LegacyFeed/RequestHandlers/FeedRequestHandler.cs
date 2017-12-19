using System.Configuration;
using System.Web.Http;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.RequestHandlers
{
    using Microsoft.Practices.Unity;
    using Container;
    using Container.Enums;
    using Common.Interfaces;
    using Helpers.AppSettings;
    using Models.Shared;
    using ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
    using Config;
    using SuperSportDataEngine.Common.Logging;
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
        private ILoggingService _loggingService;

        private readonly int _authKeyCacheExpiryInMinutes;
        

        public FeedRequestHandler()
        {
            ResolveDependencies();

            _authKeyCacheExpiryInMinutes = 
                int.Parse(ConfigurationManager.AppSettings["AuthKeyCacheExpiryInMinutes"]);
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!IslegacyAuthServiceAvailable())
            {
                var response = new HttpResponseMessage(HttpStatusCode.InternalServerError);

                return response;
            }

            if (!IsRequestRedirectorEnabled())
            {
                return await AuthorizeNewFeedRequest(request, cancellationToken);
            }

            if (!IsOldFeedRequest(request))
            {
                return await AuthorizeNewFeedRequest(request, cancellationToken);
            }

            return await ForwardRequestToOldFeed(request, cancellationToken);
        }


        private bool IslegacyAuthServiceAvailable()
        {
            var results = _legacyAuthService != null;

            return results;
        }

        private void ResolveDependencies()
        {
            var container = new UnityContainer();

            UnityConfigurationManager.RegisterTypes(container, ApplicationScope.WebApiLegacyFeed);

            UnityConfigurationManager.RegisterApiGlobalTypes(container, ApplicationScope.WebApiLegacyFeed);

            _loggingService = container.Resolve<ILoggingService>();

            _legacyAuthService = container.Resolve<ILegacyAuthService>();

            _cache = container.Resolve<ICache>();
        }

        private static bool IsRequestRedirectorEnabled()
        {
            return LegacyFeedConfig.IsRequestRedirectorEnabled;
        }

        private async Task<HttpResponseMessage> AuthorizeNewFeedRequest(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var queryDictionary = HttpUtility.ParseQueryString(request.RequestUri.Query);

            int.TryParse(queryDictionary.Get("site"), out var siteId);

            var auth = queryDictionary.Get("auth");

            var authModel = await GetAuthModelFromCache(siteId, auth);

            if (authModel == null)
            {
                authModel = new AuthModel
                {
                    Authorised = _legacyAuthService.IsAuthorised(auth, siteId)
                };
            }

            if (!authModel.Authorised)
            {
                return GetUnauthorizedResponse();
            }

            PersistRequestToCache(siteId, auth, authModel);

            return await base.SendAsync(request, cancellationToken);
        }

        private static HttpResponseMessage GetUnauthorizedResponse()
        {
            var msg = new HttpResponseMessage(HttpStatusCode.Unauthorized);

            throw new HttpResponseException(msg);
        }

        private Task<AuthModel> GetAuthModelFromCache(int siteId, string auth)
        {
            try
            {
                if (_cache is null)
                {
                    return null;
                }

                return _cache.GetAsync<AuthModel>($"auth/{siteId}/{auth}");
            }
            catch (Exception exception)
            {
                _loggingService.Fatal("GetAuthModelFromCache", exception.Message + exception.StackTrace);

                return null;
            }
        }

        private void PersistRequestToCache(int siteId, string auth, AuthModel authModel)
        {
            try
            {
                _cache.Add($"auth/{siteId}/{auth}", authModel, TimeSpan.FromMinutes(_authKeyCacheExpiryInMinutes));
            }
            catch (Exception exception)
            {
                _loggingService.Fatal("PersistRequestToCache", exception.Message + exception.StackTrace);
            }
        }

        private static bool IsOldFeedRequest(HttpRequestMessage request)
        {
            var isOldFeedRequest = !IsRugbyRequest(request) && !IsAuthRequest(request);

            return isOldFeedRequest;
        }

        private static async Task<HttpResponseMessage> ForwardRequestToOldFeed(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var newUri = new Uri(
                $"http://{SuperSportDataApplicationSettingsHelper.GetSuperSportFeedHost()}" +
                $"{request.RequestUri.GetComponents(UriComponents.PathAndQuery, UriFormat.Unescaped)}");

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

            var isRugbyRequest = matchFixtures.Success | matchLogs.Success | matchResults.Success | matchDetails.Success | liveMatches.Success;

            return isRugbyRequest;
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