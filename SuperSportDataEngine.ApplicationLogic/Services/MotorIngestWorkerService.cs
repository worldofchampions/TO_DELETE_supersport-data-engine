using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Motor;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;

namespace SuperSportDataEngine.ApplicationLogic.Services
{
    public class MotorIngestWorkerService : IMotorIngestWorkerService
    {
        private readonly IStatsProzoneMotorIngestService _statsProzoneMotorIngestService;

        //TODO: Get this from UnitOfWwork
        private readonly IBaseEntityFrameworkRepository<MotorDriver> _motorDriverRepository;
        private readonly IBaseEntityFrameworkRepository<MotorLeague> _motorLeaguesRepository;

        public MotorIngestWorkerService(
            IStatsProzoneMotorIngestService statsProzoneMotorIngestService,
            IBaseEntityFrameworkRepository<MotorDriver> motorDriverRepository,
            IBaseEntityFrameworkRepository<MotorLeague> motorLeaguesRepository)
        {
            _statsProzoneMotorIngestService = statsProzoneMotorIngestService;
            _motorDriverRepository = motorDriverRepository;
            _motorLeaguesRepository = motorLeaguesRepository;
        }

        public async Task IngestDriversForActiveTournaments(CancellationToken cancellationToken)
        {
            var motorLeagues = _motorLeaguesRepository.Where(league => league.IsEnabled);
            if (motorLeagues != null)
            {
                foreach (var league in motorLeagues)
                {
                    var tournamentDrivers = _statsProzoneMotorIngestService.IngestTournamentDrivers(league.ProviderSlug);
                    await PersistTournamentDriversInRepository(tournamentDrivers);
                }
            }
        }

        public async Task IngestTeamsForActiveTournaments(CancellationToken cancellationToken)
        {
            var motorLeagues = _motorLeaguesRepository.Where(league => league.IsEnabled);
            if (motorLeagues != null)
            {
                foreach (var league in motorLeagues)
                {
                    if (league.ProviderSlug == null) continue;
                    var tournamentDrivers = _statsProzoneMotorIngestService.IngestTournamentTeams(league.ProviderSlug);
                    await PersistTournamentTeamsInRepository(tournamentDrivers);
                }
            }
        }

        public async Task IngestDriverStandingsForActiveTournaments(CancellationToken cancellationToken)
        {
            var motorLeagues = _motorLeaguesRepository.Where(league => league.IsEnabled);
            if (motorLeagues != null)
            {
                foreach (var league in motorLeagues)
                {
                    if (league.ProviderSlug == null) continue;
                    var driverStandings = _statsProzoneMotorIngestService.IngestDriverStandings(league.ProviderSlug);
                    await PersistDriverStandingsInRepository(driverStandings, cancellationToken);
                }
            }
        }

        public async Task IngestTeamStandingsForActiveTournaments(CancellationToken cancellationToken)
        {
            var motorLeagues = _motorLeaguesRepository.Where(league => league.IsEnabled);
            if (motorLeagues != null)
            {
                foreach (var league in motorLeagues)
                {
                    if (league.ProviderSlug == null) continue;
                    var teamStandings = _statsProzoneMotorIngestService.IngestDriverStandings(league.ProviderSlug);
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
            var motorLeagues = _motorLeaguesRepository.Where(league => league.IsEnabled);
            var repositoryHasActiveLeagues = motorLeagues != null;
            if (repositoryHasActiveLeagues)
            {
                foreach (var league in motorLeagues)
                {
                    if (league.ProviderSlug == null) continue;
                    var races = _statsProzoneMotorIngestService.IngestTournamentRaces(league.ProviderSlug);
                    await PersistRacesInRepository(races, cancellationToken);
                }
            }
        }

        public async Task IngestSchedulesForActiveTournaments(CancellationToken cancellationToken)
        {
            var motorLeagues = _motorLeaguesRepository.Where(league => league.IsEnabled);
            var repositoryHasActiveLeagues = motorLeagues != null;
            if (repositoryHasActiveLeagues)
            {
                foreach (var league in motorLeagues)
                {
                    if (league.ProviderSlug == null) continue;
                    var schedule = _statsProzoneMotorIngestService.IngestTournamentSchedule(league.ProviderSlug, league.ProviderLeagueId);
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

        private async Task PersistTournamentDriversInRepository(MotorEntitiesResponse providerResponse)
        {
            var responseHasNoData = providerResponse.recordCount <= 0;
            if (responseHasNoData) return;

            var apiResult = providerResponse.apiResults.FirstOrDefault();
            var providerDataIsValid = apiResult?.league.subLeague?.players != null;
            if (providerDataIsValid)
            {
                var driversFromProvider = apiResult.league.subLeague.players;

                foreach (var providerDriver in driversFromProvider)
                {
                    var driverInRepo =
                        _motorDriverRepository.FirstOrDefault(d => d.ProviderDriverId == providerDriver.playerId);

                    if (driverInRepo is null)
                    {
                        SaveNewDriverInRepo(providerDriver);
                    }
                    else
                    {
                        UpdateDriverInRepo(providerDriver, driverInRepo);
                    }
                }
            }
        }

        private void SaveNewDriverInRepo(Player providerDriver)
        {
            var driver = new MotorDriver
            {
                FirstName = providerDriver.firstName,
                LastName = providerDriver.lastName,
                HeightInCentimeters = providerDriver.height.centimeters,
                WeightInKilograms = providerDriver.weight.kilograms,
                ProviderDriverId = providerDriver.playerId,
                DataProvider = DataProvider.StatsProzone
            };

            _motorDriverRepository.Add(driver);
        }

        private void UpdateDriverInRepo(Player providerDriver, MotorDriver driverFromRepository)
        {
            driverFromRepository.FirstName = providerDriver.firstName;
            driverFromRepository.LastName = providerDriver.lastName;
            driverFromRepository.HeightInCentimeters = providerDriver.height.centimeters;
            driverFromRepository.WeightInKilograms = providerDriver.weight.kilograms;
            driverFromRepository.DataProvider = DataProvider.StatsProzone;

            _motorDriverRepository.Update(driverFromRepository);
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