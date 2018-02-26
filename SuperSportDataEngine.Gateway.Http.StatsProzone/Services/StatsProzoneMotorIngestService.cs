using System;
using System.Net;

namespace SuperSportDataEngine.Gateway.Http.StatsProzone.Services
{
    using System.IO;
    using System.Text;
    using Newtonsoft.Json;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Motor;

    public class StatsProzoneMotorIngestService : IStatsProzoneMotorIngestService
    {
        private const string TeamStandingsTypeId = "2"; //TODO Move to STATS constants

        private const string DriverStandingsTypeId = "1"; //TODO Move to STATS constants

        private readonly IProviderWebRequest _statsMotorsportWebRequest;

        public StatsProzoneMotorIngestService(IProviderWebRequest statsMotorsportWebRequest)
        {
            _statsMotorsportWebRequest = statsMotorsportWebRequest;
        }

        public MotorsportEntitiesResponse IngestLeagues()
        {
            var webRequestForTournamentsIngest = _statsMotorsportWebRequest.GetRequestForTournaments();

            MotorsportEntitiesResponse leagues;

            using (var webResponse = webRequestForTournamentsIngest.GetResponse())
            {
                using (var responseStream = webResponse.GetResponseStream())
                {
                    if (responseStream == null) return null;

                    var streamReader = new StreamReader(responseStream, Encoding.UTF8);

                    leagues = JsonConvert.DeserializeObject<MotorsportEntitiesResponse>(streamReader.ReadToEnd());
                }
            }

            return leagues;
        }

        public MotorsportEntitiesResponse IngestLeagueSeasons(string providerSlug)
        {
            var webRequestForTournamentsIngest = _statsMotorsportWebRequest.GetRequestForLeagueSeasons(providerSlug);

            try
            {
                MotorsportEntitiesResponse seasons;

                using (var webResponse = webRequestForTournamentsIngest.GetResponse())
                {
                    if (((HttpWebResponse)webResponse).StatusCode != HttpStatusCode.OK) return null;

                    using (var responseStream = webResponse.GetResponseStream())
                    {
                        if (responseStream == null) return null;

                        var streamReader = new StreamReader(responseStream, Encoding.UTF8);

                        seasons = JsonConvert.DeserializeObject<MotorsportEntitiesResponse>(streamReader.ReadToEnd());
                    }

                }

                return seasons;
            }
            catch (WebException)
            {
                // Provider must have returned error object hence failed to serealize it.
                // TODO rework handling of this
                return null;
            }
        }

        public MotorsportEntitiesResponse IngestTournamentRaces(string providerSlug)
        {
            var statsWebRequest = _statsMotorsportWebRequest.GetRequestForRaces(providerSlug);

            MotorsportEntitiesResponse tournamentRacesEntitiesResponse;

            using (var statsWebResponse = statsWebRequest.GetResponse())
            {
                using (var responseStream = statsWebResponse.GetResponseStream())
                {
                    if (responseStream == null) return null;

                    var streamReader = new StreamReader(responseStream, Encoding.UTF8);

                    tournamentRacesEntitiesResponse =
                        JsonConvert.DeserializeObject<MotorsportEntitiesResponse>(streamReader.ReadToEnd());
                }
            }

            return tournamentRacesEntitiesResponse;
        }

        public MotorsportEntitiesResponse IngestLeagueCalendar(string providerSlug, int providerSeasonId)
        {
            try
            {
                var requestForSchedule = _statsMotorsportWebRequest.GetRequestForSchedule(providerSlug, providerSeasonId);

                MotorsportEntitiesResponse tournamentSchedule;

                using (var webResponse = requestForSchedule.GetResponse())
                {
                    using (var responseStream = webResponse.GetResponseStream())
                    {
                        if (responseStream is null) return null;

                        var streamReader = new StreamReader(responseStream, Encoding.UTF8);

                        tournamentSchedule = JsonConvert.DeserializeObject<MotorsportEntitiesResponse>(streamReader.ReadToEnd());
                    }
                }

                return tournamentSchedule;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);

                return null;
            }
        }

        public MotorsportEntitiesResponse IngestRaceResults(string providerSlug, int providerSeasonId, int providerRaceId)
        {
            var raceResultsRequest =
                _statsMotorsportWebRequest.GetRequestForRaceResults(providerSlug, providerSeasonId, providerRaceId);

            MotorsportEntitiesResponse raceResultsResponse;

            using (var webResponse = raceResultsRequest.GetResponse())
            {
                using (var responseStream = webResponse.GetResponseStream())
                {
                    if (responseStream is null) return null;

                    var streamReader = new StreamReader(responseStream, Encoding.UTF8);

                    raceResultsResponse =
                        JsonConvert.DeserializeObject<MotorsportEntitiesResponse>(streamReader.ReadToEnd());
                }
            }

            return raceResultsResponse;
        }

        public MotorsportEntitiesResponse IngestRaceGrid(string providerSlug, int providerSeasonId, int raceId)
        {
            return IngestRaceResults(providerSlug, providerSeasonId, providerSeasonId);
        }

        public MotorsportEntitiesResponse IngestDriversForLeague(string providerSlug, int providerSeasonId)
        {
            var webRequestForDriverIngest =
                _statsMotorsportWebRequest.GetRequestForDrivers(providerSlug, providerSeasonId);

            MotorsportEntitiesResponse tournamentDrivers;

            using (var webResponse = webRequestForDriverIngest.GetResponse())
            {
                using (var responseStream = webResponse.GetResponseStream())
                {
                    if (responseStream is null) return null;

                    var streamReader = new StreamReader(responseStream, Encoding.UTF8);

                    tournamentDrivers = JsonConvert.DeserializeObject<MotorsportEntitiesResponse>(streamReader.ReadToEnd());
                }
            }

            return tournamentDrivers;
        }

        public MotorsportEntitiesResponse IngestTeamsForLeague(string providerSlug)
        {
            var webRequestForTeamIngestIngest = _statsMotorsportWebRequest.GetRequestForTeams(providerSlug);

            MotorsportEntitiesResponse tournamentTeamsResponse;

            using (var webResponse = webRequestForTeamIngestIngest.GetResponse())
            {
                using (var responseStream = webResponse.GetResponseStream())
                {
                    if (responseStream is null) return null;

                    var streamReader = new StreamReader(responseStream, Encoding.UTF8);

                    tournamentTeamsResponse = JsonConvert.DeserializeObject<MotorsportEntitiesResponse>(streamReader.ReadToEnd());
                }
            }

            return tournamentTeamsResponse;
        }

        public MotorsportEntitiesResponse IngestDriverStandings(string providerSlug, int providerSeasonId)
        {
            var driverStandings = IngestStandings(providerSlug, DriverStandingsTypeId, providerSeasonId);

            return driverStandings;
        }

        public MotorsportEntitiesResponse IngestTeamStandings(string providerSlug, int providerSeasonId)
        {
            var teamStandings = IngestStandings(providerSlug, TeamStandingsTypeId, providerSeasonId);

            return teamStandings;
        }

        private MotorsportEntitiesResponse IngestStandings(string providerSlug, string standingsTypeId, int providerSeasonId)
        {
            var webRequestForStandingsIngest = _statsMotorsportWebRequest.GetRequestForStandings(providerSlug, standingsTypeId, providerSeasonId);

            MotorsportEntitiesResponse standings;

            using (var webResponse = webRequestForStandingsIngest.GetResponse())
            {
                using (var responseStream = webResponse.GetResponseStream())
                {
                    if (responseStream == null) return null;

                    var streamReader = new StreamReader(responseStream, Encoding.UTF8);

                    standings = JsonConvert.DeserializeObject<MotorsportEntitiesResponse>(streamReader.ReadToEnd());
                }
            }

            return standings;
        }
    }
}