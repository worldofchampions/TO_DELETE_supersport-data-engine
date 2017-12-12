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
        private readonly IBaseEntityFrameworkRepository<MotorRace> _motorRaceRepository;


        public MotorIngestWorkerService(
            IStatsProzoneMotorIngestService statsProzoneMotorIngestService,
            IBaseEntityFrameworkRepository<MotorDriver> motorDriverRepository,
            IBaseEntityFrameworkRepository<MotorLeague> motorLeaguesRepository,
            IBaseEntityFrameworkRepository<MotorRace> motorRaceRepository)
        {
            _statsProzoneMotorIngestService = statsProzoneMotorIngestService;
            _motorDriverRepository = motorDriverRepository;
            _motorLeaguesRepository = motorLeaguesRepository;
            _motorRaceRepository = motorRaceRepository;
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
                    await PersistRacesInRepository(races, league, cancellationToken);
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

                await _motorDriverRepository.SaveAsync();
            }
        }

        private async Task PersistRacesInRepository(MotorEntitiesResponse providerResponse, MotorLeague league, CancellationToken cancellationToken)
        {
            var responseHasNoData = providerResponse.recordCount <= 0;
            if (responseHasNoData) return;

            var apiResult = providerResponse.apiResults.FirstOrDefault();
            var providerDataIsValid = apiResult?.league.races != null;
            if (providerDataIsValid)
            {
                var racesFromProvider = apiResult.league.races;
                foreach (var providerRace in racesFromProvider)
                {
                    var raceInRepo = _motorRaceRepository.FirstOrDefault(r => r.LegacyRaceId == providerRace.raceId);
                    if (raceInRepo is null)
                    {
                        SaveNewRaceInRepo(providerRace, league);
                    }
                    else
                    {
                        UpdateRaceInRepo(providerRace, raceInRepo);
                    }
                }

                await _motorRaceRepository.SaveAsync();
            }
        }

        private async Task PersistTournamentsInRepository(MotorEntitiesResponse providerResponse,
            CancellationToken cancellationToken)
        {
            if (providerResponse.recordCount <= 0) return;

            var apiResult = providerResponse.apiResults.FirstOrDefault();
            if (apiResult?.leagues != null)
            {
                var leaguesFromProvider = apiResult.leagues;
                foreach (var leagueFromProvider in leaguesFromProvider)
                {
                    var leagueInRepo = _motorLeaguesRepository.FirstOrDefault(l => l.ProviderLeagueId == leagueFromProvider.leagueId);
                    if (leagueInRepo is null)
                    {
                        SaveNewLeagueInRepo(leagueFromProvider);
                    }
                    else
                    {
                        UpdateLeagueInRepo(leagueFromProvider, leagueInRepo);
                    }
                }

                await _motorRaceRepository.SaveAsync();
            }
        }

        private void UpdateLeagueInRepo(League leagueFromProvider, MotorLeague leagueInRepo)
        {
            leagueInRepo.ProviderSlug = leagueFromProvider.uriPaths.FirstOrDefault()?.path;
            leagueInRepo.Abbreviation = leagueFromProvider.abbreviation;
            leagueInRepo.Name = leagueFromProvider.name;
            leagueInRepo.DisplayName = leagueFromProvider.displayName;

            _motorLeaguesRepository.Update(leagueInRepo);
        }

        private void SaveNewLeagueInRepo(League leagueFromProvider)
        {
            var league = new MotorLeague
            {
                ProviderSlug = leagueFromProvider.uriPaths.FirstOrDefault()?.path,
                Name = leagueFromProvider.name,
                DisplayName = leagueFromProvider.displayName,
                ProviderLeagueId = leagueFromProvider.leagueId,
                Abbreviation = leagueFromProvider.abbreviation
            };

            _motorLeaguesRepository.Add(league);
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

        private void UpdateDriverInRepo(Player providerDriver, MotorDriver driverInRepo)
        {
            driverInRepo.FirstName = providerDriver.firstName;
            driverInRepo.LastName = providerDriver.lastName;
            driverInRepo.HeightInCentimeters = providerDriver.height.centimeters;
            driverInRepo.WeightInKilograms = providerDriver.weight.kilograms;
            driverInRepo.DataProvider = DataProvider.StatsProzone;

            _motorDriverRepository.Update(driverInRepo);
        }

        private static void UpdateRaceInRepo(Race providerRace, MotorRace raceInRepo)
        {
            raceInRepo.Name = providerRace.name;
        }

        private void SaveNewRaceInRepo(Race motorRace, MotorLeague motorLeague)
        {
            var newRace = new MotorRace
            {
                ProviderRaceId = motorRace.raceId,
                Name = motorRace.name,
                DataProvider = DataProvider.StatsProzone,
                MotorLeague = motorLeague,
                MotorLeagueId = motorLeague.Id
            };

            _motorRaceRepository.Add(newRace);
        }

        private static async Task PersistTournamentTeamsInRepository(MotorEntitiesResponse tournamentTeamsResponse)
        {
            // TODO
        }

        private async Task PersistDriverStandingsInRepository(MotorEntitiesResponse providerResponse,
            CancellationToken cancellationToken)
        {
            //TODO
        }

        private static async Task PersistTeamStandingsInRepository(MotorEntitiesResponse teamStandings,
            CancellationToken cancellationToken)
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