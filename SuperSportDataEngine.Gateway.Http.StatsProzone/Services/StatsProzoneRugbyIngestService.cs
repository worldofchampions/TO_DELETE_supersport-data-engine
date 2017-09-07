namespace SuperSportDataEngine.Gateway.Http.StatsProzone.Services
{
    using System.IO;
    using System.Net;
    using System.Text;
    using System;
    using Newtonsoft.Json;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.ResponseModels;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Interfaces;
    using System.Threading;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models;

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
    }
}
