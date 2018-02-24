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

        public MotorEntitiesResponse IngestLeagues()
        {
            var webRequestForTournamentsIngest = _statsMotorsportWebRequest.GetRequestForTournaments();

            MotorEntitiesResponse leagues;

            using (var webResponse = webRequestForTournamentsIngest.GetResponse())
            {
                using (var responseStream = webResponse.GetResponseStream())
                {
                    if (responseStream == null) return null;

                    var streamReader = new StreamReader(responseStream, Encoding.UTF8);

                    leagues = JsonConvert.DeserializeObject<MotorEntitiesResponse>(streamReader.ReadToEnd());
                }
            }

            return leagues;
        }

        public MotorEntitiesResponse IngestLeagueSeasons(string providerSlug)
        {
            var webRequestForTournamentsIngest = _statsMotorsportWebRequest.GetRequestForLeagueSeasons(providerSlug);

            try
            {
                MotorEntitiesResponse seasons;

                using (var webResponse = webRequestForTournamentsIngest.GetResponse())
                {
                    if (((HttpWebResponse)webResponse).StatusCode != HttpStatusCode.OK) return null;

                    using (var responseStream = webResponse.GetResponseStream())
                    {
                        if (responseStream == null) return null;

                        var streamReader = new StreamReader(responseStream, Encoding.UTF8);

                        seasons = JsonConvert.DeserializeObject<MotorEntitiesResponse>(streamReader.ReadToEnd());
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

        public MotorEntitiesResponse IngestTournamentRaces(string providerSlug)
        {
            var statsWebRequest = _statsMotorsportWebRequest.GetRequestForRaces(providerSlug);

            MotorEntitiesResponse tournamentRacesEntitiesResponse;

            using (var statsWebResponse = statsWebRequest.GetResponse())
            {
                using (var responseStream = statsWebResponse.GetResponseStream())
                {
                    if (responseStream == null) return null;

                    var streamReader = new StreamReader(responseStream, Encoding.UTF8);

                    tournamentRacesEntitiesResponse =
                        JsonConvert.DeserializeObject<MotorEntitiesResponse>(streamReader.ReadToEnd());
                }
            }

            return tournamentRacesEntitiesResponse;
        }

        public MotorEntitiesResponse IngestLeagueCalendar(string providerSlug, int providerSeasonId)
        {
            var requestForSchedule = _statsMotorsportWebRequest.GetRequestForSchedule(providerSlug, providerSeasonId);

            MotorEntitiesResponse tournamentSchedule;

            using (var webResponse = requestForSchedule.GetResponse())
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

        public MotorEntitiesResponse IngestRaceResults(string providerSlug, int providerSeasonId, int providerRaceId)
        {
            var raceResultsRequest =
                _statsMotorsportWebRequest.GetRequestForRaceResults(providerSlug, providerSeasonId, providerRaceId);

            MotorEntitiesResponse raceResultsResponse;

            using (var webResponse = raceResultsRequest.GetResponse())
            {
                using (var responseStream = webResponse.GetResponseStream())
                {
                    if (responseStream is null) return null;

                    var streamReader = new StreamReader(responseStream, Encoding.UTF8);

                    raceResultsResponse =
                        JsonConvert.DeserializeObject<MotorEntitiesResponse>(streamReader.ReadToEnd());
                }
            }

            return raceResultsResponse;
        }

        public MotorEntitiesResponse IngestRaceGrid(string providerSlug, int providerSeasonId, int raceId)
        {
            return IngestRaceResults(providerSlug, providerSeasonId, providerSeasonId);
        }

        public MotorEntitiesResponse IngestDriversForLeague(string providerSlug, int providerSeasonId)
        {
            var webRequestForDriverIngest =
                _statsMotorsportWebRequest.GetRequestForDrivers(providerSlug, providerSeasonId);

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

        public MotorEntitiesResponse IngestTeamsForLeague(string providerSlug)
        {
            var webRequestForTeamIngestIngest = _statsMotorsportWebRequest.GetRequestForTeams(providerSlug);

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

        public MotorEntitiesResponse IngestDriverStandings(string providerSlug, int providerSeasonId)
        {
            var driverStandings = IngestStandings(providerSlug, DriverStandingsTypeId, providerSeasonId);

            return driverStandings;
        }

        public MotorEntitiesResponse IngestTeamStandings(string providerSlug, int providerSeasonId)
        {
            var teamStandings = IngestStandings(providerSlug, TeamStandingsTypeId, providerSeasonId);

            return teamStandings;
        }

        private MotorEntitiesResponse IngestStandings(string providerSlug, string standingsTypeId, int providerSeasonId)
        {
            var webRequestForStandingsIngest = _statsMotorsportWebRequest.GetRequestForStandings(providerSlug, standingsTypeId, providerSeasonId);

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
    }
}