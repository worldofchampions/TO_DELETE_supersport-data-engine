namespace SuperSportDataEngine.Gateway.Http.StatsProzone.Services
{
    using System.IO;
    using System.Net;
    using System.Text;
    using System;
    using Newtonsoft.Json;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.ResponseModels;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Interfaces;
    using System.Threading;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyFixtures;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyEntities;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models;
    using System.Threading.Tasks;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyFlatLogs;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyGroupedLogs;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyMatchStats;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyEventsFlow;
    using System.Diagnostics;
    using SuperSportDataEngine.Common.Logging;

    public class StatsProzoneRugbyIngestService : IStatsProzoneRugbyIngestService
    {
        ILoggingService _logger;

        public StatsProzoneRugbyIngestService(
            ILoggingService logger)
        {
            _logger = logger;
        }

        public RugbyEntitiesResponse IngestRugbyReferenceData(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return null;

            try
            {
                return RugbyEntitiesResponse();
            }
            catch (Exception e)
            {
                _logger.Fatal(e);
                return new RugbyEntitiesResponse();
            }
        }

        private static RugbyEntitiesResponse RugbyEntitiesResponse()
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

            using (WebResponse response = request.GetResponse())
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    entitiesResponse.Entities =
                        JsonConvert.DeserializeObject<RugbyEntities>(reader.ReadToEnd());

                    // Not to be confused with the DateTime.Now call more above.
                    // This might be delayed due to provider being slow to process request,
                    // and is intended.
                    entitiesResponse.ResponseTime = DateTime.Now;
                    return entitiesResponse;
                }
            }
        }

        public RugbyFixturesResponse IngestFixturesForTournament(RugbyTournament tournament, int seasonId, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return null;

            try
            {
                return RugbyFixturesResponse(tournament, seasonId);
            }
            catch (Exception e)
            {
                _logger.Fatal(e);   
                return new RugbyFixturesResponse();
            }
        }

        private static RugbyFixturesResponse RugbyFixturesResponse(RugbyTournament tournament, int seasonId)
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

            using (WebResponse response = request.GetResponse())
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    fixturesResponse.Fixtures =
                        JsonConvert.DeserializeObject<RugbyFixtures>(reader.ReadToEnd());

                    // Not to be confused with the DateTime.Now call more above.
                    // This might be delayed due to provider being slow to process request,
                    // and is intended.
                    fixturesResponse.ResponseTime = DateTime.Now;
                    return fixturesResponse;
                }
            }
        }

        public RugbySeasonResponse IngestSeasonData(CancellationToken cancellationToken, int tournamentId, int tournamentYear)
        {
            if (cancellationToken.IsCancellationRequested)
                return null;

            try
            {
                return RugbySeasonResponse(tournamentId, tournamentYear);
            }
            catch (Exception e)
            {
                _logger.Fatal(e);
                return new RugbySeasonResponse();
            }
        }

        private static RugbySeasonResponse RugbySeasonResponse(int tournamentId, int tournamentYear)
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

            using (WebResponse response = request.GetResponse())
            {
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
                    return seasonsResponse;
                }
            }
        }

        public RugbyFixturesResponse IngestFixturesForTournamentSeason(int tournamentId, int seasonId, CancellationToken cancellationToken)
        {
            try
            {
                return ResponseData(tournamentId, seasonId);
            }
            catch (Exception e)
            {
                _logger.Fatal(e);
                return new RugbyFixturesResponse();
            }
        }

        private static RugbyFixturesResponse ResponseData(int tournamentId, int seasonId)
        {
            WebRequest request = GetWebRequestForFixturesEndpoint(tournamentId, seasonId, null);

            var responseData = new RugbyFixturesResponse() {RequestTime = DateTime.Now};

            using (WebResponse response = request.GetResponse())
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    var reader = new StreamReader(responseStream, Encoding.UTF8);

                    string responseDataFromServer = reader.ReadToEnd();

                    responseData.Fixtures =
                        JsonConvert.DeserializeObject<RugbyFixtures>(responseDataFromServer);

                    responseData.ResponseTime = DateTime.Now;

                    return responseData;
                }
            }
        }

        public RugbyFlatLogsResponse IngestFlatLogsForTournament(int competitionId, int seasonId)
        {
            try
            {
                return RugbyFlatLogsResponse(competitionId, seasonId);
            }
            catch (Exception e)
            {
                _logger.Fatal(e);
                return new RugbyFlatLogsResponse();
            }
        }

        private static RugbyFlatLogsResponse RugbyFlatLogsResponse(int competitionId, int seasonId)
        {
            WebRequest request = GetWebRequestForLogsEndpoint(competitionId, seasonId);

            var logsResponse = new RugbyFlatLogsResponse() {RequestTime = DateTime.Now};

            using (WebResponse response = request.GetResponse())
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    string s = reader.ReadToEnd();
                    logsResponse.RugbyFlatLogs =
                        JsonConvert.DeserializeObject<RugbyFlatLogs>(s);

                    logsResponse.ResponseTime = DateTime.Now;

                    return logsResponse;
                }
            }
        }

        public RugbyGroupedLogsResponse IngestGroupedLogsForTournament(int competitionId, int seasonId)
        {
            WebRequest request = GetWebRequestForLogsEndpoint(competitionId, seasonId);

            var logsResponse = new RugbyGroupedLogsResponse() { RequestTime = DateTime.Now };

            using (WebResponse response = request.GetResponse())
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    string s = reader.ReadToEnd();
                    logsResponse.RugbyGroupedLogs =
                        JsonConvert.DeserializeObject<RugbyGroupedLogs>(s);

                    logsResponse.ResponseTime = DateTime.Now;

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
                _logger.Fatal(e);
                return new RugbyMatchStatsResponse();
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

            Stopwatch responseTime = Stopwatch.StartNew();
            using (WebResponse response = await request.GetResponseAsync())
            {
                responseTime.Stop();
                _logger.Info("Response time is " + responseTime.ElapsedMilliseconds + "ms.");
                using (Stream responseStream = response.GetResponseStream())
                {
                    Stopwatch mappingTime = Stopwatch.StartNew();
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    matchStatsResponse.RugbyMatchStats =
                        JsonConvert.DeserializeObject<RugbyMatchStats>(reader.ReadToEnd());

                    // Not to be confused with the DateTime.Now call more above.
                    // This might be delayed due to provider being slow to process request,
                    // and is intended.
                    matchStatsResponse.ResponseTime = DateTime.Now;
                    mappingTime.Stop();
                    _logger.Info("Mapping time is " + mappingTime.ElapsedMilliseconds + "ms.");
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
                _logger.Fatal(e);
                return new RugbyEventsFlowResponse();
            }
        }

        private static async Task<RugbyEventsFlowResponse> EventsFlowResponse(long providerFixtureId)
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
    }
}
