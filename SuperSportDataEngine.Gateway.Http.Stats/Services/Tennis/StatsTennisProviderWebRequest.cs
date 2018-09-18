using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.Stats.Interfaces;

namespace SuperSportDataEngine.Gateway.Http.Stats.Services.Tennis
{
    public class StatsTennisProviderWebRequest : IStatsTennisProviderWebRequest
    {
        private readonly string _statsApiSharedSecret;
        private readonly string _statsApiKey;
        private readonly string _statsApiBaseUrl;
        private readonly string _cacheControlHeader;

        public StatsTennisProviderWebRequest(string statsApiBaseUrl, string statsApiKey, string statsApiSharedSecret)
        {
            _statsApiSharedSecret = statsApiSharedSecret;
            _statsApiKey = statsApiKey;
            _statsApiBaseUrl = statsApiBaseUrl;
            _cacheControlHeader = ConfigurationManager.AppSettings["StatsApiMotorsportCacheControlHeader"];
        }

        public WebRequest GetRequestForLeagues()
        {
            var tournamentsUrl = $"/v1/stats/tennis/leagues/";

            var requestSignature = GetRequestSignature();

            var queryString = $"?api_key={_statsApiKey}&sig={requestSignature}";

            var requestUriString = _statsApiBaseUrl + tournamentsUrl + queryString;

            var webRequestForLeagues = WebRequest.Create(requestUriString);

            webRequestForLeagues.Method = "GET";

            webRequestForLeagues.Headers.Add(HttpRequestHeader.CacheControl, _cacheControlHeader);

            return webRequestForLeagues;
        }

        public WebRequest GetWebRequestForTournamentDecode(string leagueName)
        {
            var tournamentsUrl = $"/v1/decode/tennis/{leagueName}/tournaments";

            var requestSignature = GetRequestSignature();

            var queryString = $"?api_key={_statsApiKey}&sig={requestSignature}";

            var requestUriString = _statsApiBaseUrl + tournamentsUrl + queryString;

            var webRequestForLeagues = WebRequest.Create(requestUriString);

            webRequestForLeagues.Method = "GET";

            webRequestForLeagues.Headers.Add(HttpRequestHeader.CacheControl, _cacheControlHeader);

            return webRequestForLeagues;
        }

        public WebRequest GetWebRequestForSeasonsDecode(string leagueProviderSlug)
        {
            var seasonsUrl = $"/v1/decode/tennis/{leagueProviderSlug}/seasonStructure";

            var requestSignature = GetRequestSignature();

            var queryString = $"?api_key={_statsApiKey}&sig={requestSignature}";

            var requestUriString = _statsApiBaseUrl + seasonsUrl + queryString;

            var webRequestForLeagues = WebRequest.Create(requestUriString);

            webRequestForLeagues.Method = "GET";

            webRequestForLeagues.Headers.Add(HttpRequestHeader.CacheControl, _cacheControlHeader);

            return webRequestForLeagues;
        }

        public WebRequest GetWebRequestForVenuesDecode(string leagueProviderSlug)
        {
            var venuesUrl = $"/v1/decode/tennis/{leagueProviderSlug}/venues";

            var requestSignature = GetRequestSignature();

            var queryString = $"?api_key={_statsApiKey}&sig={requestSignature}";

            var requestUriString = _statsApiBaseUrl + venuesUrl + queryString;

            var webRequestForVenues = WebRequest.Create(requestUriString);

            webRequestForVenues.Method = "GET";

            webRequestForVenues.Headers.Add(HttpRequestHeader.CacheControl, _cacheControlHeader);

            return webRequestForVenues;
        }

        public WebRequest GetWebRequestForSurfaceTypesDecode(string leagueProviderSlug)
        {
            var surfaceTypesUrl = $"/v1/decode/tennis/{leagueProviderSlug}/surfaceTypes";

            var requestSignature = GetRequestSignature();

            var queryString = $"?api_key={_statsApiKey}&sig={requestSignature}";

            var requestUriString = _statsApiBaseUrl + surfaceTypesUrl + queryString;

            var webRequestForSurfaceTypes = WebRequest.Create(requestUriString);

            webRequestForSurfaceTypes.Method = "GET";
            webRequestForSurfaceTypes.Headers.Add(HttpRequestHeader.CacheControl, _cacheControlHeader);

            return webRequestForSurfaceTypes;
        }

        public WebRequest GetWebRequestForParticipants(string leagueProviderSlug)
        {
            var participantsUrl = $"/v1/stats/tennis/{leagueProviderSlug}/participants/";

            var requestSignature = GetRequestSignature();

            var queryString = $"?api_key={_statsApiKey}&sig={requestSignature}";

            var requestUriString = _statsApiBaseUrl + participantsUrl + queryString;

            var webRequestForParticipants = WebRequest.Create(requestUriString);

            webRequestForParticipants.Method = "GET";
            webRequestForParticipants.Headers.Add(HttpRequestHeader.CacheControl, _cacheControlHeader);

            return webRequestForParticipants;
        }

        public WebRequest GetWebRequestForTournamentEvent(string leagueProviderSlug, int providerTournamentId, int seasonId)
        {
            var calendarUrl = $"/v1/stats/tennis/{leagueProviderSlug}/events/";

            var requestSignature = GetRequestSignature();

            var queryString = $"?season={seasonId}&tournamentId={providerTournamentId}&api_key={_statsApiKey}&sig={requestSignature}";

            var requestUriString = _statsApiBaseUrl + calendarUrl + queryString;

            var webRequestForLeagues = WebRequest.Create(requestUriString);

            webRequestForLeagues.Method = "GET";
            webRequestForLeagues.Headers.Add(HttpRequestHeader.CacheControl, _cacheControlHeader);

            return webRequestForLeagues;
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

        public WebRequest GetWebRequestForEvents(string leagueProviderSlug, int year)
        {
            var eventsUrl = $"/v1/stats/tennis/{leagueProviderSlug}/events/";

            var requestSignature = GetRequestSignature();

            var queryString = $"?season={year}&api_key={_statsApiKey}&sig={requestSignature}";

            var requestUriString = _statsApiBaseUrl + eventsUrl + queryString;

            var webRequestForEvents = WebRequest.Create(requestUriString);

            webRequestForEvents.Method = "GET";
            webRequestForEvents.Headers.Add(HttpRequestHeader.CacheControl, _cacheControlHeader);

            return webRequestForEvents;
        }

        public WebRequest GetWebRequestForRankings(string leagueProviderSlug, int rankingType, string providerRankingQuery, int year)
        {
            var rnakingsUrl = $"/v1/stats/tennis/{leagueProviderSlug}/leaders/";

            var requestSignature = GetRequestSignature();

            var queryString = $"?topCount={providerRankingQuery}&leaderCategoryId={rankingType}&season={year}&api_key={_statsApiKey}&sig={requestSignature}";

            var requestUriString = _statsApiBaseUrl + rnakingsUrl + queryString;

            var webRequestForRankings = WebRequest.Create(requestUriString);

            webRequestForRankings.Method = "GET";
            webRequestForRankings.Headers.Add(HttpRequestHeader.CacheControl, _cacheControlHeader);

            return webRequestForRankings;
        }

        public WebRequest GetWebRequestForResults(string leagueProviderSlug, int eventId, int providerSeasonId)
        {
            var resultsUrl = $"/v1/stats/tennis/{leagueProviderSlug}/events/{eventId}";

            var requestSignature = GetRequestSignature();

            var queryString = $"?results=true&matchStats=true&season={providerSeasonId}&seeds=true&api_key={_statsApiKey}&sig={requestSignature}";

            var requestUriString = _statsApiBaseUrl + resultsUrl + queryString;

            var webRequestForResults = WebRequest.Create(requestUriString);

            webRequestForResults.Method = "GET";
            webRequestForResults.Headers.Add(HttpRequestHeader.CacheControl, _cacheControlHeader);

            return webRequestForResults;
        }

        public WebRequest GetWebRequestForResults(string leagueProviderSlug, int eventId, int matchId, int providerSeasonId)
        {
            var resultsUrl = $"/v1/stats/tennis/{leagueProviderSlug}/events/{eventId}";

            var requestSignature = GetRequestSignature();

            var queryString = $"?results=true&matchStats=true&season={providerSeasonId}&seeds=true&api_key={_statsApiKey}&sig={requestSignature}";

            var requestUriString = _statsApiBaseUrl + resultsUrl + queryString;

            var webRequestForResults = WebRequest.Create(requestUriString);

            webRequestForResults.Method = "GET";
            webRequestForResults.Headers.Add(HttpRequestHeader.CacheControl, _cacheControlHeader);

            return webRequestForResults;
        }
    }
}
