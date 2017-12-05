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

        private const string TeamStandingsTypeId = "2"; //TODO Move to STATS constants

        private const string DriverStandingsTypeId = "1"; //TODO Move to STATS constants

        private const string BaseUrl = "http://api.stats.com";

        public MotorEntitiesResponse IngestTournamentDrivers(string providerTournamentSlug)
        {
            var webRequestForDriverIngest = GetWebRequestForDriverIngest(providerTournamentSlug);

            MotorEntitiesResponse tournamentDrivers;

            using (var webResponse = webRequestForDriverIngest.GetResponse())
            {
                using (var responseStream = webResponse.GetResponseStream())
                {
                    if (responseStream is null) return null;

                    var streamReader = new StreamReader(responseStream, Encoding.UTF8);

                    tournamentDrivers = JsonConvert.DeserializeObject<MotorEntitiesResponse>(streamReader.ReadToEnd());
                }
            }

            return tournamentDrivers;
        }

        public MotorEntitiesResponse IngestDriverStandings(string providerTournamentSlug)
        {
            var driverStandings = IngestStandings(providerTournamentSlug, DriverStandingsTypeId);

            return driverStandings;
        }

        public MotorEntitiesResponse IngestTeamStandings(string providerTournamentSlug)
        {
            var teamStandings = IngestStandings(providerTournamentSlug, TeamStandingsTypeId);

            return teamStandings;
        }

        private static MotorEntitiesResponse IngestStandings(string providerTournamentSlug, string standingsTypeId)
        {
            var webRequestForStandingsIngest = GetWebRequestForStandingsIngest(providerTournamentSlug, standingsTypeId);

            MotorEntitiesResponse standings;

            using (var webResponse = webRequestForStandingsIngest.GetResponse())
            {
                using (var responseStream = webResponse.GetResponseStream())
                {
                    if (responseStream == null) return null;

                    var streamReader = new StreamReader(responseStream, Encoding.UTF8);

                    standings = JsonConvert.DeserializeObject<MotorEntitiesResponse>(streamReader.ReadToEnd());
                }
            }

            return standings;
        }

        private static WebRequest GetWebRequestForDriverIngest(string providerTournamentSlug)
        {
            var driversUrl = $"/v1/stats/motor/{providerTournamentSlug}/participants/";

            var requestSignature = GetRequestSignature();

            var queryString = $"?api_key={StatsApiKey} & sig= {requestSignature}";

            var requestUriString = BaseUrl + driversUrl + queryString;

            var webRequestForDriverIngest = WebRequest.Create(requestUriString);

            webRequestForDriverIngest.Method = "GET";

            return webRequestForDriverIngest;
        }

        private static WebRequest GetWebRequestForStandingsIngest(string standingsTypeId, string providerTournamentSlug)
        {
            var standingsUrl = $"/v1/stats/motor/{providerTournamentSlug}/standings/";

            var requestSignature = GetRequestSignature();

            var queryString = $"?standingsTypeId={standingsTypeId}&languageId=1&api_key={StatsApiKey}&sig={requestSignature}";

            var requestUriString = BaseUrl + standingsUrl + queryString;

            var webRequestForStandingsIngest = WebRequest.Create(requestUriString);

            webRequestForStandingsIngest.Method = "GET";

            return webRequestForStandingsIngest;
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