namespace SuperSportDataEngine.Gateway.Http.StatsProzone.Services
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Net;
    using System.Security.Cryptography;
    using System.Text;

    public class StatsProzoneMotorWebRequest : IProviderWebRequest
    {
        private readonly string _statsApiSharedSecret;// = "JDgQnhPVZQ";
        private readonly string _statsApiKey;// = "ta3dprpc4sn79ecm2wg7tqbg";
        private readonly string _statsApiBaseUrl; //= "http://api.stats.com";

        public StatsProzoneMotorWebRequest(string statsApiBaseUrl, string statsApiKey, string statsApiSharedSecret)
        {
            _statsApiSharedSecret = statsApiSharedSecret;
            _statsApiKey = statsApiKey;
            _statsApiBaseUrl = statsApiBaseUrl;
        }

        public WebRequest GetRequestForDrivers(string providerSlug, int? seasonId = null)
        {
            var driversUrl = $"/v1/stats/motor/{providerSlug}/participants/";

            var requestSignature = GetRequestSignature();

            var queryString = $"?api_key={_statsApiKey}&sig={requestSignature}";

            var requestUriString = _statsApiBaseUrl + driversUrl + queryString;

            var webRequestForDriver = WebRequest.Create(requestUriString);

            webRequestForDriver.Method = "GET";

            return webRequestForDriver;
        }

        public WebRequest GetRequestForTeams(string providerSlug)
        {
            // Using the owners endpoint here is deliberate. 
            // The teams endpoint does not work!
            // Contact provider before changing this approach.
            var teamsUrl = $"/v1/stats/motor/{providerSlug}/owners/";

            var requestSignature = GetRequestSignature();

            var queryString = $"?api_key={_statsApiKey}&sig={requestSignature}";

            var requestUriString = _statsApiBaseUrl + teamsUrl + queryString;

            var requestForTeam = WebRequest.Create(requestUriString);

            requestForTeam.Method = "GET";

            return requestForTeam;
        }

        public WebRequest GetRequestForStandings(string providerSlug, string standingsTypeId, int providerSeasonId)
        {
            var standingsUrl = $"/v1/stats/motor/{providerSlug}/standings/";

            var requestSignature = GetRequestSignature();

            var queryString = $"?season={providerSeasonId}&standingsTypeId={standingsTypeId}&languageId=1&api_key={_statsApiKey}&sig={requestSignature}";

            var requestUriString = _statsApiBaseUrl + standingsUrl + queryString;

            var webRequestForStandings = WebRequest.Create(requestUriString);

            webRequestForStandings.Method = "GET";

            return webRequestForStandings;
        }

        public WebRequest GetRequestForTournaments()
        {
            const string statsMotorLeaguesUrl = "/v1/stats/motor/leagues/";

            var requestSignature = GetRequestSignature();

            var queryString = $"?accept=json&api_key={_statsApiKey}&sig={requestSignature}";

            var requestUriString = _statsApiBaseUrl + statsMotorLeaguesUrl + queryString;

            var requestForTournaments = WebRequest.Create(requestUriString);

            requestForTournaments.Method = "GET";

            return requestForTournaments;
        }

        public WebRequest GetRequestForRaces(string providerSlug, int providerSeasonId)
        {
            var racesUrl = $"/v1/decode/motor/{providerSlug}/races/";

            var requestSignature = GetRequestSignature();

            var queryString = $"?season={providerSeasonId}&api_key={_statsApiKey}&sig={requestSignature}";

            var requestUriString = _statsApiBaseUrl + racesUrl + queryString;

            var requestForTeam = WebRequest.Create(requestUriString);

            requestForTeam.Method = "GET";

            return requestForTeam;
        }

        public WebRequest GetRequestForSchedule(string providerSlug, int providerSeasonId, int providerRaceId)
        {
            var eventsUrl = $"/v1/stats/motor/{providerSlug}/events/";

            var requestSignature = GetRequestSignature();

            var queryString = $"?season={providerSeasonId}&raceId={providerRaceId}&api_key={_statsApiKey}&sig={requestSignature}";

            var requestUriString = _statsApiBaseUrl + eventsUrl + queryString;

            var requestForTournamentSchedule = WebRequest.Create(requestUriString);

            requestForTournamentSchedule.Method = "GET";

            return requestForTournamentSchedule;
        }

        public WebRequest GetRequestForRaceResults(string providerSlug, int providerSeasonId, int providerRaceId)
        {
            var raceResultsUrl = $"/v1/stats/motor/{providerSlug}/events/";

            var requestSignature = GetRequestSignature();

            var queryString =
                $"?box=true&season={providerSeasonId}&raceId={providerRaceId}&api_key={_statsApiKey}&sig={requestSignature}";

            var requestUriString = _statsApiBaseUrl + raceResultsUrl + queryString;

            var requestForTournamentSchedule = WebRequest.Create(requestUriString);

            requestForTournamentSchedule.Method = "GET";

            return requestForTournamentSchedule;
        }

        public WebRequest GetRequestForLeagueSeasons(string providerSlug)
        {
            var seasonsUrl = $"/v1/decode/motor/{providerSlug}/seasonStructure/";

            var requestSignature = GetRequestSignature();

            var queryString = $"?api_key={_statsApiKey}&sig={requestSignature}";

            var requestUriString = _statsApiBaseUrl + seasonsUrl + queryString;

            var requestForTeam = WebRequest.Create(requestUriString);

            requestForTeam.Method = "GET";

            return requestForTeam;
        }

        public WebRequest GetRequestForOwners(string providerSlug)
        {

            var ownersUrl = $"/v1/stats/motor/{providerSlug}/owners/";

            var requestSignature = GetRequestSignature();

            var queryString = $"?api_key={_statsApiKey}&sig={requestSignature}";

            var requestUriString = _statsApiBaseUrl + ownersUrl + queryString;

            var requestForOwners = WebRequest.Create(requestUriString);

            requestForOwners.Method = "GET";

            return requestForOwners;
        }

        private string GetRequestSignature()
        {
            var timestamp = GetCurrentTimestamp();

            var signature = CreateSha256Hash(_statsApiKey + _statsApiSharedSecret + timestamp);

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