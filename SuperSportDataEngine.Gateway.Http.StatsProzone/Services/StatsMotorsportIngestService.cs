using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Motorsport;

namespace SuperSportDataEngine.Gateway.Http.StatsProzone.Services
{
    using System;
    using System.Net;
    using System.IO;
    using System.Text;
    using Newtonsoft.Json;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Interfaces;
    using SuperSportDataEngine.Common.Logging;
    using SuperSportDataEngine.ApplicationLogic.Constants.Providers;

    public class StatsMotorsportIngestService : IStatsMotorsportIngestService
    {
        private readonly IStatsMotorsportWebRequest _statsMotorsportMotorsportWebRequest;
        private readonly ILoggingService _loggingService;

        public StatsMotorsportIngestService(IStatsMotorsportWebRequest statsMotorsportMotorsportWebRequest, ILoggingService loggingService)
        {
            _statsMotorsportMotorsportWebRequest = statsMotorsportMotorsportWebRequest;
            _loggingService = loggingService;
        }

        public MotorsportEntitiesResponse IngestLeagues()
        {
            try
            {
                var requestForLeagues = _statsMotorsportMotorsportWebRequest.GetRequestForLeagues();

                MotorsportEntitiesResponse leagues;

                using (var webResponse = requestForLeagues.GetResponse())
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
            catch (WebException exception)
            {
                LogProviderErrorResponse(exception);

                return null;
            }
        }

        public MotorsportEntitiesResponse IngestLeagueSeason(string providerSlug)
        {
            try
            {
                var requestForSeasons = _statsMotorsportMotorsportWebRequest.GetRequestForSeasons(providerSlug);

                MotorsportEntitiesResponse seasons;

                using (var webResponse = requestForSeasons.GetResponse())
                {
                    using (var responseStream = webResponse.GetResponseStream())
                    {
                        if (responseStream == null) return null;

                        var streamReader = new StreamReader(responseStream, Encoding.UTF8);

                        seasons = JsonConvert.DeserializeObject<MotorsportEntitiesResponse>(streamReader.ReadToEnd());
                    }
                }

                return seasons;
            }
            catch (WebException exception)
            {
                LogProviderErrorResponse(exception);

                return null;
            }
        }

        public MotorsportEntitiesResponse IngestLeagueRaces(string providerSlug, int providerSeasonId)
        {
            try
            {
                var requestForRaces = _statsMotorsportMotorsportWebRequest.GetRequestForRaces(providerSlug, providerSeasonId);

                MotorsportEntitiesResponse races;

                using (var statsWebResponse = requestForRaces.GetResponse())
                {
                    using (var responseStream = statsWebResponse.GetResponseStream())
                    {
                        if (responseStream == null) return null;

                        var streamReader = new StreamReader(responseStream, Encoding.UTF8);

                        races = JsonConvert.DeserializeObject<MotorsportEntitiesResponse>(streamReader.ReadToEnd());
                    }
                }

                return races;
            }
            catch (WebException exception)
            {
                LogProviderErrorResponse(exception);

                return null;
            }
        }

        public MotorsportEntitiesResponse IngestRaceEventsForLeague(string providerSlug, int providerSeasonId, int providerRaceId)
        {
            try
            {
                var requestForRaceEvents =
                    _statsMotorsportMotorsportWebRequest.GetRequestForRaceEvents(providerSlug, providerSeasonId, providerRaceId);

                MotorsportEntitiesResponse raceEventsForLeague;

                using (var webResponse = requestForRaceEvents.GetResponse())
                {
                    using (var responseStream = webResponse.GetResponseStream())
                    {
                        if (responseStream is null) return null;

                        var streamReader = new StreamReader(responseStream, Encoding.UTF8);

                        raceEventsForLeague = JsonConvert.DeserializeObject<MotorsportEntitiesResponse>(streamReader.ReadToEnd());
                    }
                }

                return raceEventsForLeague;
            }
            catch (WebException exception)
            {
                LogProviderErrorResponse(exception);

                return null;
            }
        }

        public MotorsportEntitiesResponse IngestRaceResults(string providerSlug, int providerSeasonId, int providerRaceId)
        {
            try
            {
                var raceResultsRequest =
                        _statsMotorsportMotorsportWebRequest.GetRequestForRaceResults(providerSlug, providerSeasonId, providerRaceId);

                MotorsportEntitiesResponse raceResults;

                using (var webResponse = raceResultsRequest.GetResponse())
                {
                    using (var responseStream = webResponse.GetResponseStream())
                    {
                        if (responseStream is null) return null;

                        var streamReader = new StreamReader(responseStream, Encoding.UTF8);

                        raceResults =
                            JsonConvert.DeserializeObject<MotorsportEntitiesResponse>(streamReader.ReadToEnd());
                    }
                }

                return raceResults;
            }
            catch (WebException exception)
            {
                LogProviderErrorResponse(exception);

                return null;
            }
        }

        public MotorsportEntitiesResponse IngestRaceGrid(string providerSlug, int providerSeasonId, int providerRaceId)
        {
            try
            {
                var requestForRaceGrid =
                        _statsMotorsportMotorsportWebRequest.GetRequestForRaceGrid(providerSlug, providerSeasonId, providerRaceId);

                MotorsportEntitiesResponse raceGrid;

                using (var webResponse = requestForRaceGrid.GetResponse())
                {
                    using (var responseStream = webResponse.GetResponseStream())
                    {
                        if (responseStream is null) return null;

                        var streamReader = new StreamReader(responseStream, Encoding.UTF8);

                        raceGrid =
                            JsonConvert.DeserializeObject<MotorsportEntitiesResponse>(streamReader.ReadToEnd());
                    }
                }

                return raceGrid;
            }
            catch (WebException exception)
            {
                LogProviderErrorResponse(exception);

                return null;
            }
        }

        public MotorsportEntitiesResponse IngestDriversForLeague(string providerSlug, int? providerDriverId)
        {
            try
            {
                var requestForDrivers =
                    _statsMotorsportMotorsportWebRequest.GetRequestForDrivers(providerSlug, providerDriverId);

                MotorsportEntitiesResponse drivers;

                using (var webResponse = requestForDrivers.GetResponse())
                {
                    using (var responseStream = webResponse.GetResponseStream())
                    {
                        if (responseStream is null) return null;

                        var streamReader = new StreamReader(responseStream, Encoding.UTF8);

                        drivers = JsonConvert.DeserializeObject<MotorsportEntitiesResponse>(streamReader.ReadToEnd());
                    }
                }

                return drivers;
            }
            catch (WebException exception)
            {
                LogProviderErrorResponse(exception);

                return null;
            }
        }

        public MotorsportEntitiesResponse IngestTeamsForLeague(string providerSlug)
        {
            try
            {
                var requestForTeams = _statsMotorsportMotorsportWebRequest.GetRequestForTeams(providerSlug);

                MotorsportEntitiesResponse teams;

                using (var webResponse = requestForTeams.GetResponse())
                {
                    using (var responseStream = webResponse.GetResponseStream())
                    {
                        if (responseStream is null) return null;

                        var streamReader = new StreamReader(responseStream, Encoding.UTF8);

                        teams = JsonConvert.DeserializeObject<MotorsportEntitiesResponse>(streamReader.ReadToEnd());
                    }
                }

                return teams;
            }
            catch (WebException exception)
            {
                LogProviderErrorResponse(exception);

                return null;
            }
        }

        public MotorsportEntitiesResponse IngestDriverStandings(string providerSlug, int providerSeasonId)
        {
            var driverStandings = IngestStandings(providerSlug, MotorsportStatsConstants.DriverStandingsTypeId, providerSeasonId);

            return driverStandings;
        }

        public MotorsportEntitiesResponse IngestTeamStandings(string providerSlug, int providerSeasonId)
        {
            var teamStandings = IngestStandings(providerSlug, MotorsportStatsConstants.TeamStandingsTypeId, providerSeasonId);

            return teamStandings;
        }

        private MotorsportEntitiesResponse IngestStandings(string providerSlug, string standingsTypeId, int providerSeasonId)
        {
            try
            {
                var requestForStandings =
                    _statsMotorsportMotorsportWebRequest.GetRequestForStandings(providerSlug, standingsTypeId, providerSeasonId);

                MotorsportEntitiesResponse standings;

                using (var webResponse = requestForStandings.GetResponse())
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
            catch (WebException exception)
            {
                LogProviderErrorResponse(exception);

                return null;
            }
        }

        private void LogProviderErrorResponse(WebException exception)
        {
            if (!(exception.Response is HttpWebResponse errorResponse) || errorResponse.StatusCode != HttpStatusCode.NotFound) return;

            const string key = "MOTORSPORT-PROVIDER-RESPONSE-ERROR";

            var url = exception.Response.ResponseUri.AbsoluteUri;

            _loggingService.Warn(key, exception, $"{key}{Environment.NewLine}{exception.Message}{Environment.NewLine}{url}");
        }
    }
}