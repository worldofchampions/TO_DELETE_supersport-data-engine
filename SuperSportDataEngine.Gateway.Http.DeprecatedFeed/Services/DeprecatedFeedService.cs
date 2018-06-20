namespace SuperSportDataEngine.Gateway.Http.DeprecatedFeed.Services
{
    using Newtonsoft.Json;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.DeprecatedFeed.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.DeprecatedFeed.ResponseModels;
    using SuperSportDataEngine.Common.Extentions;
    using SuperSportDataEngine.Common.Logging;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;

    public class DeprecatedFeedService : IDeprecatedFeedService
    {
        private readonly ILoggingService _logger;
        private readonly int _requestDurationWarningMilliseconds;
        private readonly string _authKey;
        private readonly string _host;

        public DeprecatedFeedService(ILoggingService logger)
        {
            _logger = logger;

            _requestDurationWarningMilliseconds = int.Parse(ConfigurationManager.AppSettings["RequestDurationWarningMilliseconds.SuperSportDeprecatedFeed"]);
            _authKey = ConfigurationManager.AppSettings["AuthKey.SuperSportDeprecatedFeed"];
            _host = ConfigurationManager.AppSettings["Host.SuperSportDeprecatedFeed"];
        }

        public async Task<IEnumerable<HighlightVideosResponse>> GetHighlightVideos(string sportName, int legacyFixtureId)
        {
            var webRequest = WebRequest.Create($"{_host}/{sportName}/match/video/highlight/{legacyFixtureId}?format=json&auth={_authKey}");
            webRequest.Method = WebRequestMethods.Http.Get;
            var requestTime = DateTime.Now;

            try
            {
                using (var webResponse = await webRequest.GetResponseAsync())
                {
                    using (var responseStream = webResponse.GetResponseStream())
                    {
                        var streamReader = new StreamReader(responseStream, Encoding.UTF8);
                        var responseString = streamReader.ReadToEnd();
                        var response = JsonConvert.DeserializeObject<List<HighlightVideosResponse>>(responseString);

                        var responseTime = DateTime.Now;
                        CheckIfRequestTakingTooLong(webRequest, requestTime, responseTime);

                        return response;
                    }
                }
            }
            catch (Exception exception)
            {
                var key = GetType().FullName + ".GetHighlightVideos";
                await _logger.Error(key, exception, $"Error requesting data from: {webRequest.GetBaseUri()}.{Environment.NewLine}{key}{Environment.NewLine}{exception}");

                return null;
            }
        }

        public async Task<IEnumerable<LiveVideosResponse>> GetLiveVideos(string sportName, int legacyFixtureId)
        {
            var webRequest = WebRequest.Create($"{_host}/{sportName}/match/video/live/{legacyFixtureId}?format=json&auth={_authKey}");
            webRequest.Method = WebRequestMethods.Http.Get;
            var requestTime = DateTime.Now;

            try
            {
                using (var webResponse = await webRequest.GetResponseAsync())
                {
                    using (var responseStream = webResponse.GetResponseStream())
                    {
                        var streamReader = new StreamReader(responseStream, Encoding.UTF8);
                        var responseString = streamReader.ReadToEnd();
                        var response = JsonConvert.DeserializeObject<List<LiveVideosResponse>>(responseString);

                        var responseTime = DateTime.Now;
                        CheckIfRequestTakingTooLong(webRequest, requestTime, responseTime);

                        return response;
                    }
                }
            }
            catch (Exception exception)
            {
                var key = GetType().FullName + ".GetLiveVideos";
                await _logger.Error(key, exception, $"Error requesting data from: {webRequest.GetBaseUri()}.{Environment.NewLine}{key}{Environment.NewLine}{exception}");

                return null;
            }
        }

        public async Task<int> GetMatchDayBlogId(string sportName, int legacyFixtureId)
        {
            var webRequest = WebRequest.Create($"{_host}/{sportName}/matchdayblog/{legacyFixtureId}?format=json&auth={_authKey}");
            webRequest.Method = WebRequestMethods.Http.Get;
            var requestTime = DateTime.Now;

            try
            {
                using (var webResponse = await webRequest.GetResponseAsync())
                {
                    using (var responseStream = webResponse.GetResponseStream())
                    {
                        var streamReader = new StreamReader(responseStream, Encoding.UTF8);
                        var responseString = streamReader.ReadToEnd();
                        var response = JsonConvert.DeserializeObject<MatchDayBlogResponse>(responseString);

                        var responseTime = DateTime.Now;
                        CheckIfRequestTakingTooLong(webRequest, requestTime, responseTime);

                        return response.ID;
                    }
                }
            }
            catch (Exception exception)
            {
                var key = GetType().FullName + ".GetMatchDayBlogId";
                await _logger.Error(key, exception, $"Error requesting data from: {webRequest.GetBaseUri()}.{Environment.NewLine}{key}{Environment.NewLine}{exception}");

                return 0;
            }
        }

        public async Task<int> GetMatchPreviewId(string sportName, int legacyFixtureId)
        {
            var webRequest = WebRequest.Create($"{_host}/{sportName}/match/preview/{legacyFixtureId}?format=json&auth={_authKey}");
            webRequest.Method = WebRequestMethods.Http.Get;
            var requestTime = DateTime.Now;
            try
            {
                using (var webResponse = await webRequest.GetResponseAsync())
                {
                    using (var responseStream = webResponse.GetResponseStream())
                    {
                        var streamReader = new StreamReader(responseStream, Encoding.UTF8);
                        var responseString = streamReader.ReadToEnd();
                        var response = JsonConvert.DeserializeObject<MatchPreviewResponse>(responseString);

                        var responseTime = DateTime.Now;
                        CheckIfRequestTakingTooLong(webRequest, requestTime, responseTime);

                        return response.ID;
                    }
                }
            }
            catch (Exception exception)
            {
                var key = GetType().FullName + ".GetMatchPreviewId";
                await _logger.Error(key, exception, $"Error requesting data from: {webRequest.GetBaseUri()}.{Environment.NewLine}{key}{Environment.NewLine}{exception}");

                return 0;
            }
        }

        public async Task<int> GetMatchReportId(string sportName, int legacyFixtureId)
        {
            var webRequest = WebRequest.Create($"{_host}/{sportName}/match/report/{legacyFixtureId}?format=json&auth={_authKey}");
            webRequest.Method = WebRequestMethods.Http.Get;
            var requestTime = DateTime.Now;

            try
            {
                using (var webResponse = await webRequest.GetResponseAsync())
                {
                    using (var responseStream = webResponse.GetResponseStream())
                    {
                        var streamReader = new StreamReader(responseStream, Encoding.UTF8);
                        var responseString = streamReader.ReadToEnd();
                        var response = JsonConvert.DeserializeObject<MatchReportResponse>(responseString);

                        var responseTime = DateTime.Now;
                        CheckIfRequestTakingTooLong(webRequest, requestTime, responseTime);

                        return response.ID;
                    }
                }
            }
            catch (Exception exception)
            {
                var key = GetType().FullName + ".GetMatchReportId";
                await _logger.Error(key, exception, $"Error requesting data from: {webRequest.GetBaseUri()}.{Environment.NewLine}{key}{Environment.NewLine}{exception}");

                return 0;
            }
        }

        private void CheckIfRequestTakingTooLong(WebRequest request, DateTime requestTime, DateTime responseTime)
        {
            if (request == null)
                return;

            var durationMilliseconds = (responseTime - requestTime).TotalMilliseconds;

            if (durationMilliseconds > _requestDurationWarningMilliseconds)
            {
                _logger.Warn($"HTTPRequestTooLong.{request.RequestUri}",
                    $"HTTP request taking too long. {request.GetBaseUri()}. Warning level is {_requestDurationWarningMilliseconds / 1000.0} seconds; took " + durationMilliseconds / 1000.0 + " seconds.");
            }
        }
    }
}
