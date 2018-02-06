using System.Configuration;
using System.Linq;
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
        private IUnityContainer _container;

        private readonly int _authKeyCacheExpiryInMinutes;
        private const string CacheKeyPrefix = "LegacyFeed:";
        private const string AuthId = "auth";

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
            _container?.Dispose();

            _container = new UnityContainer();

            UnityConfigurationManager.RegisterTypes(_container, ApplicationScope.WebApiLegacyFeed);

            UnityConfigurationManager.RegisterApiGlobalTypes(_container, ApplicationScope.WebApiLegacyFeed);

            _loggingService = _container.Resolve<ILoggingService>();

            _legacyAuthService = _container.Resolve<ILegacyAuthService>();

            _cache = _container.Resolve<ICache>();
        }

        private static bool IsRequestRedirectorEnabled()
        {
            return LegacyFeedConfig.IsRequestRedirectorEnabled;
        }

        private async Task<HttpResponseMessage> AuthorizeNewFeedRequest(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            int siteId = GetSiteIdFromRequest(request);

            string auth = GetAuthValueFromRequest(request);

            if (IsAuthTokenValid(auth))
            {
                return GetUnauthorizedResponse();
            }

            AuthModel authModel = await GetAuthModelFromCache(siteId, auth);

            if (authModel is null)
            {
                authModel = await GetAuthModelFromService(siteId, auth);
            }

            if (authModel.Authorised)
            {
                PersistRequestToCache(siteId, auth, authModel);

                return await base.SendAsync(request, cancellationToken);
            }

            return GetUnauthorizedResponse();
        }

        private async Task<AuthModel> GetAuthModelFromService(int siteId, string authToken)
        {
            var authModel = new AuthModel
            {
                Authorised = await _legacyAuthService.IsAuthorised(authToken, siteId)
            };

            return authModel;
        }

        private static bool IsAuthTokenValid(string auth)
        {
            return string.IsNullOrWhiteSpace(auth);
        }

        private static int GetSiteIdFromRequest(HttpRequestMessage request)
        {
            var queryDictionary = HttpUtility.ParseQueryString(request.RequestUri.Query);

            int.TryParse(queryDictionary.Get("site"), out var siteId);

            return siteId;
        }

        private static string GetAuthValueFromRequest(HttpRequestMessage request)
        {
            var queryDictionary = HttpUtility.ParseQueryString(request.RequestUri.Query);

            var auth = queryDictionary.Get(AuthId);

            if (!string.IsNullOrWhiteSpace(auth)) return auth;

            auth = request.Headers.GetValues(AuthId).FirstOrDefault();

            return auth;
        }

        private static HttpResponseMessage GetUnauthorizedResponse()
        {
            var msg = new HttpResponseMessage(HttpStatusCode.Unauthorized);

            throw new HttpResponseException(msg);
        }

        private async Task<AuthModel> GetAuthModelFromCache(int siteId, string auth)
        {
            try
            {
                if (_cache is null)
                {
                    return null;
                }

                return await _cache.GetAsync<AuthModel>(CacheKeyPrefix + "AUTH:" + $"authToken/{siteId}/{auth}");
            }
            catch (Exception exception)
            {
                await _loggingService.Fatal("GetAuthModelFromCache", exception.Message + exception.StackTrace);

                return null;
            }
        }

        private void PersistRequestToCache(int siteId, string auth, AuthModel authModel)
        {
            try
            {
                _cache.Add(CacheKeyPrefix + "AUTH:" + $"authToken/{siteId}/{auth}", authModel, TimeSpan.FromMinutes(_authKeyCacheExpiryInMinutes));
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
            const string searchTextLowercase = "/authToken/";

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