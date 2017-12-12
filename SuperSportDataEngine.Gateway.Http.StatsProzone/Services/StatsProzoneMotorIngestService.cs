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
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RequestModels;

namespace SuperSportDataEngine.Gateway.Http.StatsProzone.Services
{
    public class StatsProzoneMotorIngestService : IStatsProzoneMotorIngestService
    {
        private const string StatsApiSharedSecret = "JDgQnhPVZQ";

        private const string StatsApiKey = "ta3dprpc4sn79ecm2wg7tqbg";

        private const string TeamStandingsTypeId = "2"; //TODO Move to STATS constants

        private const string DriverStandingsTypeId = "1"; //TODO Move to STATS constants

        private const string StatsApiBaseUrl = "http://api.stats.com";

        public MotorEntitiesResponse IngestTournaments()
        {
            var webRequestForTournamentsIngest = GetWebRequestForTournamentsIngest();

            MotorEntitiesResponse tournamentsEntitiesResponse;

            using (var webResponse = webRequestForTournamentsIngest.GetResponse())
            {
                using (var responseStream = webResponse.GetResponseStream())
                {
                    if (responseStream == null) return null;

                    var streamReader = new StreamReader(responseStream, Encoding.UTF8);

                    tournamentsEntitiesResponse = JsonConvert.DeserializeObject<MotorEntitiesResponse>(streamReader.ReadToEnd());
                }
            }

            return tournamentsEntitiesResponse;
        }

        public MotorEntitiesResponse IngestTournamentRaces(string providerSlug)
        {
            var statsWebRequest = GetWebRequestForRacesIngest(providerSlug);

            MotorEntitiesResponse tournamentRacesEntitiesResponse;

            using (var statsWebResponse = statsWebRequest.GetResponse())
            {
                using (var responseStream = statsWebResponse.GetResponseStream())
                {
                    if (responseStream == null) return null;

                    var streamReader = new StreamReader(responseStream, Encoding.UTF8);

                    tournamentRacesEntitiesResponse = JsonConvert.DeserializeObject<MotorEntitiesResponse>(streamReader.ReadToEnd());
                }
            }

            return tournamentRacesEntitiesResponse;
        }

        public MotorEntitiesResponse IngestTournamentSchedule(string providerSlug, int providerSeasonId)
        {
            var requestForTournamentSchedule = GetWebRequestForTournamentSchedule(providerSlug, providerSeasonId);

            MotorEntitiesResponse tournamentSchedule;

            using (var webResponse = requestForTournamentSchedule.GetResponse())
            {
                using (var responseStream = webResponse.GetResponseStream())
                {
                    if (responseStream is null) return null;

                    var streamReader = new StreamReader(responseStream, Encoding.UTF8);

                    tournamentSchedule = JsonConvert.DeserializeObject<MotorEntitiesResponse>(streamReader.ReadToEnd());
                }
            }

            return tournamentSchedule;
        }

        public MotorEntitiesResponse IngestTournamentResults(MotorResultRequestEntity motorResultRequestEntity)
        {
            var raceResultsRequest = GetWebRequestForRaceResultsIngest(motorResultRequestEntity.ProviderSlug, motorResultRequestEntity.ProviderSeasonId, motorResultRequestEntity.ProviderRaceId);

            MotorEntitiesResponse raceResultsResponse;

            using (var webResponse = raceResultsRequest.GetResponse())
            {
                using (var responseStream = webResponse.GetResponseStream())
                {
                    if (responseStream is null) return null;

                    var streamReader = new StreamReader(responseStream, Encoding.UTF8);

                    raceResultsResponse = JsonConvert.DeserializeObject<MotorEntitiesResponse>(streamReader.ReadToEnd());
                }
            }

            return raceResultsResponse;
        }

        public MotorEntitiesResponse IngestTournamentGrid(MotorResultRequestEntity motorResultRequestEntity)
        {
            return IngestTournamentResults(motorResultRequestEntity);
        }

        public MotorEntitiesResponse IngestTournamentDrivers(string providerSlug)
        {
            var webRequestForDriverIngest = GetWebRequestForDriversIngest(providerSlug);

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

        public MotorEntitiesResponse IngestTournamentTeams(string providerSlug)
        {
            var webRequestForTeamIngestIngest = GetWebRequestForTeamsIngest(providerSlug);

            MotorEntitiesResponse tournamentTeamsResponse;

            using (var webResponse = webRequestForTeamIngestIngest.GetResponse())
            {
                using (var responseStream = webResponse.GetResponseStream())
                {
                    if (responseStream is null) return null;

                    var streamReader = new StreamReader(responseStream, Encoding.UTF8);

                    tournamentTeamsResponse = JsonConvert.DeserializeObject<MotorEntitiesResponse>(streamReader.ReadToEnd());
                }
            }

            return tournamentTeamsResponse;
        }

        public MotorEntitiesResponse IngestDriverStandings(string providerSlug)
        {
            var driverStandings = IngestStandings(providerSlug, DriverStandingsTypeId);

            return driverStandings;
        }

        public MotorEntitiesResponse IngestTeamStandings(string providerSlug)
        {
            var teamStandings = IngestStandings(providerSlug, TeamStandingsTypeId);

            return teamStandings;
        }

        private static MotorEntitiesResponse IngestStandings(string providerSlug, string standingsTypeId)
        {
            var webRequestForStandingsIngest = GetWebRequestForStandingsIngest(providerSlug, standingsTypeId);

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

        private static WebRequest GetWebRequestForDriversIngest(string providerSlug)
        {
            var driversUrl = $"/v1/stats/motor/{providerSlug}/participants/";

            var requestSignature = GetRequestSignature();

            var queryString = $"?api_key={StatsApiKey} & sig= {requestSignature}";

            var requestUriString = StatsApiBaseUrl + driversUrl + queryString;

            var webRequestForDriverIngest = WebRequest.Create(requestUriString);

            webRequestForDriverIngest.Method = "GET";

            return webRequestForDriverIngest;
        }

        private static WebRequest GetWebRequestForTeamsIngest(string providerSlug)
        {
            var teamsUrl = $"/v1/stats/motor/{providerSlug}/teams/";

            var requestSignature = GetRequestSignature();

            var queryString = $"?api_key={StatsApiKey}&sig={requestSignature}";

            var requestUriString = StatsApiBaseUrl + teamsUrl + queryString;

            var requestForTeamIngest = WebRequest.Create(requestUriString);

            requestForTeamIngest.Method = "GET";

            return requestForTeamIngest;
        }

        private static WebRequest GetWebRequestForStandingsIngest(string standingsTypeId, string providerSlug)
        {
            var standingsUrl = $"/v1/stats/motor/{providerSlug}/standings/";

            var requestSignature = GetRequestSignature();

            var queryString = $"?standingsTypeId={standingsTypeId}&languageId=1&api_key={StatsApiKey}&sig={requestSignature}";

            var requestUriString = StatsApiBaseUrl + standingsUrl + queryString;

            var webRequestForStandingsIngest = WebRequest.Create(requestUriString);

            webRequestForStandingsIngest.Method = "GET";

            return webRequestForStandingsIngest;
        }

        private static WebRequest GetWebRequestForTournamentsIngest()
        {
            const string statsMotorLeaguesUrl = "/v1/stats/motor/leagues/";

            var requestSignature = GetRequestSignature();

            var queryString = $"?api_key={StatsApiKey}&sig={requestSignature}";

            var requestUriString = StatsApiBaseUrl + statsMotorLeaguesUrl + queryString;

            var requestForTournamentsIngest = WebRequest.Create(requestUriString);

            requestForTournamentsIngest.Method = "GET";

            return requestForTournamentsIngest;
        }

        private static WebRequest GetWebRequestForRacesIngest(string providerSlug)
        {
            var racesUrl = $"/v1/decode/motor/{providerSlug}/races/";

            var requestSignature = GetRequestSignature();

            var queryString = $"?api_key={StatsApiKey}&sig={requestSignature}";

            var requestUriString = StatsApiBaseUrl + racesUrl + queryString;

            var requestForTeamIngest = WebRequest.Create(requestUriString);

            requestForTeamIngest.Method = "GET";

            return requestForTeamIngest;
        }

        private static WebRequest GetWebRequestForTournamentSchedule(string providerSlug, int providerSeasonId)
        {
            var eventsUrl = $"/v1/stats/motor/{providerSlug}/events/";

            var requestSignature = GetRequestSignature();

            var queryString = $"?season={providerSeasonId}&api_key={StatsApiKey}&sig={requestSignature}";

            var requestUriString = StatsApiBaseUrl + eventsUrl + queryString;

            var requestForTournamentSchedule = WebRequest.Create(requestUriString);

            requestForTournamentSchedule.Method = "GET";

            return requestForTournamentSchedule;
        }

        private static WebRequest GetWebRequestForRaceResultsIngest(string providerSlug, int providerSeasonId, int raceId)
        {
            var raceResultsUrl = $"/v1/stats/motor/{providerSlug}/events/";

            var requestSignature = GetRequestSignature();

            var queryString = $"?box=true&season={providerSeasonId}&raceId={raceId}&api_key={StatsApiKey}&sig={requestSignature}";

            var requestUriString = StatsApiBaseUrl + raceResultsUrl + queryString;

            var requestForTournamentSchedule = WebRequest.Create(requestUriString);

            requestForTournamentSchedule.Method = "GET";

            return requestForTournamentSchedule;
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