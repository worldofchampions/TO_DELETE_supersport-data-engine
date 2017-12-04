using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Motor;

namespace SuperSportDataEngine.Gateway.Http.StatsProzone.Services
{
    public class StatsProzoneMotorIngestService : IStatsProzoneMotorIngestService
    {
        private const string StatsApiSharedSecret = "JDgQnhPVZQ";
        private const string StatsApiKey = "ta3dprpc4sn79ecm2wg7tqbg";

        public MotorEntitiesResponse IngestTournamentDrivers(string tournamentName)
        {
            var request = GetWebRequestForDriverIngest();

            MotorEntitiesResponse entitiesResponse;

            using (var response = request.GetResponse())
            {
                using (var responseStream = response.GetResponseStream())
                {
                    if (responseStream == null) return null;

                    var reader = new StreamReader(responseStream, Encoding.UTF8);

                    entitiesResponse = JsonConvert.DeserializeObject<MotorEntitiesResponse>(reader.ReadToEnd());
                }
            }

            return entitiesResponse;
        }

        private static MotorEntitiesResponse IngestStandings(string standingsTypeId)
        {
            var request = GetWebRequestForStandingsIngest(standingsTypeId);

            MotorEntitiesResponse entitiesResponse;

            using (var response = request.GetResponse())
            {
                using (var responseStream = response.GetResponseStream())
                {
                    if (responseStream == null) return null;

                    var reader = new StreamReader(responseStream, Encoding.UTF8);

                    entitiesResponse = JsonConvert.DeserializeObject<MotorEntitiesResponse>(reader.ReadToEnd());
                }
            }

            return entitiesResponse;
        }

        public MotorEntitiesResponse IngestDriverStandings(string tournamentName)
        {
            const string driverStandingsTypeId = "1"; //TODO Move to STATS constants

            var entitiesResponse = IngestStandings(driverStandingsTypeId);

            return entitiesResponse;
        }

        public MotorEntitiesResponse IngestTeamStandings(string tournamentName)
        {
            const string teamStandingsTypeId = "2"; //TODO Move to STATS constants

            var entitiesResponse = IngestStandings(teamStandingsTypeId);

            return entitiesResponse;
        }

        private static WebRequest GetWebRequestForDriverIngest(string tournament = "f1")
        {
            var urlBase = GetBaseUrl();

            var urlAppend = $"/v1/stats/motor/{tournament}/participants/?";

            var sig = GetRequestSignature();

            var sigAppend = "&sig=" + sig;

            const string apiKeyAppend = "api_key=" + StatsApiKey;

            var url = urlBase + urlAppend + apiKeyAppend + sigAppend;

            var request = WebRequest.Create(url);

            request.Method = "GET";

            return request;
        }

        private static WebRequest GetWebRequestForStandingsIngest(string standingsTypeId, string tournament = "f1")
        {
            var sig = GetRequestSignature();

            var statsMotorF1Standings = $"/v1/stats/motor/{tournament}/standings";

            var qryStrAppend = $"/?standingsTypeId={standingsTypeId}&languageId=1&api_key={StatsApiKey}&sig={sig}";

            var urlBase = GetBaseUrl();

            var url = urlBase + statsMotorF1Standings + qryStrAppend;

            var request = WebRequest.Create(url);

            request.Method = "GET";

            return request;
        }

        private static string GetRequestSignature()
        {
            var timestamp = GetCurrentTimestamp();

            var signature = CreateSha256Hash(StatsApiKey + StatsApiSharedSecret + timestamp);

            return signature;
        }

        private static string CreateSha256Hash(string input)
        {
            var sha256 = SHA256.Create();

            var inputBytes = Encoding.ASCII.GetBytes(input);

            var hashBytes = sha256.ComputeHash(inputBytes);

            var sha256Hash = ConvertToHexText(hashBytes);

            return sha256Hash;
        }

        private static string ConvertToHexText(IEnumerable<byte> hashBytes)
        {
            var sb = new StringBuilder();

            foreach (var bt in hashBytes)
            {
                sb.Append(bt.ToString("X2"));
            }

            return sb.ToString();
        }

        private static string GetBaseUrl()
        {
            return "http://api.stats.com";
        }

        private static string GetCurrentTimestamp()
        {
            var stamp = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds.ToString(CultureInfo.InvariantCulture);

            if (stamp.Contains("."))
            {
                stamp = stamp.Substring(0, stamp.IndexOf(".", StringComparison.Ordinal));
            }

            if (stamp.Contains(","))
            {
                stamp = stamp.Substring(0, stamp.IndexOf(",", StringComparison.Ordinal));
            }

            return stamp;
        }
    }
}