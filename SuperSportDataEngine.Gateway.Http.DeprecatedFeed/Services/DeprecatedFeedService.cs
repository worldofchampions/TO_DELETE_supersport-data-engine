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
        private readonly int _requestTimeoutDurationInMilliseconds;

        public DeprecatedFeedService(ILoggingService logger)
        {
            _logger = logger;

            _requestDurationWarningMilliseconds = int.Parse(ConfigurationManager.AppSettings["RequestDurationWarningMilliseconds.SuperSportDeprecatedFeed"]);
            _authKey = ConfigurationManager.AppSettings["AuthKey.SuperSportDeprecatedFeed"];
            _requestTimeoutDurationInMilliseconds = int.Parse(ConfigurationManager.AppSettings["RequestTimeoutDurationInMilliseconds"]);

            _host = ConfigurationManager.AppSettings["Host.SuperSportDeprecatedFeed"];
        }

        public async Task<IEnumerable<HighlightVideosResponse>> GetHighlightVideos(string sportName, int legacyFixtureId)
        {
            var webRequest = WebRequest.Create($"{_host}/{sportName}/match/video/highlight/{legacyFixtureId}?format=json&auth={_authKey}");
            webRequest.Method = WebRequestMethods.Http.Get;
            var requestTime = DateTime.Now;

            try
            {
                using (var webResponse = await webRequest.GetResponseAsync(_requestTimeoutDurationInMilliseconds, _logger))
                {
                    if (webResponse == null)
                        return null;

                    using (var responseStream = webResponse.GetResponseStream())
                    {
                        if (responseStream != null)
                        {
                            var streamReader = new StreamReader(responseStream, Encoding.UTF8);
                            var responseString = streamReader.ReadToEnd();

                            var response = JsonConvert.DeserializeObject<List<HighlightVideosResponse>>(responseString);
                            if (response == null)
                                return null;

                            var responseTime = DateTime.Now;
                            CheckIfRequestTakingTooLong(webRequest, requestTime, responseTime);

                            return response;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                var key = GetType().FullName + ".GetHighlightVideos";
                await _logger.Error(key, exception, $"Error requesting data from: {webRequest.GetBaseUri()}.{Environment.NewLine}{key}{Environment.NewLine}{exception.Message}" +
                                                    $"{Environment.NewLine}{exception}" +
                                                    $"{Environment.NewLine}{exception.InnerException}");
            }

            return null;
        }

        public async Task<IEnumerable<LiveVideosResponse>> GetLiveVideos(string sportName, int legacyFixtureId)
        {
            var webRequest = WebRequest.Create($"{_host}/{sportName}/match/video/live/{legacyFixtureId}?format=json&auth={_authKey}");
            webRequest.Method = WebRequestMethods.Http.Get;
            var requestTime = DateTime.Now;

            try
            {
                using (var webResponse = await webRequest.GetResponseAsync(_requestTimeoutDurationInMilliseconds, _logger))
                {
                    if (webResponse == null)
                        return null;

                    using (var responseStream = webResponse.GetResponseStream())
                    {
                        if (responseStream != null)
                        {
                            var streamReader = new StreamReader(responseStream, Encoding.UTF8);
                            var responseString = streamReader.ReadToEnd();

                            var response = JsonConvert.DeserializeObject<List<LiveVideosResponse>>(responseString);
                            if (response == null)
                                return null;

                            var responseTime = DateTime.Now;
                            CheckIfRequestTakingTooLong(webRequest, requestTime, responseTime);

                            return response;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                var key = GetType().FullName + ".GetLiveVideos";
                await _logger.Error(key, exception, $"Error requesting data from: {webRequest.GetBaseUri()}.{Environment.NewLine}{key}{Environment.NewLine}{exception.Message}" +
                                                    $"{Environment.NewLine}{exception}" +
                                                    $"{Environment.NewLine}{exception.InnerException}");
            }

            return null;
        }

        public async Task<int> GetMatchDayBlogId(string sportName, int legacyFixtureId)
        {
            var webRequest = WebRequest.Create($"{_host}/{sportName}/matchdayblog/{legacyFixtureId}?format=json&auth={_authKey}");
            webRequest.Method = WebRequestMethods.Http.Get;
            var requestTime = DateTime.Now;

            try
            {
                using (var webResponse = await webRequest.GetResponseAsync(_requestTimeoutDurationInMilliseconds, _logger))
                {
                    if (webResponse == null)
                        return 0;

                    using (var responseStream = webResponse.GetResponseStream())
                    {
                        if (responseStream != null)
                        {
                            var streamReader = new StreamReader(responseStream, Encoding.UTF8);
                            var responseString = streamReader.ReadToEnd();

                            var response = JsonConvert.DeserializeObject<MatchDayBlogResponse>(responseString);
                            if (response == null)
                                return 0;

                            var responseTime = DateTime.Now;
                            CheckIfRequestTakingTooLong(webRequest, requestTime, responseTime);

                            return response.ID;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                var key = GetType().FullName + ".GetMatchDayBlogId";
                await _logger.Error(key, exception, $"Error requesting data from: {webRequest.GetBaseUri()}.{Environment.NewLine}{key}{Environment.NewLine}{exception.Message}" +
                                                    $"{Environment.NewLine}{exception}" +
                                                    $"{Environment.NewLine}{exception.InnerException}");
            }

            return 0;
        }

        public async Task<int> GetMatchPreviewId(string sportName, int legacyFixtureId)
        {
            var webRequest = WebRequest.Create($"{_host}/{sportName}/match/preview/{legacyFixtureId}?format=json&auth={_authKey}");
            webRequest.Method = WebRequestMethods.Http.Get;
            var requestTime = DateTime.Now;
            try
            {
                using (var webResponse = await webRequest.GetResponseAsync(_requestTimeoutDurationInMilliseconds, _logger))
                {
                    if (webResponse == null)
                        return 0;

                    using (var responseStream = webResponse.GetResponseStream())
                    {
                        if (responseStream != null)
                        {
                            var streamReader = new StreamReader(responseStream, Encoding.UTF8);
                            var responseString = streamReader.ReadToEnd();

                            var response = JsonConvert.DeserializeObject<MatchPreviewResponse>(responseString);
                            if (response == null)
                                return 0;

                            var responseTime = DateTime.Now;
                            CheckIfRequestTakingTooLong(webRequest, requestTime, responseTime);

                            return response.ID;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                var key = GetType().FullName + ".GetMatchPreviewId";
                await _logger.Error(key, exception, $"Error requesting data from: {webRequest.GetBaseUri()}.{Environment.NewLine}{key}{Environment.NewLine}{exception.Message}" +
                                                    $"{Environment.NewLine}{exception}" +
                                                    $"{Environment.NewLine}{exception.InnerException}");
            }

            return 0;
        }

        public async Task<int> GetMatchReportId(string sportName, int legacyFixtureId)
        {
            var webRequest = WebRequest.Create($"{_host}/{sportName}/match/report/{legacyFixtureId}?format=json&auth={_authKey}");
            webRequest.Method = WebRequestMethods.Http.Get;
            var requestTime = DateTime.Now;

            try
            {
                using (var webResponse = await webRequest.GetResponseAsync(_requestTimeoutDurationInMilliseconds, _logger))
                {
                    if (webResponse == null)
                        return 0;

                    using (var responseStream = webResponse.GetResponseStream())
                    {
                        if (responseStream != null)
                        {
                            var streamReader = new StreamReader(responseStream, Encoding.UTF8);
                            var responseString = streamReader.ReadToEnd();

                            var response = JsonConvert.DeserializeObject<MatchReportResponse>(responseString);
                            if (response == null)
                                return 0;

                            var responseTime = DateTime.Now;
                            CheckIfRequestTakingTooLong(webRequest, requestTime, responseTime);

                            return response.ID;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                var key = GetType().FullName + ".GetMatchReportId";
                await _logger.Error(key, exception, $"Error requesting data from: {webRequest.GetBaseUri()}.{Environment.NewLine}{key}{Environment.NewLine}{exception.Message}" +
                                                    $"{Environment.NewLine}{exception}" +
                                                    $"{Environment.NewLine}{exception.InnerException}");
            }

            return 0;
        }

        private void CheckIfRequestTakingTooLong(WebRequest request, DateTime requestTime, DateTime responseTime)
        {
            try
            {
                if (request == null)
                    return;

                var durationMilliseconds = (responseTime - requestTime).TotalMilliseconds;

                if (durationMilliseconds > _requestDurationWarningMilliseconds)
                {
                    _logger.Warn(
                        $"HTTPRequestTooLong.{request.RequestUri}",
                        null,
                        $"HTTP request taking too long. {request.GetBaseUri()}. " +
                        $"Warning level is {_requestDurationWarningMilliseconds / 1000.0} seconds; " +
                        $"took {durationMilliseconds / 1000.0} seconds.");
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}
