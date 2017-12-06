using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Motor;

namespace SuperSportDataEngine.ApplicationLogic.Services
{
    public class MotorIngestWorkerService : IMotorIngestWorkerService
    {
        private readonly IStatsProzoneMotorIngestService _statsProzoneMotorIngestService;

        //TODO: Get this from repository
        private readonly List<string> _activeTournamentsSlugsTemp = new List<string> { "f1" };

        public MotorIngestWorkerService(IStatsProzoneMotorIngestService statsProzoneMotorIngestService)
        {
            _statsProzoneMotorIngestService = statsProzoneMotorIngestService;
        }

        public async Task IngestDriversForActiveTournaments(CancellationToken cancellationToken)
        {
            if (_activeTournamentsSlugsTemp != null && _activeTournamentsSlugsTemp.Count > 0)
            {
                foreach (var slug in _activeTournamentsSlugsTemp)
                {
                    var tournamentDrivers = _statsProzoneMotorIngestService.IngestTournamentDrivers(slug);

                    await PersistTournamentDriversInRepository(tournamentDrivers);
                }
            }
        }

        public async Task IngestTeamsForActiveTournaments(CancellationToken cancellationToken)
        {
            if (_activeTournamentsSlugsTemp != null && _activeTournamentsSlugsTemp.Count > 0)
            {
                foreach (var slug in _activeTournamentsSlugsTemp)
                {
                    var tournamentDrivers = _statsProzoneMotorIngestService.IngestTournamentTeams(slug);

                    await PersistTournamentTeamsInRepository(tournamentDrivers);
                }
            }
        }

        public async Task IngestDriverStandingsForActiveTournaments(CancellationToken cancellationToken)
        {
            if (_activeTournamentsSlugsTemp != null && _activeTournamentsSlugsTemp.Count > 0)
            {
                foreach (var slug in _activeTournamentsSlugsTemp)
                {
                    var driverStandings = _statsProzoneMotorIngestService.IngestDriverStandings(slug);

                    await PersistDriverStandingsInRepository(driverStandings, cancellationToken);
                }
            }
        }

        public async Task IngestTeamStandingsForActiveTournaments(CancellationToken cancellationToken)
        {
            if (_activeTournamentsSlugsTemp != null && _activeTournamentsSlugsTemp.Count > 0)
            {
                foreach (var slug in _activeTournamentsSlugsTemp)
                {
                    var teamStandings = _statsProzoneMotorIngestService.IngestDriverStandings(slug);

                    await PersistTeamStandingsInRepository(teamStandings, cancellationToken);
                }
            }
        }

        public async Task IngestTournaments(CancellationToken cancellationToken)
        {
            var tournaments = _statsProzoneMotorIngestService.IngestTournaments();

            await PersistTournamentsInRepository(tournaments, cancellationToken);
        }

        public async Task IngestRacesForActiveTournaments(CancellationToken cancellationToken)
        {
            if (_activeTournamentsSlugsTemp != null && _activeTournamentsSlugsTemp.Count > 0)
            {
                foreach (var slug in _activeTournamentsSlugsTemp)
                {
                    var races = _statsProzoneMotorIngestService.IngestTournamentRaces(slug);

                    await PersistRacesInRepository(races, cancellationToken);
                }
            }
        }

        public async Task IngestSchedulesForActiveTournaments(CancellationToken cancellationToken)
        {
            if (_activeTournamentsSlugsTemp != null && _activeTournamentsSlugsTemp.Count > 0)
            {
                foreach (var slug in _activeTournamentsSlugsTemp)
                {
                    const int tempSeasonId = 2017; // TODO: Get this from the repository

                    var schedule = _statsProzoneMotorIngestService.IngestTournamentSchedule(slug, tempSeasonId);

                    await PersistScheduleInRepository(schedule, cancellationToken);
                }
            }
        }

        public async Task IngestTournamentResults(string providerSlug, int providerSeasonId, int providerRaceId,
            CancellationToken cancellationToken)
        {
            var raceResults = _statsProzoneMotorIngestService.IngestTournamentResults(providerSlug, providerSeasonId, providerRaceId);

            await PersistResultsInRepository(raceResults, cancellationToken);
        }

        public async Task IngestTournamentGrid(string providerSlug, int providerSeasonId, int providerRaceId,
            CancellationToken cancellationToken)
        {
            var tournamentGrid = _statsProzoneMotorIngestService.IngestTournamentGrid(
                providerSlug, providerSeasonId, providerRaceId);

            await PersistGridInRepository(tournamentGrid, cancellationToken);
        }

        private static async Task PersistGridInRepository(MotorEntitiesResponse tournamentGrid, CancellationToken cancellationToken)
        {
            // TODO
        }

        private static async Task PersistTournamentDriversInRepository(MotorEntitiesResponse tournamentDriversResponse)
        {
            // TODO
        }

        private static async Task PersistTournamentTeamsInRepository(MotorEntitiesResponse tournamentTeamsResponse)
        {
            // TODO
        }

        private static async Task PersistDriverStandingsInRepository(MotorEntitiesResponse driverStandings,
            CancellationToken cancellationToken)
        {
            //TODO
        }

        private static async Task PersistTeamStandingsInRepository(MotorEntitiesResponse teamStandings,
            CancellationToken cancellationToken)
        {
            //TODO
        }

        private static async Task PersistTournamentsInRepository(MotorEntitiesResponse tournaments,
            CancellationToken cancellationToken)
        {
            //TODO
        }

        private async Task PersistRacesInRepository(MotorEntitiesResponse races, CancellationToken cancellationToken)
        {
            //TODO
        }

        private static async Task PersistScheduleInRepository(MotorEntitiesResponse races, CancellationToken cancellationToken)
        {
            //TODO
        }

        private async Task PersistResultsInRepository(MotorEntitiesResponse raceResults, CancellationToken cancellationToken)
        {
            //TODO
        }

    }
}