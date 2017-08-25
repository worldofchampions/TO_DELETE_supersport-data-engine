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
    using System.Collections.Generic;
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
        private readonly UnityContainer container = new UnityContainer();

        public FeedRequestHandler()
        {
        }

        protected override async Task<HttpResponseMessage> SendAsync(
                        HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var container = new UnityContainer();
            UnityConfigurationManager.RegisterTypes(container, ApplicationScope.WebApiLegacyFeed);
            _legacyAuthService = container.Resolve<ILegacyAuthService>();
            _cache = container.Resolve<ICache>();

            if (!IsRugbyRequest(request) && !IsAuthRequest(request))
            {
                var newUri = new Uri(
                    $"http://{SuperSportDataApplicationSettingsHelper.GetSuperSportFeedHost()}" +
                    $"{request.RequestUri.GetComponents(UriComponents.PathAndQuery, UriFormat.Unescaped)}"
                    );
                var requestOldFeed = ChangeHostRequest(request, newUri);
                var client = new HttpClient();
                var response = await client.SendAsync(requestOldFeed);
                return response;
            }

            var queryDictionary = HttpUtility.ParseQueryString(request.RequestUri.Query.ToString());
            int siteId;
            Int32.TryParse(queryDictionary.Get("site"), out siteId);
            var auth = queryDictionary.Get("auth");
            var authModel = await _cache.GetAsync<AuthModel>($"auth/{siteId}/{auth}");
            if (authModel == null)
            {
                authModel = new AuthModel
                {
                    Authorised = _legacyAuthService.IsAuthorised(auth, siteId)
                };
                _cache.Add<AuthModel>($"auth/{siteId}/{auth}", authModel);
            }

            if (!authModel.Authorised)
            {
                var response = new HttpResponseMessage(HttpStatusCode.Forbidden);
                return response;
            }

            return await base.SendAsync(request, cancellationToken);
        }

        private bool IsRugbyRequest(HttpRequestMessage message)
        {
            var requestUrl = message.RequestUri.ToString();
            var testUrl = requestUrl.Remove(requestUrl.IndexOf('?'));
            if (testUrl.Contains("/matchdetails") && testUrl.Contains("/rugby/"))
            {
                return true;
            }

            var matchFixtures = Regex.Match(testUrl, @"\/rugby\/((?:\w+-)+\w+)\/fixtures");
            var matchLogs = Regex.Match(testUrl, @"\/rugby\/((?:\w+-)+\w+)\/logs");
            var matchResults = Regex.Match(testUrl, @"\/rugby\/((?:\w+-)+\w+)\/results");

            return matchFixtures.Success | matchLogs.Success | matchResults.Success;
        }

        private bool IsAuthRequest(HttpRequestMessage message)
        {
            var requestUrl = message.RequestUri.ToString();
            return (requestUrl.Contains("/Auth/") | requestUrl.Contains("/auth/"));
        }

        private HttpRequestMessage ChangeHostRequest(HttpRequestMessage req, Uri newUri)
        {
            var clone = new HttpRequestMessage(req.Method, newUri);

            if (req.Method != HttpMethod.Get)
            {
                clone.Content = req.Content;
            }
            clone.Version = req.Version;

            foreach (KeyValuePair<string, object> prop in req.Properties)
            {
                clone.Properties.Add(prop);
            }

            foreach (KeyValuePair<string, IEnumerable<string>> header in req.Headers)
            {
                clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            clone.Headers.Host = newUri.Authority;

            return clone;
        }
    }
}