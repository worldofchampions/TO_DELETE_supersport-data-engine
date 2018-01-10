namespace SuperSportDataEngine.Gateway.Http.StatsProzone.Services
{
    using Newtonsoft.Json;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyEntities;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyEventsFlow;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyFixtures;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyFlatLogs;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyGroupedLogs;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyMatchStats;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.ResponseModels;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
    using SuperSportDataEngine.Common.Extentions;
    using SuperSportDataEngine.Common.Logging;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public class StatsProzoneRugbyIngestService : IStatsProzoneRugbyIngestService
    {
        private readonly ILoggingService _logger;
        private readonly int _maximumTimeForRequestWithResponseInMilliseconds;
        private readonly int _maximumTimeForResponseInMilliseconds;

        public StatsProzoneRugbyIngestService(
            ILoggingService logger)
        {
            _logger = logger;
            _maximumTimeForRequestWithResponseInMilliseconds = int.Parse(ConfigurationManager.AppSettings["maximumTimeForRequestWithResponseInMilliseconds"]);
            _maximumTimeForResponseInMilliseconds = int.Parse(ConfigurationManager.AppSettings["maximumTimeForResponseInMilliseconds"]);
        }

        public async Task<RugbyEntitiesResponse> IngestRugbyReferenceData(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return null;

            try
            {
                return await RugbyEntitiesResponse();
            }
            catch (Exception e)
            {
                await _logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name, e.StackTrace);
                return null;
            }
        }

        private async Task<RugbyEntitiesResponse> RugbyEntitiesResponse()
        {
            WebRequest request = WebRequest.Create("http://rugbyunion-api.stats.com/api/ru/configuration/entities");
            request.Method = "GET";

            request.Headers["Authorization"] = "Basic U3VwZXJTcG9ydF9NZWRpYTpTdTkzUjdyMFA1";
            request.ContentType = "application/json; charset=UTF-8";

            var entitiesResponse =
                new RugbyEntitiesResponse()
                {
                    RequestTime = DateTime.Now
                };

            using (WebResponse response = await request.GetResponseAsync(_maximumTimeForResponseInMilliseconds, _logger))
            {
                if (response == null)
                    return null;

                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    entitiesResponse.Entities =
                        JsonConvert.DeserializeObject<RugbyEntities>(reader.ReadToEnd());

                    // Not to be confused with the DateTime.Now call more above.
                    // This might be delayed due to provider being slow to process request,
                    // and is intended.
                    entitiesResponse.ResponseTime = DateTime.Now;
                    CheckIfRequestTakingTooLong(request, entitiesResponse);

                    return entitiesResponse;
                }
            }
        }

        public async Task<RugbyFixturesResponse> IngestFixturesForTournament(RugbyTournament tournament, int seasonId, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return null;

            try
            {
                return await RugbyFixturesResponse(tournament, seasonId);
            }
            catch (Exception e)
            {
                await _logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name, e.StackTrace);
                return null;
            }
        }

        private async Task<RugbyFixturesResponse> RugbyFixturesResponse(RugbyTournament tournament, int seasonId)
        {
            var tournamentId = tournament.ProviderTournamentId;

            WebRequest request =
                WebRequest.Create("http://rugbyunion-api.stats.com/api/ru/competitions/fixtures/" + tournamentId + "/" +
                                  seasonId);

            request.Method = "GET";

            request.Headers["Authorization"] = "Basic U3VwZXJTcG9ydF9NZWRpYTpTdTkzUjdyMFA1";
            request.ContentType = "application/json; charset=UTF-8";

            var fixturesResponse =
                new RugbyFixturesResponse()
                {
                    RequestTime = DateTime.Now
                };

            using (WebResponse response = await request.GetResponseAsync(_maximumTimeForResponseInMilliseconds, _logger))
            {
                if (response == null)
                    return null;

                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    fixturesResponse.Fixtures =
                        JsonConvert.DeserializeObject<RugbyFixtures>(reader.ReadToEnd());

                    // Not to be confused with the DateTime.Now call more above.
                    // This might be delayed due to provider being slow to process request,
                    // and is intended.
                    fixturesResponse.ResponseTime = DateTime.Now;
                    CheckIfRequestTakingTooLong(request, fixturesResponse);

                    return fixturesResponse;
                }
            }
        }

        public async Task<RugbySeasonResponse> IngestSeasonData(CancellationToken cancellationToken, int tournamentId, int tournamentYear)
        {
            if (cancellationToken.IsCancellationRequested)
                return null;

            try
            {
                return await RugbySeasonResponse(tournamentId, tournamentYear);
            }
            catch (Exception e)
            {
                await _logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name, e.StackTrace);
                return null;
            }
        }

        private async Task<RugbySeasonResponse> RugbySeasonResponse(int tournamentId, int tournamentYear)
        {
            WebRequest request = WebRequest.Create("http://rugbyunion-api.stats.com/api/ru/competitions/seasons/" +
                                                   tournamentId + "/" + tournamentYear);

            request.Method = "GET";

            request.Headers["Authorization"] = "Basic U3VwZXJTcG9ydF9NZWRpYTpTdTkzUjdyMFA1";
            request.ContentType = "application/json; charset=UTF-8";

            var seasonsResponse =
                new RugbySeasonResponse()
                {
                    RequestTime = DateTime.Now
                };

            using (WebResponse response = await request.GetResponseAsync(_maximumTimeForResponseInMilliseconds, _logger))
            {
                if (response == null)
                    return null;

                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    string s = reader.ReadToEnd();
                    seasonsResponse.RugbySeasons =
                        JsonConvert.DeserializeObject<RugbySeasons>(s);

                    // Not to be confused with the DateTime.Now call more above.
                    // This might be delayed due to provider being slow to process request,
                    // and is intended.
                    seasonsResponse.ResponseTime = DateTime.Now;
                    CheckIfRequestTakingTooLong(request, seasonsResponse);

                    return seasonsResponse;
                }
            }
        }

        public async Task<RugbyFixturesResponse> IngestFixturesForTournamentSeason(int tournamentId, int seasonId, CancellationToken cancellationToken)
        {
            try
            {
                return await ResponseData(tournamentId, seasonId);
            }
            catch (Exception e)
            {
                await _logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name, e.StackTrace);
                return null;
            }
        }

        private async Task<RugbyFixturesResponse> ResponseData(int tournamentId, int seasonId)
        {
            WebRequest request = GetWebRequestForFixturesEndpoint(tournamentId, seasonId, null);

            var responseData = new RugbyFixturesResponse() { RequestTime = DateTime.Now };

            using (WebResponse response = await request.GetResponseAsync(_maximumTimeForResponseInMilliseconds, _logger))
            {
                if (response == null)
                    return null;

                using (Stream responseStream = response.GetResponseStream())
                {
                    var reader = new StreamReader(responseStream, Encoding.UTF8);

                    string responseDataFromServer = reader.ReadToEnd();

                    responseData.Fixtures =
                        JsonConvert.DeserializeObject<RugbyFixtures>(responseDataFromServer);

                    responseData.ResponseTime = DateTime.Now;
                    CheckIfRequestTakingTooLong(request, responseData);

                    return responseData;
                }
            }
        }

        public async Task<RugbyFlatLogsResponse> IngestFlatLogsForTournament(int competitionId, int seasonId)
        {
            try
            {
                return await RugbyFlatLogsResponse(competitionId, seasonId);
            }
            catch (Exception e)
            {
                await _logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name, e.StackTrace);
                return null;
            }
        }

        private async Task<RugbyFlatLogsResponse> RugbyFlatLogsResponse(int competitionId, int seasonId)
        {
            WebRequest request = GetWebRequestForLogsEndpoint(competitionId, seasonId, null);

            var logsResponse = new RugbyFlatLogsResponse() { RequestTime = DateTime.Now };

            using (WebResponse response = await request.GetResponseAsync(_maximumTimeForResponseInMilliseconds, _logger))
            {
                if (response == null)
                    return null;

                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    string s = reader.ReadToEnd();
                    logsResponse.RugbyFlatLogs =
                        JsonConvert.DeserializeObject<RugbyFlatLogs>(s);

                    logsResponse.ResponseTime = DateTime.Now;
                    CheckIfRequestTakingTooLong(request, logsResponse);

                    return logsResponse;
                }
            }
        }

        public async Task<RugbyGroupedLogsResponse> IngestGroupedLogsForTournament(int competitionId, int seasonId, int numberOfRounds)
        {
            // Ronald: Going to hack the response model returned to the ingest service for the Sevens tournament.
            // We need to return a list of ladders, such that each item (list of ladder positions), have a group id
            // set to the round number.
            // The reasoning behind this is that each round for Sevens is played in a different location,
            // With the same teams. So we group each log based on the round number.

            // This is the competition id for Sevens
            if (competitionId == 831)
            {
                return await GetSevensGroupedLogs(competitionId, seasonId, numberOfRounds);
            }

            WebRequest request = GetWebRequestForLogsEndpoint(competitionId, seasonId);

            var logsResponse = new RugbyGroupedLogsResponse() { RequestTime = DateTime.Now };

            using (WebResponse response = await request.GetResponseAsync(_maximumTimeForResponseInMilliseconds, _logger))
            {
                if (response == null)
                    return null;

                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    string s = reader.ReadToEnd();
                    logsResponse.RugbyGroupedLogs =
                        JsonConvert.DeserializeObject<RugbyGroupedLogs>(s);

                    logsResponse.ResponseTime = DateTime.Now;
                    CheckIfRequestTakingTooLong(request, logsResponse);

                    return logsResponse;
                }
            }
        }

        private async Task<RugbyGroupedLogsResponse> GetSevensGroupedLogs(int competitionId, int seasonId, int numberOfRounds)
        {
            WebRequest request = GetWebRequestForLogsEndpoint(competitionId, seasonId, numberOfRounds);

            var ladders = new List<List<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyGroupedLogs.Ladderposition>>();

            var response = await GetSevensLogsResponse(request);
            ladders.AddRange(response.RugbyGroupedLogs.ladders);

            while (numberOfRounds > 1)
            {
                numberOfRounds--;
                var req = GetWebRequestForLogsEndpoint(competitionId, seasonId, numberOfRounds);
                var res = await GetSevensLogsResponse(req);

                ladders.AddRange(res.RugbyGroupedLogs.ladders);
            }

            response.RugbyGroupedLogs.ladders = ladders;

            return response;
        }

        private async Task<RugbyGroupedLogsResponse> GetSevensLogsResponse(WebRequest request)
        {
            var ladders = new List<List<ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyGroupedLogs.Ladderposition>>();

            var logsResponse = new RugbyGroupedLogsResponse() { RequestTime = DateTime.Now };

            using (WebResponse response = await request.GetResponseAsync(_maximumTimeForResponseInMilliseconds, _logger))
            {
                if (response == null)
                    return null;

                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    string s = reader.ReadToEnd();
                    var groupedLogs = JsonConvert.DeserializeObject<RugbyGroupedLogs>(s);
                    groupedLogs.ladderposition.ForEach((l) =>
                    {
                        l.group = groupedLogs.roundNumber;
                    });

                    ladders.Add(
                        groupedLogs.ladderposition);

                    logsResponse.RugbyGroupedLogs = groupedLogs;
                    logsResponse.RugbyGroupedLogs.ladders = ladders;
                    logsResponse.ResponseTime = DateTime.Now;

                    CheckIfRequestTakingTooLong(request, logsResponse);

                    return logsResponse;
                }
            }
        }

        private static WebRequest GetWebRequestForFixturesEndpoint(int competionId, int seasonId, int? roundId)
        {
            var baseUrl = "http://rugbyunion-api.stats.com/api/ru/competitions/fixtures/";

            var request = WebRequest.Create(baseUrl + competionId + "/" + seasonId + "/" + roundId);

            request.Method = "GET";
            request.Headers["Authorization"] = "Basic U3VwZXJTcG9ydF9NZWRpYTpTdTkzUjdyMFA1";

            request.ContentType = "application/json; charset=UTF-8";

            return request;
        }

        public async Task<RugbyMatchStatsResponse> IngestMatchStatsForFixtureAsync(CancellationToken cancellationToken, long providerFixtureId)
        {
            if (cancellationToken.IsCancellationRequested)
                return null;

            try
            {
                return await MatchStatsResponse(providerFixtureId);
            }
            catch (Exception e)
            {
                await _logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name, e.StackTrace);
                return null;
            }
        }

        private async Task<RugbyMatchStatsResponse> MatchStatsResponse(long providerFixtureId)
        {
            WebRequest request =
                WebRequest.Create("http://rugbyunion-api.stats.com/api/ru/matchStats/" + providerFixtureId + "/");

            request.Method = "GET";

            request.Headers["Authorization"] = "Basic U3VwZXJTcG9ydF9NZWRpYTpTdTkzUjdyMFA1";
            request.ContentType = "application/json; charset=UTF-8";

            var matchStatsResponse =
                new RugbyMatchStatsResponse()
                {
                    RequestTime = DateTime.Now
                };

            using (WebResponse response = await request.GetResponseAsync())
            {
                if (response == null)
                    return null;

                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    matchStatsResponse.RugbyMatchStats =
                        JsonConvert.DeserializeObject<RugbyMatchStats>(reader.ReadToEnd());

                    // Not to be confused with the DateTime.Now call more above.
                    // This might be delayed due to provider being slow to process request,
                    // and is intended.
                    matchStatsResponse.ResponseTime = DateTime.Now;
                    CheckIfRequestTakingTooLong(request, matchStatsResponse);

                    return matchStatsResponse;
                }
            }
        }

        public async Task<RugbyEventsFlowResponse> IngestEventsFlow(CancellationToken cancellationToken, long providerFixtureId)
        {
            if (cancellationToken.IsCancellationRequested)
                return null;

            try
            {
                return await EventsFlowResponse(providerFixtureId);
            }
            catch (Exception e)
            {
                await _logger.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name, e.StackTrace);
                return null;
            }
        }

        private async Task<RugbyEventsFlowResponse> EventsFlowResponse(long providerFixtureId)
        {
            WebRequest request =
                WebRequest.Create("http://rugbyunion-api.stats.com/api/ru/matchStats/eventsflow/" + providerFixtureId + "/");

            request.Method = "GET";

            request.Headers["Authorization"] = "Basic U3VwZXJTcG9ydF9NZWRpYTpTdTkzUjdyMFA1";
            request.ContentType = "application/json; charset=UTF-8";

            var eventsFlowResponse =
                new RugbyEventsFlowResponse()
                {
                    RequestTime = DateTime.Now
                };

            using (WebResponse response = await request.GetResponseAsync())
            {
                if (response == null)
                    return null;

                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    string stats = reader.ReadToEnd();
                    eventsFlowResponse.RugbyEventsFlow =
                        JsonConvert.DeserializeObject<RugbyEventsFlow>(stats);

                    // Not to be confused with the DateTime.Now call more above.
                    // This might be delayed due to provider being slow to process request,
                    // and is intended.
                    eventsFlowResponse.ResponseTime = DateTime.Now;

                    CheckIfRequestTakingTooLong(request, eventsFlowResponse);

                    return eventsFlowResponse;
                }
            }
        }

        private static WebRequest GetWebRequestForLogsEndpoint(int competitionId, int seasonId)
        {
            var baseUrl = "http://rugbyunion-api.stats.com/api/RU/competitions/ladder/";

            var request = WebRequest.Create(baseUrl + competitionId + "/" + seasonId);

            request.Method = "GET";

            request.Headers["Authorization"] = "Basic U3VwZXJTcG9ydF9NZWRpYTpTdTkzUjdyMFA1";

            request.ContentType = "application/json; charset=UTF-8";

            return request;
        }

        private static WebRequest GetWebRequestForLogsEndpoint(int competitionId, int seasonId, int? numberOfRounds)
        {
            var baseUrl = "http://rugbyunion-api.stats.com/api/RU/competitions/ladder/";

            var request = WebRequest.Create(baseUrl + competitionId + "/" + seasonId + "/" + (numberOfRounds != null ? numberOfRounds.ToString() : ""));

            request.Method = "GET";

            request.Headers["Authorization"] = "Basic U3VwZXJTcG9ydF9NZWRpYTpTdTkzUjdyMFA1";

            request.ContentType = "application/json; charset=UTF-8";

            return request;
        }

        private void CheckIfRequestTakingTooLong(WebRequest request, dynamic o)
        {
            var timeDifference = (o.ResponseTime - o.RequestTime);
            var seconds = timeDifference.TotalSeconds;

            if (seconds > _maximumTimeForRequestWithResponseInMilliseconds)
            {
                _logger.Warn("HTTPRequestTooLong." + request.RequestUri,
                    "HTTP request taking too long. " + request.RequestUri + ". Taking " + seconds + " seconds.");
            }
        }
    }
}
