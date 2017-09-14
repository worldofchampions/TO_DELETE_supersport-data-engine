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
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models;

    public class StatsProzoneRugbyIngestService : IStatsProzoneRugbyIngestService
    {
        public RugbyEntitiesResponse IngestRugbyReferenceData(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return null;

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

        public RugbyFixturesResponse IngestFixturesForTournament(RugbyTournament tournament, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return null;

            var tournamentId = tournament.ProviderTournamentId;
            // TODO: This will need to be replaced with the season identifier.
            var tournamentYear = 2017;

            WebRequest request = 
                WebRequest.Create("http://rugbyunion-api.stats.com/api/ru/competitions/fixtures/" + tournamentId + "/" + tournamentYear);

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

        public RugbyLogsResponse IngestLogsForTournament(RugbyTournament tournament, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return null;

            var tournamentId = tournament.ProviderTournamentId;
            var tournamentYear = 2017;

            WebRequest request =
                WebRequest.Create("http://rugbyunion-api.stats.com/api/ru/competitions/ladder/" + tournamentId + "/" + tournamentYear);

            request.Method = "GET";

            request.Headers["Authorization"] = "Basic U3VwZXJTcG9ydF9NZWRpYTpTdTkzUjdyMFA1";
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

        public RugbySeasonResponse IngestSeasonData(CancellationToken cancellationToken, int tournamentId, int tournamentYear)
        {
            if (cancellationToken.IsCancellationRequested)
                return null;

            WebRequest request = WebRequest.Create("http://rugbyunion-api.stats.com/api/ru/competitions/seasons/" + tournamentId + "/" + tournamentYear);

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

        public object IngestFixturesForTournamentSeason(int tournamentId, int seasonId, CancellationToken cancellationToken)
        {
            // Make provider call to http://rugbyunion-api.stats.com/api/ru/competitions/fixtures/tournamentId/seasonId
            // Maybe even specify the round number?

            return new object();
        }
    }
}
