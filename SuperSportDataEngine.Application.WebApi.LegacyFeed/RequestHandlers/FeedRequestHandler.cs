using SuperSportDataEngine.Application.WebApi.LegacyFeed.Helpers.AppSettings;
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
        protected override Task<HttpResponseMessage> SendAsync(
                        HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var newUri = new Uri(
                $"http://{SuperSportDataApplicationSettingsHelper.GetSuperSportFeedHost()}" +
                $"{request.RequestUri.GetComponents(UriComponents.PathAndQuery, UriFormat.Unescaped)}"
                );
            var requestOldFeed = ChangeHostRequest(request, newUri);
            var client = new HttpClient();
            var response = client.SendAsync(requestOldFeed);
            Task.WaitAll(new Task[] { response });
            return response;
        }

        private bool IsRugbyRequest(HttpRequestMessage message)
        {
            var requestUrl = message.RequestUri.ToString();
            var match = Regex.Match(requestUrl, @"/rugby/");
            return match.Success;
        }

        public HttpRequestMessage ChangeHostRequest(HttpRequestMessage req, Uri newUri)
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