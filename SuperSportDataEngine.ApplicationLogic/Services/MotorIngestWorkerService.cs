using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Motor;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RequestModels;
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
        private readonly IBaseEntityFrameworkRepository<MotorDriverStanding> _driverStandingRepository;
        private readonly IBaseEntityFrameworkRepository<MotorTeam> _teamsRepository;
        private readonly IBaseEntityFrameworkRepository<MotorRaceResult> _resultsRepository;



        public MotorIngestWorkerService(
            IStatsProzoneMotorIngestService statsProzoneMotorIngestService,
            IBaseEntityFrameworkRepository<MotorDriver> motorDriverRepository,
            IBaseEntityFrameworkRepository<MotorLeague> motorLeaguesRepository,
            IBaseEntityFrameworkRepository<MotorRace> motorRaceRepository,
            IBaseEntityFrameworkRepository<MotorDriverStanding> driverStandingRepository,
            IBaseEntityFrameworkRepository<MotorTeam> teamsRepository, 
            IBaseEntityFrameworkRepository<MotorRaceResult> resultsRepository)
        {
            _statsProzoneMotorIngestService = statsProzoneMotorIngestService;
            _motorDriverRepository = motorDriverRepository;
            _motorLeaguesRepository = motorLeaguesRepository;
            _motorRaceRepository = motorRaceRepository;
            _driverStandingRepository = driverStandingRepository;
            _teamsRepository = teamsRepository;
            _resultsRepository = resultsRepository;
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
                    var tournamentTeams = _statsProzoneMotorIngestService.IngestTournamentTeams(league.ProviderSlug);
                    await PersistTournamentTeamsInRepository(tournamentTeams);
                }
            }
        }

        public async Task IngestOwnersForActiveTournaments(CancellationToken cancellationToken)
        {
            var motorLeagues = _motorLeaguesRepository.Where(league => league.IsEnabled);
            if (motorLeagues != null)
            {
                foreach (var league in motorLeagues)
                {
                    if (league.ProviderSlug == null) continue;
                    var owners = _statsProzoneMotorIngestService.IngestTournamentOwners(league.ProviderSlug);
                    await PersistTournamentOwnersInRepository(owners);
                }
            }
        }

        public async Task IngestDriverStandingsForActiveTournaments(CancellationToken cancellationToken)
        {
            var motorLeagues = _motorLeaguesRepository.Where(race => race.IsEnabled);
            if (motorLeagues != null)
            {
                foreach (var league in motorLeagues)
                {
                    if (league.ProviderSlug == null) continue;
                    var driverStandings = _statsProzoneMotorIngestService.IngestDriverStandings(league.ProviderSlug);
                    await PersistDriverStandingsInRepository(driverStandings, league, cancellationToken);
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
                    var schedule =
                        _statsProzoneMotorIngestService.IngestTournamentSchedule(league.ProviderSlug, league.ProviderLeagueId);
                    await PersistScheduleInRepository(schedule, cancellationToken);
                }
            }
        }

        public async Task IngestTournamentResults(MotorResultRequestEntity resultRequestEntity,
            CancellationToken cancellationToken)
        {
            var raceResults = _statsProzoneMotorIngestService.IngestTournamentResults(resultRequestEntity);

            await PersistResultsInRepository(raceResults, cancellationToken);
        }

        public async Task IngestTournamentGrid(MotorResultRequestEntity motorResultRequestEntity, CancellationToken cancellationToken)
        {
            var tournamentGrid = _statsProzoneMotorIngestService.IngestTournamentGrid(motorResultRequestEntity);

            await PersistGridInRepository(tournamentGrid, cancellationToken);
        }

        private async Task PersistTournamentDriversInRepository(MotorEntitiesResponse providerResponse)
        {
            var driversFromProvider = ExtractDriversFromProviderResponse(providerResponse);
            if (driversFromProvider != null)
            {
                foreach (var providerDriver in driversFromProvider)
                {
                    var driverInRepo =
                        _motorDriverRepository.FirstOrDefault(d => d.ProviderId == providerDriver.playerId);

                    if (driverInRepo is null)
                    {
                        AddNewDriverToRepo(providerDriver);
                    }
                    else
                    {
                        UpdateDriverInRepo(providerDriver, driverInRepo);
                    }
                }

                await _motorDriverRepository.SaveAsync();
            }
        }

        private static IEnumerable<Player> ExtractDriversFromProviderResponse(MotorEntitiesResponse response)
        {
            var responseHasNoData = response.recordCount <= 0;
            return responseHasNoData ? null : response.apiResults.FirstOrDefault()?.leagues.FirstOrDefault()?.subLeague.players;
        }

        private async Task PersistRacesInRepository(MotorEntitiesResponse providerResponse,
            MotorLeague league, CancellationToken cancellationToken)
        {
            var racesFromProvider = ExtractRacesFromProviderResponse(providerResponse);
            if (racesFromProvider is null)
                return;

            foreach (var providerRace in racesFromProvider)
            {
                var raceInRepo = _motorRaceRepository.FirstOrDefault(r => r.LegacyRaceId == providerRace.raceId);
                if (raceInRepo is null)
                {
                    AddNewRaceToRepo(providerRace, league);
                }
                else
                {
                    UpdateRaceInRepo(providerRace, raceInRepo);
                }
            }

            await _motorRaceRepository.SaveAsync();
        }

        private async Task PersistTournamentsInRepository(MotorEntitiesResponse providerResponse,
            CancellationToken cancellationToken)
        {
            var leaguesFromProvider = ExtractLeaguesFromProviderResponse(providerResponse);
            if (leaguesFromProvider != null)
            {
                foreach (var leagueFromProvider in leaguesFromProvider)
                {
                    var leagueInRepo =
                        _motorLeaguesRepository.FirstOrDefault(l => l.ProviderLeagueId == leagueFromProvider.leagueId);
                    if (leagueInRepo is null)
                    {
                        AddNewLeagueToRepo(leagueFromProvider);
                    }
                    else
                    {
                        UpdateLeagueInRepo(leagueFromProvider, leagueInRepo);
                    }
                }

                await _motorRaceRepository.SaveAsync();
            }
        }

        private async Task PersistDriverStandingsInRepository(MotorEntitiesResponse providerResponse, MotorLeague league,
            CancellationToken cancellationToken)
        {
            var standingsFromProvider = ExtractDriverStandingsFromProviderResponse(providerResponse);
            if (standingsFromProvider != null)
            {
                foreach (var providerEntry in standingsFromProvider)
                {
                    var driverStanding =
                        _driverStandingRepository.FirstOrDefault(s => s.MotorDriver.ProviderId == providerEntry.playerId);
                    if (driverStanding is null)
                    {
                        AddNewDriverStandingToRepo(providerEntry, league);
                    }
                    else
                    {
                        UpdateDriverStandingInRepo(providerEntry, driverStanding);
                    }
                }

                await _driverStandingRepository.SaveAsync();
            }
        }

        private async Task PersistTournamentOwnersInRepository(MotorEntitiesResponse response)
        {
            var ownersFromProvider = ExtractOwnersFromProviderResponse(response);

            if (ownersFromProvider is null)
            {
                return;
            }

            foreach (var owner in ownersFromProvider)
            {
                var ownerInRepo = _teamsRepository.FirstOrDefault(o => o.ProviderTeamId == owner.ownerId);

                if (ownerInRepo is null)
                {
                    AddNewOwnerToRepo(owner);
                }
                else
                {
                    UpdateOwnerInRepo(ownerInRepo, owner);
                }
            }

            await _teamsRepository.SaveAsync();

        }

        private async Task PersistResultsInRepository(MotorEntitiesResponse response, CancellationToken cancellationToken)
        {
            var resultsFromProviderResponse = ExtractResultsFromProviderResponse(response);

            if (resultsFromProviderResponse is null)
            {
                return;
            }

            foreach (var result in resultsFromProviderResponse)
            {
                var playerId = result?.player?.playerId;
                if(playerId is null) continue;

                var resultInRepo = 
                    _resultsRepository.FirstOrDefault(r => r.MotorDriver.ProviderId == playerId);

                if (resultInRepo is null)
                {
                    AddNewResultsToRepo(result);
                }
                else
                {
                    UpdateResultInRepo(resultInRepo, result);
                }
            }

            await _resultsRepository.SaveAsync();
        }

        private void UpdateResultInRepo(MotorRaceResult resultInRepo, Result result)
        {
            resultInRepo.DriverTotalPoints = int.Parse(result.points.driver.total);
            resultInRepo.DriverPenaltyPoints = int.Parse(result.points.driver.penalty);
            resultInRepo.DriverBonusPoints = int.Parse(result.points.driver.bonus);

            resultInRepo.OwnerTotalPoints = int.Parse(result.points.owner.total);
            resultInRepo.OwnerBonusPoints = int.Parse(result.points.owner.bonus);
            resultInRepo.OwnerPenaltyPoints = int.Parse(result.points.owner.penalty);

            resultInRepo.FinishingTime.Hours = result.finishingTime.hours;
            resultInRepo.FinishingTime.Minutes = result.finishingTime.minutes;
            resultInRepo.FinishingTime.Seconds = result.finishingTime.seconds;

            resultInRepo.IsFastest = result.laps.isFastest;
            resultInRepo.LapsLed = result.laps.totalLed;
            resultInRepo.LapsBehind = result.laps.behind;
            resultInRepo.LapsCompleted = result.laps.completed;

            resultInRepo.Position = result.carPosition.position;
            resultInRepo.StartingPosition = result.carPosition.startingPosition;

            _resultsRepository.Update(resultInRepo);
        }

        private static void AddNewResultsToRepo(Result result)
        {
            throw new System.NotImplementedException();
        }

        private IEnumerable<Result> ExtractResultsFromProviderResponse(MotorEntitiesResponse response)
        {
            throw new System.NotImplementedException();
        }

        private static IEnumerable<Race> ExtractRacesFromProviderResponse(MotorEntitiesResponse response)
        {
            return response.recordCount <= 0 ? null : response.apiResults.FirstOrDefault()?.leagues.FirstOrDefault()?.races;
        }

        private static IEnumerable<League> ExtractLeaguesFromProviderResponse(MotorEntitiesResponse response)
        {
            return response.recordCount <= 0 ? null : response.apiResults.FirstOrDefault()?.leagues;
        }

        private static IEnumerable<Player> ExtractDriverStandingsFromProviderResponse(MotorEntitiesResponse response)
        {
            if (response.recordCount <= 0) return null;

            var result = response.apiResults.FirstOrDefault()?.leagues.FirstOrDefault()?.subLeague?.players;

            return result;
        }

        private static IEnumerable<Owner> ExtractOwnersFromProviderResponse(MotorEntitiesResponse response)
        {
            if (response.recordCount <= 0)
            {
                return null;
            }
            var results = response.apiResults.FirstOrDefault()?.leagues.FirstOrDefault()?.subLeague.owners;

            return results;
        }

        private void UpdateLeagueInRepo(League leagueFromProvider, MotorLeague leagueInRepo)
        {
            leagueInRepo.ProviderSlug = leagueFromProvider.uriPaths.FirstOrDefault()?.path;
            leagueInRepo.Abbreviation = leagueFromProvider.abbreviation;
            leagueInRepo.Name = leagueFromProvider.name;
            leagueInRepo.DisplayName = leagueFromProvider.displayName;

            _motorLeaguesRepository.Update(leagueInRepo);
        }

        private void AddNewLeagueToRepo(League leagueFromProvider)
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

        private void AddNewDriverToRepo(Player providerDriver)
        {
            var driver = new MotorDriver
            {
                ProviderId = providerDriver.playerId,
                FirstName = providerDriver.firstName,
                LastName = providerDriver.lastName,
                HeightInCentimeters = providerDriver.height.centimeters,
                WeightInKilograms = providerDriver.weight.kilograms,
                CountryName = providerDriver.birth.country.name,
                ProviderCarId = providerDriver.car.make.makeId,
                CarNumber = providerDriver.car.carNumber,
                CarDisplayNumber = providerDriver.car.carDisplayNumber,
                CarName = providerDriver.car.make.name,
                TeamName = providerDriver.owner.name,
                ProviderTeamId = providerDriver.owner.ownerId,
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

        private void AddNewRaceToRepo(Race motorRace, MotorLeague motorLeague)
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

        private void UpdateOwnerInRepo(MotorTeam ownerInRepo, Owner owner)
        {
            ownerInRepo.Name = owner.name;

            _teamsRepository.Update(ownerInRepo);
        }

        private void AddNewOwnerToRepo(Owner owner)
        {
            var newOwner = new MotorTeam
            {
                Name = owner.name,
                ProviderTeamId = owner.ownerId,
                DataProvider = DataProvider.StatsProzone
            };

            _teamsRepository.Add(newOwner);
        }

        private static async Task PersistTournamentTeamsInRepository(MotorEntitiesResponse tournamentTeamsResponse)
        {
            // TODO
        }

        private static void UpdateDriverStandingInRepo(Player providerStanding, MotorDriverStanding repoStanding)
        {
            repoStanding.Points = providerStanding.points;
            repoStanding.Rank = providerStanding.rank;
            repoStanding.Wins = providerStanding.finishes.first;
            repoStanding.FinishedSecond = providerStanding.finishes.second;
            repoStanding.FinishedThird = providerStanding.finishes.third;
            repoStanding.Top5Finishes = providerStanding.finishes.top5;
            repoStanding.Top10Finishes = providerStanding.finishes.top10;
            repoStanding.Top15Finishes = providerStanding.finishes.top15;
            repoStanding.Top20Finishes = providerStanding.finishes.top20;
            repoStanding.DidNotFinish = providerStanding.finishes.didNotFinish;
            repoStanding.LapsCompleted = providerStanding.laps.completed;
            repoStanding.LapsTotalLed = providerStanding.laps.totalLed;
        }

        private void AddNewDriverStandingToRepo(Player providerPlayerInfo, MotorLeague league)
        {
            var repoDriver = _motorDriverRepository.FirstOrDefault(d => d.ProviderId == providerPlayerInfo.playerId);
            if (repoDriver is null)
            {
                AddNewDriverToRepo(providerPlayerInfo);
                _motorDriverRepository.SaveAsync();
                repoDriver = _motorDriverRepository.FirstOrDefault(d => d.ProviderId == providerPlayerInfo.playerId);
            }

            var standingEntry = new MotorDriverStanding
            {
                MotorLeague = league,
                Points = providerPlayerInfo.points,
                Rank = providerPlayerInfo.rank,
                Wins = providerPlayerInfo.finishes.first,
                FinishedSecond = providerPlayerInfo.finishes.second,
                FinishedThird = providerPlayerInfo.finishes.third,
                Top5Finishes = providerPlayerInfo.finishes.top5,
                Top10Finishes = providerPlayerInfo.finishes.top10,
                Top15Finishes = providerPlayerInfo.finishes.top15,
                Top20Finishes = providerPlayerInfo.finishes.top20,
                DidNotFinish = providerPlayerInfo.finishes.didNotFinish,
                LapsCompleted = providerPlayerInfo.laps.completed,
                LapsTotalLed = providerPlayerInfo.laps.totalLed,
                MotorDriver = repoDriver
            };

            _driverStandingRepository.Add(standingEntry);
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

       

        private static async Task PersistGridInRepository(MotorEntitiesResponse tournamentGrid, CancellationToken cancellationToken)
        {
            // TODO
        }
    }
}