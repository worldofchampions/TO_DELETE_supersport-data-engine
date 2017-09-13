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
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyFixtures;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyLogs;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RugbyEntities;
    using System.Threading.Tasks;

    public class StatsProzoneRugbyIngestService : IStatsProzoneRugbyIngestService
    {
        public RugbyEntitiesResponse IngestRugbyReferenceData(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return null;

            WebRequest request = WebRequest.Create("http://rugbyunion-api.stats.com/api/ru/configuration/entities");
            request.Method = "GET";

            request.Headers["Authorization"] = "Basic c3VwZXJzcG9ydDpvYTNuZzcrMjlmMw==";
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

        public RugbyFixturesResponse IngestFixturesForTournament(SportTournament tournament, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return null;

            var tournamentId = tournament.TournamentIndex;
            var tournamentYear = 2017;

            WebRequest request =
                WebRequest.Create("http://rugbyunion-api.stats.com/api/ru/competitions/fixtures/" + tournamentId + "/" + tournamentYear);

            request.Method = "GET";

            request.Headers["Authorization"] = "Basic c3VwZXJzcG9ydDpvYTNuZzcrMjlmMw==";
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

        public RugbyLogsResponse IngestLogsForTournament(SportTournament tournament, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return null;

            var tournamentId = tournament.TournamentIndex;
            var tournamentYear = 2017;

            WebRequest request =
                WebRequest.Create("http://rugbyunion-api.stats.com/api/ru/competitions/ladder/" + tournamentId + "/" + tournamentYear);

            request.Method = "GET";

            request.Headers["Authorization"] = "Basic c3VwZXJzcG9ydDpvYTNuZzcrMjlmMw==";
            request.ContentType = "application/json; charset=UTF-8";

            var logsResponse =
                new RugbyLogsResponse()
                {
                    RequestTime = DateTime.Now
                };

            using (WebResponse response = request.GetResponse())
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    logsResponse.RugbyLogs =
                        JsonConvert.DeserializeObject<RugbyLogs>(reader.ReadToEnd());

                    // Not to be confused with the DateTime.Now call more above.
                    // This might be delayed due to provider being slow to process request,
                    // and is intended.
                    logsResponse.ResponseTime = DateTime.Now;
                    return logsResponse;
                }
            }
        }

        public async Task<RugbyFixturesResponse> IngestFixtureResults(int competionId, int seasonId, int roundId)
        {
            WebRequest request = GetWebRequestForFixturesEndpoint(competionId, seasonId, roundId);

            var responseData = new RugbyFixturesResponse() { RequestTime = DateTime.Now };

            using (WebResponse response = await request.GetResponseAsync())
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    var reader = new StreamReader(responseStream, Encoding.UTF8);

                    string responseDataFromServer = reader.ReadToEnd();

                    //TODO: Return Response in a Correct DataModel

                    var responseDataToReturn = JsonConvert.DeserializeObject<RoundFixtureResultsResponse>(responseDataFromServer);

                    responseData.ResponseTime = DateTime.Now;

                    return responseData;
                }
            }
        }

        private static WebRequest GetWebRequestForFixturesEndpoint(int competionId, int seasonId, int roundId)
        {
            var baseUrl = "http://rugbyunion-api.stats.com/api/ru/competitions/fixtures/";

            var request = WebRequest.Create(baseUrl + competionId + "/" + seasonId + "/" + roundId);

            request.Method = "GET";

            request.Headers["Authorization"] = "Basic c3VwZXJzcG9ydDpvYTNuZzcrMjlmMw==";

            request.ContentType = "application/json; charset=UTF-8";

            return request;
        }
    }
}
