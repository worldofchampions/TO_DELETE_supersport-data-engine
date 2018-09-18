using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.Stats.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.Stats.Models.Tennis;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.Stats.Models.Tennis.TennisEventsResponse;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.Stats.Models.Tennis.TennisLeaguesResponse;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.Stats.Models.Tennis.TennisLeagueTournamentsResponse;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.Stats.Models.Tennis.TennisParticipantsResponse;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.Stats.Models.Tennis.TennisRankingsResponse;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.Stats.Models.Tennis.TennisResultsResponse;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.Stats.Models.Tennis.TennisSurfaceTypesResponse;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.Stats.Models.Tennis.TennisTournamentEventResponse;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.Stats.Models.Tennis.TennisVenuesResponse;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Interfaces;

namespace SuperSportDataEngine.Gateway.Http.Stats.Services.Tennis
{
    public class StatsTennisIngestService : IStatsTennisIngestService
    {
        private readonly IStatsTennisProviderWebRequest _tennisProviderWebRequest;

        public StatsTennisIngestService(
            IStatsTennisProviderWebRequest tennisProviderWebRequest)
        {
            _tennisProviderWebRequest = tennisProviderWebRequest;
        }

        public TennisLeaguesResponse GetLeagues()
        {
            try
            {
                var webRequestForTournamentsIngest = _tennisProviderWebRequest.GetRequestForLeagues();

                TennisLeaguesResponse tournamentsEntitiesResponse;

                using (var webResponse = webRequestForTournamentsIngest.GetResponse())
                {
                    using (var responseStream = webResponse.GetResponseStream())
                    {
                        if (responseStream == null) return null;

                        var streamReader = new StreamReader(responseStream, Encoding.UTF8);

                        tournamentsEntitiesResponse = JsonConvert.DeserializeObject<TennisLeaguesResponse>(streamReader.ReadToEnd());
                    }
                }

                return tournamentsEntitiesResponse;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public TennisLeagueTournamentsResponse GetTournamentsForLeague(string leagueProviderSlug)
        {
            try
            {
                var webRequestForTournamentsIngest = _tennisProviderWebRequest.GetWebRequestForTournamentDecode(leagueProviderSlug);

                TennisLeagueTournamentsResponse tournamentsEntitiesResponse;

                using (var webResponse = webRequestForTournamentsIngest.GetResponse())
                {
                    using (var responseStream = webResponse.GetResponseStream())
                    {
                        if (responseStream == null) return null;

                        var streamReader = new StreamReader(responseStream, Encoding.UTF8);

                        tournamentsEntitiesResponse = JsonConvert.DeserializeObject<TennisLeagueTournamentsResponse>(streamReader.ReadToEnd());
                    }
                }

                return tournamentsEntitiesResponse;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public TennisSeasonsResponse GetSeasonForLeague(string leagueProviderSlug)
        {
            try
            {
                var webRequestForSeasonsIngest = _tennisProviderWebRequest.GetWebRequestForSeasonsDecode(leagueProviderSlug);

                TennisSeasonsResponse seasonsEntitiesResponse;

                using (var webResponse = webRequestForSeasonsIngest.GetResponse())
                {
                    using (var responseStream = webResponse.GetResponseStream())
                    {
                        if (responseStream == null) return null;

                        var streamReader = new StreamReader(responseStream, Encoding.UTF8);

                        seasonsEntitiesResponse = JsonConvert.DeserializeObject<TennisSeasonsResponse>(streamReader.ReadToEnd());
                    }
                }

                return seasonsEntitiesResponse;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public TennisTournamentEventResponse GetCalendarEventForTournament(string leagueProviderSlug, int providerTournamentId, int seasonId)
        {
            try
            {
                var webRequestForSeasonsIngest = _tennisProviderWebRequest.GetWebRequestForTournamentEvent(leagueProviderSlug, providerTournamentId, seasonId);

                TennisTournamentEventResponse calendarResponse;

                using (var webResponse = webRequestForSeasonsIngest.GetResponse())
                {
                    using (var responseStream = webResponse.GetResponseStream())
                    {
                        if (responseStream == null) return null;

                        var streamReader = new StreamReader(responseStream, Encoding.UTF8);

                        calendarResponse = JsonConvert.DeserializeObject<TennisTournamentEventResponse>(streamReader.ReadToEnd());
                    }
                }

                return calendarResponse;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public TennisVenuesResponse GetVenuesForLeague(string leagueProviderSlug)
        {
            try
            {
                var webRequestForVenuesIngest = _tennisProviderWebRequest.GetWebRequestForVenuesDecode(leagueProviderSlug);

                TennisVenuesResponse venuesResponse;

                using (var webResponse = webRequestForVenuesIngest.GetResponse())
                {
                    using (var responseStream = webResponse.GetResponseStream())
                    {
                        if (responseStream == null) return null;

                        var streamReader = new StreamReader(responseStream, Encoding.UTF8);

                        venuesResponse = JsonConvert.DeserializeObject<TennisVenuesResponse>(streamReader.ReadToEnd());
                    }
                }

                return venuesResponse;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public TennisSurfaceTypesResponse GetSurfaceTypes(string leagueProviderSlug)
        {
            try
            {
                var webRequestForSurfaceTypesIngest = _tennisProviderWebRequest.GetWebRequestForSurfaceTypesDecode(leagueProviderSlug);

                TennisSurfaceTypesResponse surfaceTypesResponse;

                using (var webResponse = webRequestForSurfaceTypesIngest.GetResponse())
                {
                    using (var responseStream = webResponse.GetResponseStream())
                    {
                        if (responseStream == null) return null;

                        var streamReader = new StreamReader(responseStream, Encoding.UTF8);

                        surfaceTypesResponse = JsonConvert.DeserializeObject<TennisSurfaceTypesResponse>(streamReader.ReadToEnd());
                    }
                }

                return surfaceTypesResponse;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public TennisParticipantsResponse GetParticipants(string leagueProviderSlug)
        {
            try
            {
                var webRequestForParticipantsIngest = _tennisProviderWebRequest.GetWebRequestForParticipants(leagueProviderSlug);

                TennisParticipantsResponse participantsResponse;

                using (var webResponse = webRequestForParticipantsIngest.GetResponse())
                {
                    using (var responseStream = webResponse.GetResponseStream())
                    {
                        if (responseStream == null) return null;

                        var streamReader = new StreamReader(responseStream, Encoding.UTF8);

                        participantsResponse = JsonConvert.DeserializeObject<TennisParticipantsResponse>(streamReader.ReadToEnd());
                    }
                }

                return participantsResponse;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public TennisEventsResponse GetLeagueEvents(string leagueProviderSlug, int year)
        {
            try
            {
                var webRequestForEventsIngest = _tennisProviderWebRequest.GetWebRequestForEvents(leagueProviderSlug, year);

                TennisEventsResponse eventsResponse;

                using (var webResponse = webRequestForEventsIngest.GetResponse())
                {
                    using (var responseStream = webResponse.GetResponseStream())
                    {
                        if (responseStream == null) return null;

                        var streamReader = new StreamReader(responseStream, Encoding.UTF8);

                        eventsResponse = JsonConvert.DeserializeObject<TennisEventsResponse>(streamReader.ReadToEnd());
                    }
                }

                return eventsResponse;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public TennisRankingsResponse GetRankRankings(string providerSlug, int year)
        {
            try
            {
                var webRequestForRankingsIngest = _tennisProviderWebRequest.GetWebRequestForRankings(providerSlug, 1, "all", year);

                TennisRankingsResponse eventsResponse;

                using (var webResponse = webRequestForRankingsIngest.GetResponse())
                {
                    using (var responseStream = webResponse.GetResponseStream())
                    {
                        if (responseStream == null) return null;

                        var streamReader = new StreamReader(responseStream, Encoding.UTF8);

                        eventsResponse = JsonConvert.DeserializeObject<TennisRankingsResponse>(streamReader.ReadToEnd());
                    }
                }

                return eventsResponse;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public TennisRankingsResponse GetRaceRankings(string providerSlug, int year)
        {
            try
            {
                var webRequestForRankingsIngest = _tennisProviderWebRequest.GetWebRequestForRankings(providerSlug, 2, "all", year);

                TennisRankingsResponse eventsResponse;

                using (var webResponse = webRequestForRankingsIngest.GetResponse())
                {
                    using (var responseStream = webResponse.GetResponseStream())
                    {
                        if (responseStream == null) return null;

                        var streamReader = new StreamReader(responseStream, Encoding.UTF8);

                        eventsResponse = JsonConvert.DeserializeObject<TennisRankingsResponse>(streamReader.ReadToEnd());
                    }
                }

                return eventsResponse;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public TennisResultsResponse GetResultsForEvent(string providerSlug, int eventId, int providerSeasonId)
        {
            try
            {
                var webRequestForResultsIngest = _tennisProviderWebRequest.GetWebRequestForResults(providerSlug, eventId, providerSeasonId);

                TennisResultsResponse eventsResponse;

                using (var webResponse = webRequestForResultsIngest.GetResponse())
                {
                    using (var responseStream = webResponse.GetResponseStream())
                    {
                        if (responseStream == null) return null;

                        var streamReader = new StreamReader(responseStream, Encoding.UTF8);

                        eventsResponse = JsonConvert.DeserializeObject<TennisResultsResponse>(streamReader.ReadToEnd());
                    }
                }

                return eventsResponse;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public TennisResultsResponse GetResultsForMatch(string providerSlug, int eventId, int matchId, int providerSeasonId)
        {
            try
            {
                var webRequestForResultsIngest = _tennisProviderWebRequest.GetWebRequestForResults(providerSlug, eventId, matchId, providerSeasonId);

                TennisResultsResponse eventsResponse;

                using (var webResponse = webRequestForResultsIngest.GetResponse())
                {
                    using (var responseStream = webResponse.GetResponseStream())
                    {
                        if (responseStream == null) return null;

                        var streamReader = new StreamReader(responseStream, Encoding.UTF8);

                        eventsResponse = JsonConvert.DeserializeObject<TennisResultsResponse>(streamReader.ReadToEnd());
                    }
                }

                return eventsResponse;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
