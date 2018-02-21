using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Motor;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.RequestModels;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.UnitOfWork;
using SuperSportDataEngine.Common.Logging;

namespace SuperSportDataEngine.ApplicationLogic.Services
{
    public class MotorIngestWorkerService : IMotorIngestWorkerService
    {
        private readonly IStatsProzoneMotorIngestService _statsProzoneMotorIngestService;
        private readonly IPublicSportDataUnitOfWork _publicSportDataUnitOfWork;
        private readonly IMotorService _motorService;
        private readonly ILoggingService _loggingService;

        public MotorIngestWorkerService(
            IStatsProzoneMotorIngestService statsProzoneMotorIngestService,
            IPublicSportDataUnitOfWork publicSportDataUnitOfWork,
            ILoggingService loggingService,
            IMotorService motorService)
        {
            _statsProzoneMotorIngestService = statsProzoneMotorIngestService;
            _publicSportDataUnitOfWork = publicSportDataUnitOfWork;
            _loggingService = loggingService;
            _motorService = motorService;
        }

        public async Task IngestDriversForActiveTournaments(MotorDriverRequestEntity motorDriverRequestEntity, CancellationToken cancellationToken)
        {
            var tournamentDrivers = _statsProzoneMotorIngestService.IngestTournamentDrivers(motorDriverRequestEntity);
            await PersistTournamentDriversInRepository(tournamentDrivers);
        }

        private async Task IngestDriversForActiveTournaments(CancellationToken cancellationToken)
        {
            var activeLeagues = await _motorService.GetActiveLeagues();

            foreach (var league in activeLeagues)
            {
                var providerSeasonId = await _motorService.GetProviderSeasonIdForLeague(league.Id, cancellationToken);
                var motorDriverRequestEntity = new MotorDriverRequestEntity(league.ProviderSlug, providerSeasonId);
                var tournamentDrivers = _statsProzoneMotorIngestService.IngestTournamentDrivers(motorDriverRequestEntity);
                await PersistTournamentDriversInRepository(tournamentDrivers);
            }
        }

        public async Task IngestTeamsForActiveTournaments(CancellationToken cancellationToken)
        {
            var motorLeagues = _publicSportDataUnitOfWork.MotorLeagues.Where(league => league.IsEnabled);
            if (motorLeagues != null)
            {
                foreach (var league in motorLeagues)
                {
                    if (league.ProviderSlug is null) continue;

                    var tournamentTeams = _statsProzoneMotorIngestService.IngestTournamentTeams(league.ProviderSlug);
                    await PersistTournamentTeamsInRepository(tournamentTeams);
                }
            }
        }

        public async Task IngestOwnersForActiveTournaments(CancellationToken cancellationToken)
        {
            var motorLeagues = _publicSportDataUnitOfWork.MotorLeagues.Where(league => league.IsEnabled);
            if (motorLeagues != null)
            {
                foreach (var league in motorLeagues)
                {
                    if (league.ProviderSlug is null) continue;

                    var owners = _statsProzoneMotorIngestService.IngestTournamentOwners(league.ProviderSlug);
                    await PersistOwnersInRepository(owners);
                }
            }
        }

        public async Task IngestDriverStandingsForActiveTournaments(CancellationToken cancellationToken)
        {
            var motorLeagues = _publicSportDataUnitOfWork.MotorLeagues.Where(race => race.IsEnabled);
            if (motorLeagues != null)
            {
                foreach (var league in motorLeagues)
                {
                    if (league.ProviderSlug is null) continue;

                    var driverStandings = _statsProzoneMotorIngestService.IngestDriverStandings(league.ProviderSlug);
                    await PersistDriverStandingsInRepository(driverStandings, league, cancellationToken);
                }
            }
        }

        public async Task IngestTeamStandingsForActiveTournaments(CancellationToken cancellationToken)
        {
            var motorLeagues = _publicSportDataUnitOfWork.MotorLeagues.Where(league => league.IsEnabled);
            if (motorLeagues != null)
            {
                foreach (var league in motorLeagues)
                {
                    if (league.ProviderSlug is null) continue;

                    var teamStandings = _statsProzoneMotorIngestService.IngestDriverStandings(league.ProviderSlug);
                    await PersistTeamStandingsInRepository(teamStandings, cancellationToken);
                }
            }
        }

        public async Task IngestRacesForActiveTournaments(CancellationToken cancellationToken)
        {
            var motorLeagues = await _motorService.GetActiveLeagues();
            if (motorLeagues != null)
            {
                foreach (var league in motorLeagues)
                {
                    if (league.ProviderSlug is null) continue;

                    var races = _statsProzoneMotorIngestService.IngestTournamentRaces(league.ProviderSlug);
                    await PersistRacesInRepository(races, league, cancellationToken);
                }
            }
        }

        public async Task IngestSchedulesForActiveTournaments(CancellationToken cancellationToken)
        {
            var motorLeagues = _publicSportDataUnitOfWork.MotorLeagues.Where(league => league.IsEnabled);
            if (motorLeagues != null)
            {
                foreach (var league in motorLeagues)
                {
                    if (league.ProviderSlug is null) continue;

                    var schedule =
                        _statsProzoneMotorIngestService.IngestTournamentSchedule(league.ProviderSlug, league.ProviderLeagueId);
                    await PersistScheduleInRepository(schedule, cancellationToken);
                }
            }
        }

        public async Task IngestTournamentResults(MotorResultRequestParams requestParams, CancellationToken cancellationToken)
        {
            var raceResults = _statsProzoneMotorIngestService.IngestTournamentResults(requestParams);

            await PersistResultsInRepository(raceResults, cancellationToken);
        }

        public async Task IngestTournamentGrid(MotorResultRequestParams requestParams, CancellationToken cancellationToken)
        {
            var tournamentGrid = _statsProzoneMotorIngestService.IngestTournamentGrid(requestParams);

            await PersistGridInRepository(tournamentGrid, cancellationToken);
        }

        public async Task IngestLeagues(CancellationToken cancellationToken)
        {
            var leagues = _statsProzoneMotorIngestService.IngestLeagues();

            await PersistLeaguesInRepository(leagues, cancellationToken);
        }

        private async Task PersistTournamentDriversInRepository(MotorEntitiesResponse providerResponse)
        {
            var driversFromProvider = ExtractDriversFromProviderResponse(providerResponse);
            if (driversFromProvider != null)
            {
                foreach (var providerDriver in driversFromProvider)
                {
                    var driverInRepo =
                        _publicSportDataUnitOfWork.MotorDrivers.FirstOrDefault(d => d.ProviderId == providerDriver.playerId);

                    if (driverInRepo is null)
                    {
                        AddNewDriverToRepo(providerDriver);
                    }
                    else
                    {
                        UpdateDriverInRepo(providerDriver, driverInRepo);
                    }
                }

                await _publicSportDataUnitOfWork.SaveChangesAsync();
            }
        }

        private async Task PersistRacesInRepository(MotorEntitiesResponse providerResponse,
            MotorLeague league, CancellationToken cancellationToken)
        {
            var racesFromProvider = ExtractRacesFromProviderResponse(providerResponse);
            if (racesFromProvider is null)
                return;

            foreach (var providerRace in racesFromProvider)
            {
                var raceInRepo = _publicSportDataUnitOfWork.MotorRaces.FirstOrDefault(r => r.LegacyRaceId == providerRace.raceId);
                if (raceInRepo is null)
                {
                    AddNewRaceToRepo(providerRace, league);
                }
                else
                {
                    UpdateRaceInRepo(providerRace, raceInRepo);
                }
            }

            await _publicSportDataUnitOfWork.SaveChangesAsync();
        }

        private async Task PersistLeaguesInRepository(MotorEntitiesResponse providerResponse,
            CancellationToken cancellationToken)
        {
            var leaguesFromProvider = ExtractLeaguesFromProviderResponse(providerResponse);
            if (leaguesFromProvider != null)
            {
                foreach (var leagueFromProvider in leaguesFromProvider)
                {
                    var leagueInRepo =
                        _publicSportDataUnitOfWork.MotorLeagues.FirstOrDefault(l =>
                            l.ProviderLeagueId == leagueFromProvider.league.subLeague.subLeagueId);
                    if (leagueInRepo is null)
                    {
                        AddNewLeagueToRepo(leagueFromProvider);
                    }
                    else
                    {
                        UpdateLeagueInRepo(leagueFromProvider, leagueInRepo);
                    }
                }

                await _publicSportDataUnitOfWork.SaveChangesAsync();
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
                        _publicSportDataUnitOfWork.MotorDriverStandings.FirstOrDefault(s => s.MotorDriver.ProviderId == providerEntry.playerId);
                    if (driverStanding is null)
                    {
                        AddNewDriverStandingToRepo(providerEntry, league);
                    }
                    else
                    {
                        UpdateDriverStandingInRepo(providerEntry, driverStanding);
                    }
                }

                await _publicSportDataUnitOfWork.SaveChangesAsync();
            }
        }

        private async Task PersistTeamStandingsInRepository(MotorEntitiesResponse providerResponse,
            CancellationToken cancellationToken)
        {
            var teams = ExtractTeamStandingsFromProviderResponse(providerResponse);
            if (teams != null)
            {
                foreach (var team in teams)
                {
                    var teamStanding =
                        _publicSportDataUnitOfWork.MotorTeamStandings.FirstOrDefault(s => s.MotorTeam.ProviderTeamId == team.teamId);
                    if (teamStanding is null)
                    {
                        AddNewTeamStandingToRepo(team);
                    }
                    else
                    {
                        UpdateTeamStandingInRepo(team, teamStanding);
                    }
                }

                await _publicSportDataUnitOfWork.SaveChangesAsync();
            }
        }

        private async Task PersistOwnersInRepository(MotorEntitiesResponse response)
        {
            var ownersFromProvider = ExtractOwnersFromProviderResponse(response);

            if (ownersFromProvider is null)
            {
                return;
            }

            foreach (var owner in ownersFromProvider)
            {
                var ownerInRepo = _publicSportDataUnitOfWork.MotortTeams.FirstOrDefault(o => o.ProviderTeamId == owner.ownerId);

                if (ownerInRepo is null)
                {
                    AddNewOwnerToRepo(owner);
                }
                else
                {
                    UpdateOwnerInRepo(ownerInRepo, owner);
                }
            }

            await _publicSportDataUnitOfWork.SaveChangesAsync();

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
                if (playerId is null) continue;

                var resultInRepo =
                    _publicSportDataUnitOfWork.MotorTeamResults.FirstOrDefault(r => r.MotorDriver.ProviderId == playerId);

                if (resultInRepo is null)
                {
                    AddNewResultsToRepo(result);
                }
                else
                {
                    UpdateResultsInRepo(resultInRepo, result);
                }
            }

            await _publicSportDataUnitOfWork.SaveChangesAsync();
        }

        private async Task PersistGridInRepository(MotorEntitiesResponse response, CancellationToken cancellationToken)
        {
            var gridFromProviderResponse = ExtractRaceGridFromProviderResponse(response);

            if (gridFromProviderResponse is null)
            {
                return;
            }

            foreach (var providerGridEntry in gridFromProviderResponse)
            {
                var playerId = providerGridEntry?.player?.playerId;
                if (playerId is null) continue;

                var gridInRepo =
                    _publicSportDataUnitOfWork.MotorGrids.FirstOrDefault(r => r.MotorDriver.ProviderId == playerId);

                if (gridInRepo is null)
                {
                    AddNewGridEntryToRepo(providerGridEntry);
                }
                else
                {
                    UpdateGridEntryInRepo(gridInRepo, providerGridEntry);
                }
            }

            await _publicSportDataUnitOfWork.SaveChangesAsync();
        }

        private async Task PersistScheduleInRepository(MotorEntitiesResponse response, CancellationToken cancellationToken)
        {
            var scheduleFromProviderResponse = ExtractScheduleFromProviderResponse(response);

            if (scheduleFromProviderResponse != null)
                foreach (var providerEvent in scheduleFromProviderResponse)
                {
                    var raceRaceId = providerEvent?.race?.raceId;
                    if (raceRaceId is null) continue;

                    var scheduleInRepo =
                        _publicSportDataUnitOfWork.MotorSchedules.FirstOrDefault(s => s.ProviderRaceId == raceRaceId);

                    if (scheduleInRepo is null)
                    {
                        AddNewScheduleToRepo(providerEvent);
                    }
                    else
                    {
                        UpdateScheduleInRepo(scheduleInRepo, providerEvent);
                    }
                }

            await _publicSportDataUnitOfWork.SaveChangesAsync();
        }

        private void UpdateScheduleInRepo(MotorRaceCalendar raceCalendarInRepo, Event raceEvent)
        {
            if (raceEvent is null || raceEvent.race is null)
                return;

            var date = raceEvent.startDate?.FirstOrDefault(d =>
                string.Equals(d.dateType, "utc", StringComparison.InvariantCultureIgnoreCase))?.full;

            var currentChampionProviderId = raceEvent.champions?.FirstOrDefault(c => c.championType == "current")?.playerId;
            var currentChampionInRepo = _publicSportDataUnitOfWork.MotorDrivers.FirstOrDefault(d => d.ProviderId == currentChampionProviderId);

            var previousChampionProviderId = raceEvent.champions?.FirstOrDefault(c => c.championType == "previous")?.playerId;
            var previousChampionInRepo = _publicSportDataUnitOfWork.MotorDrivers.FirstOrDefault(d => d.ProviderId == previousChampionProviderId);

            raceCalendarInRepo.City = raceEvent.venue.city;
            raceCalendarInRepo.CountryAbbreviation = raceEvent.venue?.country?.abbreviation;
            raceCalendarInRepo.CountryName = raceEvent.venue?.country?.name;
            raceCalendarInRepo.VenueName = raceEvent.venue?.name;
            raceCalendarInRepo.EventName = raceEvent.race.nameFull;
            raceCalendarInRepo.ProviderRaceId = raceEvent.race.raceId;
            raceCalendarInRepo.StartDateTimeUtc = date;

            _publicSportDataUnitOfWork.MotorSchedules.Update(raceCalendarInRepo);
        }

        private void AddNewScheduleToRepo(Event raceEvent)
        {
            if (raceEvent is null || raceEvent.race is null)
                return;

            var date = raceEvent.startDate?.FirstOrDefault(d =>
                string.Equals(d.dateType, "utc", StringComparison.InvariantCultureIgnoreCase))?.full;

            var currentChampionProviderId = raceEvent.champions?.FirstOrDefault(c => c.championType == "current")?.playerId;
            var currentChampionInRepo = _publicSportDataUnitOfWork.MotorDrivers.FirstOrDefault(d => d.ProviderId == currentChampionProviderId);

            var previousChampionProviderId = raceEvent.champions?.FirstOrDefault(c => c.championType == "previous")?.playerId;
            var previousChampionInRepo = _publicSportDataUnitOfWork.MotorDrivers.FirstOrDefault(d => d.ProviderId == previousChampionProviderId);

            var newSchedule = new MotorRaceCalendar
            {
                City = raceEvent.venue.city,
                CountryAbbreviation = raceEvent.venue?.country?.abbreviation,
                CountryName = raceEvent.venue?.country?.name,
                VenueName = raceEvent.venue?.name,
                EventName = raceEvent.race.nameFull,
                ProviderRaceId = raceEvent.race.raceId,
                StartDateTimeUtc = date
            };

            _publicSportDataUnitOfWork.MotorSchedules.Add(newSchedule);
        }

        private void UpdateGridEntryInRepo(MotorGrid gridInRepo, Result providerGridEntry)
        {
            var qualifyingRun = providerGridEntry?.qualifying?.qualifyingRuns?.FirstOrDefault();
            if (qualifyingRun is null) return;

            gridInRepo.Position = providerGridEntry.carPosition.startingPosition;
            gridInRepo.QualifyingTime = new MotorTime
            {
                Minutes = qualifyingRun.time.minutes,
                Seconds = qualifyingRun.time.seconds,
                Milliseconds = qualifyingRun.time.milliseconds
            };

            _publicSportDataUnitOfWork.MotorGrids.Update(gridInRepo);
        }

        private void AddNewGridEntryToRepo(Result providerGridEntry)
        {
            var qualifyingRun = providerGridEntry?.qualifying?.qualifyingRuns?.FirstOrDefault();
            if (qualifyingRun is null) return;

            var driverInRepo = _publicSportDataUnitOfWork.MotorDrivers.FirstOrDefault(d => d.ProviderId == providerGridEntry.player.playerId);
            if (driverInRepo is null) return;

            var newGridEntry = new MotorGrid
            {
                MotorDriver = driverInRepo,
                DriverId = driverInRepo.Id,
                Position = providerGridEntry.carPosition.startingPosition,
                QualifyingTime = new MotorTime
                {
                    Minutes = qualifyingRun.time.minutes,
                    Seconds = qualifyingRun.time.seconds,
                    Milliseconds = qualifyingRun.time.milliseconds
                }
            };

            _publicSportDataUnitOfWork.MotorGrids.Add(newGridEntry);
        }

        private void UpdateResultsInRepo(MotorRaceResult resultInRepo, Result result)
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

            _publicSportDataUnitOfWork.MotorTeamResults.Update(resultInRepo);
        }

        private void AddNewResultsToRepo(Result result)
        {
            var newEntry = new MotorRaceResult
            {
                Position = result.carPosition.position,
                StartingPosition = result.carPosition.startingPosition,

                DriverTotalPoints = int.Parse(result.points.driver.total),
                DriverPenaltyPoints = int.Parse(result.points.driver.penalty),
                DriverBonusPoints = int.Parse(result.points.driver.bonus),

                OwnerTotalPoints = int.Parse(result.points.owner.total),
                OwnerBonusPoints = int.Parse(result.points.owner.bonus),
                OwnerPenaltyPoints = int.Parse(result.points.owner.penalty),

                FinishingTime =
                {
                    Hours = result.finishingTime.hours,
                    Minutes = result.finishingTime.minutes,
                    Seconds = result.finishingTime.seconds
                },

                IsFastest = result.laps.isFastest,
                LapsLed = result.laps.totalLed,
                LapsBehind = result.laps.behind,
                LapsCompleted = result.laps.completed,
            };

            var driver = _publicSportDataUnitOfWork.MotorDrivers.FirstOrDefault(d => d.ProviderId == result.player.playerId);
            newEntry.DriverId = driver.Id;

            _publicSportDataUnitOfWork.MotorTeamResults.Add(newEntry);
        }

        private void UpdateLeagueInRepo(League leagueFromProvider, MotorLeague leagueInRepo)
        {
            leagueInRepo.ProviderSlug = leagueFromProvider.league.subLeague.abbreviation;
            leagueInRepo.Abbreviation = leagueFromProvider.league.subLeague.abbreviation;
            leagueInRepo.Name = leagueFromProvider.league.subLeague.name;
            leagueInRepo.DisplayName = leagueFromProvider.league.subLeague.displayName;

            _publicSportDataUnitOfWork.MotorLeagues.Update(leagueInRepo);
        }

        private void AddNewLeagueToRepo(League leagueFromProvider)
        {
            var league = new MotorLeague
            {
                ProviderSlug = leagueFromProvider.league.subLeague.abbreviation,
                Name = leagueFromProvider.league.subLeague.name,
                DisplayName = leagueFromProvider.league.subLeague.displayName,
                ProviderLeagueId = leagueFromProvider.league.subLeague.subLeagueId,
                Abbreviation = leagueFromProvider.league.subLeague.abbreviation,
                Slug = leagueFromProvider.league.name,
                DataProvider = DataProvider.StatsProzone
            };

            _publicSportDataUnitOfWork.MotorLeagues.Add(league);
        }

        private void UpdateDriverInRepo(Player providerDriver, MotorDriver driverInRepo)
        {
            driverInRepo.FirstName = providerDriver.firstName;
            driverInRepo.LastName = providerDriver.lastName;
            driverInRepo.HeightInCentimeters = providerDriver.height.centimeters;
            driverInRepo.WeightInKilograms = providerDriver.weight.kilograms;
            driverInRepo.DataProvider = DataProvider.StatsProzone;

            _publicSportDataUnitOfWork.MotorDrivers.Update(driverInRepo);
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

            _publicSportDataUnitOfWork.MotorDrivers.Add(driver);
        }

        private void UpdateRaceInRepo(Race providerRace, MotorRace raceInRepo)
        {
            raceInRepo.Name = providerRace.name;
            raceInRepo.Abbreviation = providerRace.name;

            _publicSportDataUnitOfWork.MotorRaces.Update(raceInRepo);
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

            _publicSportDataUnitOfWork.MotorRaces.Add(newRace);
        }

        private void UpdateOwnerInRepo(MotorTeam ownerInRepo, Owner owner)
        {
            if (owner is null || owner.name is null)
                return;

            ownerInRepo.Name = owner.name;

            _publicSportDataUnitOfWork.MotortTeams.Update(ownerInRepo);
        }

        private void AddNewOwnerToRepo(Owner owner)
        {
            if (owner is null || owner.name is null)
                return;

            var newOwner = new MotorTeam
            {
                Name = owner.name,
                ProviderTeamId = owner.ownerId,
                DataProvider = DataProvider.StatsProzone
            };

            _publicSportDataUnitOfWork.MotortTeams.Add(newOwner);
        }

        private void UpdateDriverStandingInRepo(Player providerStanding, MotorDriverStanding repoStanding)
        {
            if (providerStanding is null || providerStanding.finishes is null)
                return;

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

            _publicSportDataUnitOfWork.MotorDriverStandings.Update(repoStanding);
        }

        private void AddNewDriverStandingToRepo(Player providerStanding, MotorLeague league)
        {
            if (providerStanding is null || providerStanding.finishes is null)
                return;

            var repoDriver = _publicSportDataUnitOfWork.MotorDrivers.FirstOrDefault(d => d.ProviderId == providerStanding.playerId);
            if (repoDriver is null)
            {
                AddNewDriverToRepo(providerStanding);
                _publicSportDataUnitOfWork.SaveChangesAsync();
                repoDriver = _publicSportDataUnitOfWork.MotorDrivers.FirstOrDefault(d => d.ProviderId == providerStanding.playerId);
            }

            var standingEntry = new MotorDriverStanding
            {
                MotorLeague = league,
                Points = providerStanding.points,
                Rank = providerStanding.rank,
                Wins = providerStanding.finishes.first,
                FinishedSecond = providerStanding.finishes.second,
                FinishedThird = providerStanding.finishes.third,
                Top5Finishes = providerStanding.finishes.top5,
                Top10Finishes = providerStanding.finishes.top10,
                Top15Finishes = providerStanding.finishes.top15,
                Top20Finishes = providerStanding.finishes.top20,
                DidNotFinish = providerStanding.finishes.didNotFinish,
                LapsCompleted = providerStanding.laps.completed,
                LapsTotalLed = providerStanding.laps.totalLed,
                MotorDriver = repoDriver
            };

            _publicSportDataUnitOfWork.MotorDriverStandings.Add(standingEntry);
        }

        private void UpdateTeamStandingInRepo(Team providerStanding, MotorTeamStanding repoStanding)
        {
            if (providerStanding is null || providerStanding.finishes is null)
                return;

            repoStanding.Wins = providerStanding.finishes.first;
            repoStanding.FinishedSecond = providerStanding.finishes.second;
            repoStanding.FinishedThird = providerStanding.finishes.third;
            repoStanding.Top5Finishes = providerStanding.finishes.top5;
            repoStanding.Top10Finishes = providerStanding.finishes.top10;
            repoStanding.Top15Finishes = providerStanding.finishes.top15;
            repoStanding.Top20Finishes = providerStanding.finishes.top20;
            repoStanding.DidNotFinish = providerStanding.finishes.didNotFinish;
            repoStanding.Points = providerStanding.points;
            repoStanding.Rank = providerStanding.rank;
            repoStanding.Starts = providerStanding.starts;
            repoStanding.Poles = providerStanding.poles;

            _publicSportDataUnitOfWork.MotorTeamStandings.Update(repoStanding);
        }

        private void AddNewTeamStandingToRepo(Team providerTeam)
        {
            if (providerTeam is null || providerTeam.finishes is null)
                return;

            var teamFromRepo = _publicSportDataUnitOfWork.MotortTeams.FirstOrDefault(t => t.ProviderTeamId == providerTeam.teamId);

            if (teamFromRepo is null)
                return;

            var teamStanding = new MotorTeamStanding
            {
                Wins = providerTeam.finishes.first,
                FinishedSecond = providerTeam.finishes.second,
                FinishedThird = providerTeam.finishes.third,
                Top5Finishes = providerTeam.finishes.top5,
                Top10Finishes = providerTeam.finishes.top10,
                Top15Finishes = providerTeam.finishes.top15,
                Top20Finishes = providerTeam.finishes.top20,
                DidNotFinish = providerTeam.finishes.didNotFinish,
                Points = providerTeam.points,
                Rank = providerTeam.rank,
                Starts = providerTeam.starts,
                Poles = providerTeam.poles
            };

            _publicSportDataUnitOfWork.MotorTeamStandings.Add(teamStanding);
        }

        private static IEnumerable<Result> ExtractResultsFromProviderResponse(MotorEntitiesResponse response)
        {
            if (response != null && response.recordCount <= 0)
                return null;

            var result = response
                ?.apiResults.FirstOrDefault()
                ?.leagues.FirstOrDefault()
                ?.league.subLeague
                ?.season
                ?.eventType.FirstOrDefault()
                ?.events.FirstOrDefault()
                ?.boxscore
                ?.results;

            return result;
        }

        private static IEnumerable<Result> ExtractRaceGridFromProviderResponse(MotorEntitiesResponse response)
        {
            return ExtractResultsFromProviderResponse(response);
        }

        private static IEnumerable<Race> ExtractRacesFromProviderResponse(MotorEntitiesResponse response)
        {
            if (response != null && response.recordCount <= 0)
                return null;

            var races = response
                ?.apiResults.FirstOrDefault()
                ?.leagues.FirstOrDefault()
                ?.league.races;

            return races;
        }

        private static IEnumerable<League> ExtractLeaguesFromProviderResponse(MotorEntitiesResponse response)
        {
            if (response != null && response.recordCount <= 0)
                return null;

            var leagues = response
                ?.apiResults.FirstOrDefault()
                ?.leagues;

            return leagues;
        }

        private static IEnumerable<Player> ExtractDriverStandingsFromProviderResponse(MotorEntitiesResponse response)
        {
            if (response != null && response.recordCount <= 0)
                return null;

            var result = response
                ?.apiResults.FirstOrDefault()
                ?.leagues.FirstOrDefault()
                ?.league.subLeague
                ?.season
                ?.standings
                ?.players;

            return result;
        }

        private static IEnumerable<Player> ExtractDriversFromProviderResponse(MotorEntitiesResponse response)
        {
            if (response != null && response.recordCount <= 0)
                return null;

            var players = response
                ?.apiResults
                ?.FirstOrDefault()
                ?.leagues.FirstOrDefault()
                ?.league.subLeague
                ?.players;

            return players;
        }

        private static IEnumerable<Team> ExtractTeamStandingsFromProviderResponse(MotorEntitiesResponse response)
        {
            if (response != null && response.recordCount <= 0)
                return null;

            var teams = response
                ?.apiResults.FirstOrDefault()
                ?.leagues.FirstOrDefault()
                ?.league.subLeague
                ?.season
                ?.standings
                ?.teams;

            return teams;
        }

        private static IEnumerable<Owner> ExtractOwnersFromProviderResponse(MotorEntitiesResponse response)
        {
            if (response != null && response.recordCount <= 0)
                return null;

            var results = response
                ?.apiResults.FirstOrDefault()
                ?.leagues.FirstOrDefault()
                ?.league.subLeague
                ?.owners;

            return results;
        }

        private static IEnumerable<Event> ExtractScheduleFromProviderResponse(MotorEntitiesResponse response)
        {
            if (response != null && response.recordCount <= 0)
                return null;

            var events = response
                ?.apiResults
                ?.FirstOrDefault()
                ?.leagues.FirstOrDefault()
                ?.league.subLeague
                ?.season
                ?.eventType.FirstOrDefault()
                ?.events;

            return events;
        }

        private static async Task PersistTournamentTeamsInRepository(MotorEntitiesResponse response)
        {
            // STATS API does not provide such data under teams end-point. 
            // We Ingest it using their "Owners" end-point.
        }
    }
}