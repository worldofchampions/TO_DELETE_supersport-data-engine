using System.IO;
using System.Text;
using Newtonsoft.Json;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Motor;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RequestModels;

namespace SuperSportDataEngine.Gateway.Http.StatsProzone.Services
{
    public class StatsProzoneMotorIngestService : IStatsProzoneMotorIngestService
    {
        private const string TeamStandingsTypeId = "2"; //TODO Move to STATS constants

        private const string DriverStandingsTypeId = "1"; //TODO Move to STATS constants

        private readonly IProviderWebRequest _prozoneMotorWebRequest;
        public StatsProzoneMotorIngestService(IProviderWebRequest prozoneMotorWebRequest)
        {
            _prozoneMotorWebRequest = prozoneMotorWebRequest;
        }

        public MotorEntitiesResponse IngestTournaments()
        {
            var webRequestForTournamentsIngest = _prozoneMotorWebRequest.GetRequestForTournaments();

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
            var statsWebRequest = _prozoneMotorWebRequest.GetRequestForRaces(providerSlug);

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
            var requestForTournamentSchedule = _prozoneMotorWebRequest.GetRequestForSchedule(providerSlug, providerSeasonId);

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
            var raceResultsRequest = _prozoneMotorWebRequest.GetRequestRaceResults(motorResultRequestEntity.ProviderSlug, motorResultRequestEntity.ProviderSeasonId, motorResultRequestEntity.ProviderRaceId);

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
            var webRequestForDriverIngest = _prozoneMotorWebRequest.GetRequestForDrivers(providerSlug);

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
            var webRequestForTeamIngestIngest = _prozoneMotorWebRequest.GetRequestForTeams(providerSlug);

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

        public MotorEntitiesResponse IngestStandings(string providerSlug, string standingsTypeId)
        {
            var webRequestForStandingsIngest = _prozoneMotorWebRequest.GetRequestForStandings(providerSlug, standingsTypeId);

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