using Microsoft.Practices.Unity;
using SuperSportDataEngine.Application.Container;
using SuperSportDataEngine.Application.WebApi.LegacyFeed.Helpers.AppSettings;
using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.RequestHandlers
{
    public class FeedRequestHandler : DelegatingHandler
    {
        private readonly ITemporaryExampleService _temporaryExampleService;
        private readonly UnityContainer container = new UnityContainer();
        public FeedRequestHandler()
        {
            var container = new UnityContainer();
            UnityConfigurationManager.RegisterTypes(container);
            _temporaryExampleService = container.Resolve<ITemporaryExampleService>();
        }

        protected override Task<HttpResponseMessage> SendAsync(
                        HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var authorised = _temporaryExampleService.HelloMessage().Contains(""); // service for authenticating
            if (authorised)
            {
                var response = new HttpResponseMessage(System.Net.HttpStatusCode.Forbidden);
                return Task.Run(() =>
                {
                    return response;
                });
            }
            if (!IsRugbyRequest(request))
            {
                var newUri = new Uri(
                    $"http://{SuperSportDataApplicationSettingsHelper.GetSuperSportFeedHost()}" +
                    $"{request.RequestUri.GetComponents(UriComponents.PathAndQuery, UriFormat.Unescaped)}"
                    );
                var requestOldFeed = ChangeHostRequest(request, newUri);
                var client = new HttpClient();
                var response = client.SendAsync(requestOldFeed);
                return response;
            }
            return base.SendAsync(request, cancellationToken);
        }

        private bool IsRugbyRequest(HttpRequestMessage message)
        {
            var requestUrl = message.RequestUri.ToString();
            var testUrl = requestUrl.Remove(requestUrl.IndexOf('?'));
            if(testUrl.Contains("/matchdetails") && testUrl.Contains("/rugby/"))
            {
                return true;
            }
            var match = Regex.Match(testUrl, @"\/rugby\/((?:\w+-)+\w+)\/fixtures
                                                    |\/rugby\/((?:\w+-)+\w+)\/logs
                                                    |\/rugby\/((?:\w+-)+\w+)\/results");
            return match.Success;
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