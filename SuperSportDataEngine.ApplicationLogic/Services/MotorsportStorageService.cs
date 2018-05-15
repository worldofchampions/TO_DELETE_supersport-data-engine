namespace SuperSportDataEngine.ApplicationLogic.Services
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.UnitOfWork;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.Stats.Models.Motorsport;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.Stats.Models.Motorsport.Enums;

    public class MotorsportStorageService : IMotorsportStorageService
    {
        private readonly IPublicSportDataUnitOfWork _publicSportDataUnitOfWork;

        public MotorsportStorageService(IPublicSportDataUnitOfWork publicSportDataUnitOfWork)
        {
            _publicSportDataUnitOfWork = publicSportDataUnitOfWork;
        }

        public async Task PersistLeagueDriversInRepository(MotorsportEntitiesResponse providerResponse, MotorsportLeague league)
        {
            var driversFromProvider = ExtractDriversFromProviderResponse(providerResponse);

            if (driversFromProvider != null)
            {
                foreach (var providerDriver in driversFromProvider)
                {
                    var driverInRepo = _publicSportDataUnitOfWork.MotorsportDrivers.FirstOrDefault(d =>
                    d.ProviderDriverId == providerDriver.playerId && d.MotorsportLeague.Id == league.Id);

                    if (driverInRepo is null)
                    {
                        AddNewDriverToRepo(providerDriver, league);
                    }
                    else
                    {
                        UpdateDriverInRepo(providerDriver, driverInRepo, league);
                    }
                }

                await _publicSportDataUnitOfWork.SaveChangesAsync();
            }
        }

        public async Task PersistRacesInRepository(
            MotorsportEntitiesResponse providerResponse,
            MotorsportLeague league,
            CancellationToken cancellationToken)
        {
            var racesFromProvider = ExtractRacesFromProviderResponse(providerResponse);
            if (racesFromProvider == null) return;

            foreach (var providerRace in racesFromProvider)
            {
                var raceInRepo = _publicSportDataUnitOfWork.MotorsportRaces.FirstOrDefault(r =>
                    r.ProviderRaceId == providerRace.raceId && r.MotorsportLeague.Id == league.Id);

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

        public async Task PersistRaceEventsInRepository(
            MotorsportEntitiesResponse providerResponse,
            MotorsportRace race,
            MotorsportSeason season,
            CancellationToken cancellationToken)
        {
            var raceEventsFromProviderResponse = ExtractRaceEventsFromProviderResponse(providerResponse);
            if (raceEventsFromProviderResponse is null) return;

            foreach (var providerRaceEvent in raceEventsFromProviderResponse)
            {
                if (providerRaceEvent.race == null) continue;

                var eventInRepo = _publicSportDataUnitOfWork.MotorsportRaceEvents.FirstOrDefault(r =>
                    r.MotorsportRace.ProviderRaceId == providerRaceEvent.race.raceId
                    && r.MotorsportSeason.Id == season.Id);

                if (eventInRepo is null)
                {
                    AddNewRaceEventToRepo(providerRaceEvent, race, season);
                }
                else
                {
                    UpdateRaceEventInRepo(providerRaceEvent, eventInRepo);
                }
            }

            await _publicSportDataUnitOfWork.SaveChangesAsync();
        }

        public async Task PersistLeaguesInRepository(
            MotorsportEntitiesResponse providerResponse,
            CancellationToken cancellationToken)
        {
            var leaguesFromProvider = ExtractLeaguesFromProviderResponse(providerResponse);
            if (leaguesFromProvider == null) return;

            foreach (var leagueFromProvider in leaguesFromProvider)
            {
                if (leagueFromProvider.league?.subLeague == null) continue;

                var leagueInRepo = _publicSportDataUnitOfWork.MotorsportLeagues.FirstOrDefault(l =>
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

        public async Task PersistSeasonsInRepository(
            MotorsportEntitiesResponse providerResponse,
            MotorsportLeague league,
            CancellationToken cancellationToken)
        {
            var seasonsFromProviderResponse = ExtractSeasonsFromProviderResponse(providerResponse);
            if (seasonsFromProviderResponse == null) return;

            foreach (var providerSeason in seasonsFromProviderResponse)
            {
                var seasonInRepo = _publicSportDataUnitOfWork.MotorsportSeasons.FirstOrDefault(s =>
                    s.ProviderSeasonId == providerSeason.season
                    && s.MotorsportLeague.Id == league.Id);

                if (seasonInRepo is null)
                {
                    AddNewSeasonToRepo(providerSeason, league);
                }
                else
                {
                    UpdateSeasonInRepo(seasonInRepo, providerSeason);
                }
            }

            await _publicSportDataUnitOfWork.SaveChangesAsync();
        }

        public async Task PersistDriverStandingsInRepository(
            MotorsportEntitiesResponse providerResponse,
            MotorsportLeague league,
            MotorsportSeason season,
            CancellationToken cancellationToken)
        {
            var standingsFromProvider = ExtractDriverStandingsFromProviderResponse(providerResponse);
            if (standingsFromProvider == null) return;

            foreach (var providerEntry in standingsFromProvider)
            {
                var driverStanding = _publicSportDataUnitOfWork.MotorsportDriverStandings.FirstOrDefault(s =>
                    s.MotorsportDriver.ProviderDriverId == providerEntry.playerId
                    && s.MotorsportSeasonId == season.Id && s.MotorsportLeagueId == league.Id);

                if (driverStanding is null)
                {
                    await AddNewDriverStandingToRepo(providerEntry, league, season);
                }
                else
                {
                    UpdateDriverStandingInRepo(providerEntry, driverStanding);
                }
            }

            await _publicSportDataUnitOfWork.SaveChangesAsync();
        }

        public async Task PersistTeamsInRepository(MotorsportEntitiesResponse response, MotorsportLeague league)
        {
            var ownersFromProvider = ExtractOwnersFromProviderResponse(response);
            if (ownersFromProvider == null) return;

            foreach (var owner in ownersFromProvider)
            {
                var ownerInRepo =
                    _publicSportDataUnitOfWork.MotortsportTeams.FirstOrDefault(t => t.ProviderTeamId == owner.ownerId && t.MotorsportLeague.Id == league.Id);

                if (ownerInRepo is null)
                {
                    AddNewOwnerToRepo(owner, league);
                }
                else
                {
                    UpdateOwnerInRepo(ownerInRepo, owner);
                }
            }

            await _publicSportDataUnitOfWork.SaveChangesAsync();
        }

        public async Task PersistTeamStandingsInRepository(
            MotorsportEntitiesResponse providerResponse,
            MotorsportLeague league,
            MotorsportSeason season,
            CancellationToken cancellationToken)
        {
            var teamStandingsFromProviderResponse = ExtractTeamStandingsFromProviderResponse(providerResponse);
            if (teamStandingsFromProviderResponse == null) return;

            foreach (var providerStanding in teamStandingsFromProviderResponse)
            {
                var repoStanding = _publicSportDataUnitOfWork.MotorsportTeamStandings.FirstOrDefault(s =>
                    s.MotorsportTeam.ProviderTeamId == providerStanding.teamId
                    && s.MotorsportSeasonId == season.Id && s.MotorsportLeagueId == league.Id);

                if (repoStanding is null)
                {
                    await AddNewTeamStandingToRepo(providerStanding, league, season);
                }
                else
                {
                    UpdateTeamStandingInRepo(providerStanding, repoStanding);
                }
            }

            await _publicSportDataUnitOfWork.SaveChangesAsync();
        }

        public async Task PersistResultsInRepository(MotorsportEntitiesResponse response, MotorsportRaceEvent raceEvent, MotorsportLeague league)
        {
            var resultsFromProviderResponse = ExtractResultsFromProviderResponse(response);
            if (resultsFromProviderResponse is null) return;

            foreach (var result in resultsFromProviderResponse)
            {
                var playerId = result?.player?.playerId;
                if (playerId is null) continue;

                var resultInRepo = _publicSportDataUnitOfWork.MotorsportRaceEventResults.FirstOrDefault(
                    r => r.MotorsportDriver.ProviderDriverId == playerId
                         && r.MotorsportRaceEventId == raceEvent.Id);

                if (resultInRepo is null)
                {
                    AddNewResultsToRepo(result, raceEvent, league);
                }
                else
                {
                    UpdateResultsInRepo(resultInRepo, result, league);
                }
            }

            await _publicSportDataUnitOfWork.SaveChangesAsync();

            await UpdateGapToLeaderTimesForEvent(raceEvent);
        }

        private async Task UpdateGapToLeaderTimesForEvent(MotorsportRaceEvent motorsportRaceEvent)
        {
            var leaderResults =
                _publicSportDataUnitOfWork.MotorsportRaceEventResults.FirstOrDefault(r =>
                r.Position == 1 && r.MotorsportRaceEventId == motorsportRaceEvent.Id);

            if (leaderResults == null) return;

            var leaderTimeCombinedInMilliseconds =
                leaderResults.FinishingTimeHours * 3600000 +
                leaderResults.FinishingTimeMinutes * 60000 +
                leaderResults.FinishingTimeSeconds * 1000 +
                leaderResults.FinishingTimeMilliseconds;

            var followersRaceEventResults =
                _publicSportDataUnitOfWork.MotorsportRaceEventResults.Where(r =>
                    r.Position != 1 && r.MotorsportRaceEventId == motorsportRaceEvent.Id).ToList();

            foreach (var result in followersRaceEventResults)
            {
                var resultTimeCombinedInMilliseconds =
                    result.FinishingTimeHours * 3600000 +
                    result.FinishingTimeMinutes * 60000 +
                    result.FinishingTimeSeconds * 1000 +
                    result.FinishingTimeMilliseconds;

                resultTimeCombinedInMilliseconds = resultTimeCombinedInMilliseconds - leaderTimeCombinedInMilliseconds;

                result.GapToLeaderTimeSeconds = resultTimeCombinedInMilliseconds / 1000;
                resultTimeCombinedInMilliseconds = resultTimeCombinedInMilliseconds % 1000;

                result.GapToLeaderTimeMilliseconds = resultTimeCombinedInMilliseconds;

                _publicSportDataUnitOfWork.MotorsportRaceEventResults.Update(result);
            }

            await _publicSportDataUnitOfWork.SaveChangesAsync();
        }

        public async Task PersistGridInRepository(
            MotorsportEntitiesResponse response,
            MotorsportRaceEvent raceEvent,
            CancellationToken cancellationToken)
        {
            var gridFromProviderResponse = ExtractRaceGridFromProviderResponse(response);
            if (gridFromProviderResponse is null) return;

            foreach (var providerGridEntry in gridFromProviderResponse)
            {
                var playerId = providerGridEntry?.player?.playerId;
                if (playerId is null) continue;

                var gridInRepo = _publicSportDataUnitOfWork.MotorsportRaceEventGrids.FirstOrDefault(
                    g => g.MotorsportDriver.ProviderDriverId == playerId
                         && g.MotorsportRaceEvent.Id == raceEvent.Id);

                if (gridInRepo is null)
                {
                    AddNewGridEntryToRepo(providerGridEntry, raceEvent);
                }
                else
                {
                    UpdateGridEntryInRepo(gridInRepo, providerGridEntry);
                }
            }

            await _publicSportDataUnitOfWork.SaveChangesAsync();
        }

        private void UpdateGridEntryInRepo(MotorsportRaceEventGrid raceEventGridInRepo, Result providerGridEntry)
        {
            if (providerGridEntry.carPosition != null)
            {
                raceEventGridInRepo.GridPosition = providerGridEntry.carPosition.startingPosition;
            }

            var qualifyingRuns = providerGridEntry.qualifying?.qualifyingRuns;
            if (qualifyingRuns == null) return;

            var bestQualifyingRun = providerGridEntry.qualifying?.qualifyingRuns?.FirstOrDefault();
            if (bestQualifyingRun == null) return;

            foreach (var run in qualifyingRuns)
            {
                var bestRunTime = new TimeSpan(0, 0, bestQualifyingRun.time.minutes, bestQualifyingRun.time.seconds,
                    bestQualifyingRun.time.milliseconds);

                var currentRunTime = new TimeSpan(0, 0, run.time.minutes, run.time.seconds, run.time.milliseconds);

                if (bestRunTime.Subtract(currentRunTime).TotalMilliseconds > 0)
                {
                    bestQualifyingRun = run;
                }
            }

            raceEventGridInRepo.QualifyingTimeMinutes = bestQualifyingRun.time.minutes;
            raceEventGridInRepo.QualifyingTimeSeconds = bestQualifyingRun.time.seconds;
            raceEventGridInRepo.QualifyingTimeMilliseconds = bestQualifyingRun.time.milliseconds;

            _publicSportDataUnitOfWork.MotorsportRaceEventGrids.Update(raceEventGridInRepo);
        }

        private void AddNewGridEntryToRepo(Result providerGridEntry, MotorsportRaceEvent raceEvent)
        {
            var driverInRepo =
                _publicSportDataUnitOfWork.MotorsportDrivers.FirstOrDefault(d => d.ProviderDriverId == providerGridEntry.player.playerId);
            if (driverInRepo is null) return;

            var racecEvent =
                _publicSportDataUnitOfWork.MotorsportRaceEvents.FirstOrDefault(e => e.Id == raceEvent.Id);
            if (racecEvent is null) return;

            var newGridEntry = new MotorsportRaceEventGrid
            {
                MotorsportDriver = driverInRepo,
                MotorsportDriverId = driverInRepo.Id,
                GridPosition = providerGridEntry.carPosition.startingPosition,
                MotorsportRaceEventId = racecEvent.Id,
                MotorsportRaceEvent = racecEvent
            };

            if (providerGridEntry.owner != null)
            {
                var teamInRepo =
                    _publicSportDataUnitOfWork.MotortsportTeams.FirstOrDefault(d => d.ProviderTeamId == providerGridEntry.owner.ownerId);

                newGridEntry.MotorsportTeam = teamInRepo;
            }

            var qualifyingRuns = providerGridEntry.qualifying?.qualifyingRuns;
            if (qualifyingRuns != null)
            {
                var bestQualifyingRun = providerGridEntry.qualifying?.qualifyingRuns?.FirstOrDefault();
                foreach (var run in qualifyingRuns)
                {
                    if (bestQualifyingRun == null) continue;

                    var bestRunTime = new TimeSpan(0, 0, bestQualifyingRun.time.minutes, bestQualifyingRun.time.seconds,
                        bestQualifyingRun.time.milliseconds);

                    var currentRunTime = new TimeSpan(0, 0, run.time.minutes, run.time.seconds, run.time.milliseconds);

                    if (bestRunTime.Subtract(currentRunTime).TotalMilliseconds > 0)
                    {
                        bestQualifyingRun = run;
                    }
                }

                if (bestQualifyingRun != null)
                {
                    newGridEntry.QualifyingTimeMinutes = bestQualifyingRun.time.minutes;
                    newGridEntry.QualifyingTimeSeconds = bestQualifyingRun.time.seconds;
                    newGridEntry.QualifyingTimeMilliseconds = bestQualifyingRun.time.milliseconds;
                }
            }

            _publicSportDataUnitOfWork.MotorsportRaceEventGrids.Add(newGridEntry);
        }

        private void UpdateResultsInRepo(MotorsportRaceEventResult eventResultInRepo, Result result, MotorsportLeague league)
        {
            if (result.points?.driver?.total != null)
            {
                eventResultInRepo.Points = int.Parse(result.points.driver.total);
            }

            if (result.finishingTime != null)
            {
                eventResultInRepo.FinishingTimeHours = result.finishingTime.hours;
                eventResultInRepo.FinishingTimeMinutes = result.finishingTime.minutes;
                eventResultInRepo.FinishingTimeSeconds = result.finishingTime.seconds;
                eventResultInRepo.FinishingTimeMilliseconds = result.finishingTime.milliseconds;
            }

            if (result.carPosition != null)
            {
                eventResultInRepo.Position = result.carPosition.position;
                eventResultInRepo.GridPosition = result.carPosition.startingPosition;

                if (result.carPosition.position == 1)
                {
                    var raceEvent = _publicSportDataUnitOfWork.MotorsportRaceEvents.FirstOrDefault(e =>
                        e.Id == eventResultInRepo.MotorsportRaceEventId);

                    AssignRaceEventWinner(raceEvent, eventResultInRepo.MotorsportDriver);
                }
            }

            if (result.carStatus != null)
            {
                eventResultInRepo.OutReason = result.carStatus.name;
                eventResultInRepo.CompletedRace = result.carStatus.carStatusId.Equals(0);
            }

            if (result.owner != null)
            {
                var teamInRepo =
                    _publicSportDataUnitOfWork.MotortsportTeams.FirstOrDefault(t =>
                        t.ProviderTeamId == result.owner.ownerId && t.MotorsportLeague.Id == league.Id);

                if (teamInRepo != null)
                    eventResultInRepo.MotorsportTeam = teamInRepo;
            }

            if (result.laps?.completed != null)
            {
                eventResultInRepo.LapsCompleted = result.laps.completed.Value;
            }

            if (result.laps?.behind != null)
            {
                eventResultInRepo.LapsBehind = result.laps.behind.Value;
            }

            _publicSportDataUnitOfWork.MotorsportRaceEventResults.Update(eventResultInRepo);
        }

        private void AddNewResultsToRepo(Result result, MotorsportRaceEvent raceEvent, MotorsportLeague league)
        {
            if (result.player == null) return;

            var driver =
                _publicSportDataUnitOfWork.MotorsportDrivers.FirstOrDefault(d => d.ProviderDriverId == result.player.playerId);
            if (driver is null) return;

            var motorsportRaceResult = new MotorsportRaceEventResult
            {
                MotorsportDriver = driver,
                MotorsportDriverId = driver.Id,
                MotorsportRaceEventId = raceEvent.Id
            };

            if (result.carPosition != null)
            {
                motorsportRaceResult.Position = result.carPosition.position;
                motorsportRaceResult.GridPosition = result.carPosition.startingPosition;

                if (result.carPosition.position == 1)
                {
                    AssignRaceEventWinner(raceEvent, driver);
                }
            }

            if (result.carStatus != null)
            {
                motorsportRaceResult.OutReason = result.carStatus.name;
                motorsportRaceResult.CompletedRace = result.carStatus.carStatusId.Equals(0);
            }

            if (result.points.driver.total != null)
            {
                motorsportRaceResult.Points = int.Parse(result.points.driver.total);
            }

            if (result.owner != null)
            {
                var teamInRepo =
                    _publicSportDataUnitOfWork.MotortsportTeams.FirstOrDefault(t =>
                        t.ProviderTeamId == result.owner.ownerId && t.MotorsportLeague.Id == league.Id);

                if (teamInRepo != null)
                    motorsportRaceResult.MotorsportTeam = teamInRepo;
            }

            if (result.laps?.completed != null)
            {
                motorsportRaceResult.LapsCompleted = (int)result.laps.completed;
            }

            if (result.finishingTime != null)
            {
                motorsportRaceResult.FinishingTimeHours = result.finishingTime.hours;
                motorsportRaceResult.FinishingTimeMinutes = result.finishingTime.minutes;
                motorsportRaceResult.FinishingTimeSeconds = result.finishingTime.seconds;
                motorsportRaceResult.FinishingTimeMilliseconds = result.finishingTime.milliseconds;
            }

            if (result.laps?.completed != null)
            {
                motorsportRaceResult.LapsCompleted = result.laps.completed.Value;
            }

            if (result.laps?.behind != null)
            {
                motorsportRaceResult.LapsBehind = result.laps.behind.Value;
            }

            _publicSportDataUnitOfWork.MotorsportRaceEventResults.Add(motorsportRaceResult);

        }

        private void AssignRaceEventWinner(MotorsportRaceEvent raceEvent, MotorsportDriver driver)
        {
            raceEvent.RaceEventWinner = driver;

            _publicSportDataUnitOfWork.MotorsportRaceEvents.Update(raceEvent);
        }

        private void UpdateLeagueInRepo(League leagueFromProvider, MotorsportLeague leagueInRepo)
        {
            if (leagueFromProvider.league == null) return;

            leagueInRepo.ProviderSlug = leagueFromProvider.league.uriPaths?.FirstOrDefault()?.path;
            if (leagueFromProvider.league?.subLeague != null)
            {
                leagueInRepo.Name = leagueFromProvider.league.subLeague.name;
            }

            var sportType = GetSportTypeForLeague(leagueInRepo.ProviderSlug);
            leagueInRepo.MotorsportSportType = sportType;

            _publicSportDataUnitOfWork.MotorsportLeagues.Update(leagueInRepo);
        }

        private void AddNewLeagueToRepo(League leagueFromProvider)
        {
            if (leagueFromProvider.league?.uriPaths == null) return;

            if (leagueFromProvider.league?.subLeague == null) return;

            var league = new MotorsportLeague
            {
                ProviderSlug = leagueFromProvider.league.uriPaths.FirstOrDefault()?.path,
                Slug = leagueFromProvider.league.uriPaths.FirstOrDefault()?.path,
                Name = leagueFromProvider.league.subLeague.name,
                ProviderLeagueId = leagueFromProvider.league.subLeague.subLeagueId,
                DataProvider = DataProvider.Stats
            };

            var sportType = GetSportTypeForLeague(league.ProviderSlug);
            league.MotorsportSportType = sportType;

            _publicSportDataUnitOfWork.MotorsportLeagues.Add(league);
        }

        private static MotorsportSportType GetSportTypeForLeague(string providerSlug)
        {
            if (string.IsNullOrEmpty(providerSlug)) return MotorsportSportType.MotorsportSportGeneral;

            if (providerSlug.Equals("f1")) return MotorsportSportType.FormulaOne;

            if (providerSlug.Equals("superbike")) return MotorsportSportType.Superbike;

            if (providerSlug.Equals("motogp")) return MotorsportSportType.MotoGp;

            return MotorsportSportType.MotorsportSportGeneral;
        }

        private void UpdateDriverInRepo(Player providerDriver, MotorsportDriver driverInRepo, MotorsportLeague league)
        {
            driverInRepo.FirstName = providerDriver.firstName;
            driverInRepo.LastName = providerDriver.lastName;
            driverInRepo.DataProvider = DataProvider.Stats;
            driverInRepo.MotorsportLeague = league;

            if (providerDriver.car != null)
            {
                if (providerDriver.car.make != null)
                {
                    driverInRepo.ProviderCarId = providerDriver.car.make.makeId;
                }

                if (providerDriver.car.carNumber != null)
                {
                    driverInRepo.CarNumber = providerDriver.car.carNumber;
                }
            }

            if (providerDriver.birth?.country != null)
            {
                driverInRepo.CountryName = providerDriver.birth.country.name;
            }

            if (providerDriver.owner != null)
            {
                var playerTeam = _publicSportDataUnitOfWork.MotortsportTeams.FirstOrDefault(t =>
                    t.ProviderTeamId == providerDriver.owner.ownerId && t.MotorsportLeague.Id == league.Id);

                driverInRepo.MotorsportTeam = playerTeam;
            }

            _publicSportDataUnitOfWork.MotorsportDrivers.Update(driverInRepo);
        }

        private void AddNewDriverToRepo(Player providerDriver, MotorsportLeague league)
        {
            var newMotorsportDriver = new MotorsportDriver
            {
                ProviderDriverId = providerDriver.playerId,
                FirstName = providerDriver.firstName,
                LastName = providerDriver.lastName,
                MotorsportLeague = league,
                DataProvider = DataProvider.Stats
            };

            if (providerDriver.car != null)
            {
                if (providerDriver.car.make != null)
                {
                    newMotorsportDriver.ProviderCarId = providerDriver.car.make.makeId;
                }

                if (providerDriver.car.carNumber != null)
                {
                    newMotorsportDriver.CarNumber = providerDriver.car.carNumber;
                }
            }

            if (providerDriver.birth?.country != null)
            {
                newMotorsportDriver.CountryName = providerDriver.birth.country.name;
            }

            if (providerDriver.owner != null)
            {
                var playerTeam = _publicSportDataUnitOfWork.MotortsportTeams.FirstOrDefault(t =>
                    t.ProviderTeamId == providerDriver.owner.ownerId && t.MotorsportLeague.Id == league.Id);

                newMotorsportDriver.MotorsportTeam = playerTeam;
            }

            _publicSportDataUnitOfWork.MotorsportDrivers.Add(newMotorsportDriver);
        }

        private void UpdateRaceInRepo(Race providerRace, MotorsportRace raceInRepo)
        {
            raceInRepo.RaceName = providerRace.name;
            raceInRepo.RaceNameAbbreviation = providerRace.name;
            raceInRepo.DataProvider = DataProvider.Stats;

            _publicSportDataUnitOfWork.MotorsportRaces.Update(raceInRepo);
        }

        private void AddNewRaceToRepo(Race providerRace, MotorsportLeague motorsportLeague)
        {
            var newRace = new MotorsportRace
            {
                ProviderRaceId = providerRace.raceId,
                RaceName = providerRace.name,
                RaceNameAbbreviation = providerRace.name,
                MotorsportLeague = motorsportLeague,
                DataProvider = DataProvider.Stats
            };

            _publicSportDataUnitOfWork.MotorsportRaces.Add(newRace);
        }

        private void UpdateRaceEventInRepo(Event providerRaceEvent, MotorsportRaceEvent eventInRepo)
        {
            if (providerRaceEvent.venue != null)
            {
                eventInRepo.CityName = providerRaceEvent.venue.city;
                eventInRepo.CircuitName = providerRaceEvent.venue.name;
            }

            if (providerRaceEvent.venue?.country != null)
            {
                eventInRepo.CountryName = providerRaceEvent.venue.country.name;
                eventInRepo.CountryAbbreviation = providerRaceEvent.venue.country.abbreviation;
            }

            var providerStartDate =
                providerRaceEvent.startDate.FirstOrDefault(d => d.dateType.ToLowerInvariant().Equals("utc"));

            if (providerStartDate != null)
            {
                var dateTimeOffset = DateTimeOffset.Parse(providerStartDate.full.ToString(CultureInfo.InvariantCulture) + "Z");

                eventInRepo.StartDateTimeUtc = dateTimeOffset;
            }

            var raceEventStatus = MapProviderRaceEventStatusToInternal(providerRaceEvent.eventStatus.eventStatusId);
            eventInRepo.MotorsportRaceEventStatus = raceEventStatus;

            var winner = providerRaceEvent.champions?.FirstOrDefault(c => c.championType.ToLowerInvariant().Equals("current"));

            if (winner != null)
            {
                var winnerDetails =
                    _publicSportDataUnitOfWork.MotorsportDrivers.FirstOrDefault(d =>
                        d.ProviderDriverId == winner.playerId && d.MotorsportLeague.Id == eventInRepo.MotorsportRace.MotorsportLeague.Id);

                eventInRepo.RaceEventWinner = winnerDetails;
            }

            _publicSportDataUnitOfWork.MotorsportRaceEvents.Update(eventInRepo);
        }

        private void AddNewRaceEventToRepo(Event providerRaceEvent, MotorsportRace race, MotorsportSeason season)
        {
            var motorsportRaceEvent = new MotorsportRaceEvent
            {
                MotorsportRace = race,
                MotorsportSeason = season,
                ProviderRaceEventId = providerRaceEvent.eventId
            };

            if (providerRaceEvent.venue != null)
            {
                motorsportRaceEvent.CityName = providerRaceEvent.venue.city;
                motorsportRaceEvent.CircuitName = providerRaceEvent.venue.name;
            }

            if (providerRaceEvent.venue?.country != null)
            {
                motorsportRaceEvent.CountryName = providerRaceEvent.venue.country.name;
                motorsportRaceEvent.CountryAbbreviation = providerRaceEvent.venue.country.abbreviation;
            }

            var providerStartDate =
                providerRaceEvent.startDate.FirstOrDefault(d => d.dateType.ToLowerInvariant().Equals("utc"));

            if (providerStartDate != null)
            {
                var dateTimeOffset = DateTimeOffset.Parse(providerStartDate.full.ToString(CultureInfo.InvariantCulture) + "Z");

                motorsportRaceEvent.StartDateTimeUtc = dateTimeOffset;
            }

            var raceEventStatus = MapProviderRaceEventStatusToInternal(providerRaceEvent.eventStatus.eventStatusId);
            motorsportRaceEvent.MotorsportRaceEventStatus = raceEventStatus;

            var winner = providerRaceEvent.champions?.FirstOrDefault(c => c.championType.ToLowerInvariant().Equals("current"));

            if (winner != null)
            {
                var winnerDetails =
                    _publicSportDataUnitOfWork.MotorsportDrivers.FirstOrDefault(d =>
                        d.ProviderDriverId == winner.playerId && d.MotorsportLeague.Id == race.MotorsportLeague.Id);

                motorsportRaceEvent.RaceEventWinner = winnerDetails;
            }

            _publicSportDataUnitOfWork.MotorsportRaceEvents.Add(motorsportRaceEvent);
        }

        private void UpdateOwnerInRepo(MotorsportTeam ownerInRepo, Owner owner)
        {
            if (owner is null || owner.name is null)
                return;

            ownerInRepo.Name = owner.name;

            _publicSportDataUnitOfWork.MotortsportTeams.Update(ownerInRepo);
        }

        private void AddNewOwnerToRepo(Owner owner, MotorsportLeague motorsportLeague)
        {
            if (owner is null || owner.name is null)
                return;

            var motorsportTeam = new MotorsportTeam
            {
                Name = owner.name,
                ProviderTeamId = owner.ownerId,
                MotorsportLeague = motorsportLeague,
                DataProvider = DataProvider.Stats
            };

            _publicSportDataUnitOfWork.MotortsportTeams.Add(motorsportTeam);
        }

        private void AddNewTeamToRepo(Team team, MotorsportLeague motorsportLeague)
        {
            if (team is null || team.name is null)
                return;

            var motorsportTeam = new MotorsportTeam
            {
                Name = team.name,
                ProviderTeamId = team.teamId,
                MotorsportLeague = motorsportLeague,
                DataProvider = DataProvider.Stats
            };

            _publicSportDataUnitOfWork.MotortsportTeams.Add(motorsportTeam);
        }

        private void UpdateDriverStandingInRepo(Player providerStanding, MotorsportDriverStanding repoStanding)
        {
            if (providerStanding != null)
            {
                repoStanding.Points = providerStanding.points;
                repoStanding.Position = providerStanding.rank;

                if (providerStanding.finishes != null)
                {
                    repoStanding.Wins = providerStanding.finishes.first;
                }
            }

            _publicSportDataUnitOfWork.MotorsportDriverStandings.Update(repoStanding);
        }

        private async Task AddNewDriverStandingToRepo(Player providerStanding, MotorsportLeague league, MotorsportSeason season)
        {
            if (providerStanding is null)
                return;

            var repoDriver = _publicSportDataUnitOfWork.MotorsportDrivers.FirstOrDefault(d =>
                d.ProviderDriverId == providerStanding.playerId && d.MotorsportLeague.Id == league.Id);

            if (repoDriver is null)
            {
                AddNewDriverToRepo(providerStanding, league);

                await _publicSportDataUnitOfWork.SaveChangesAsync();

                repoDriver = _publicSportDataUnitOfWork.MotorsportDrivers.FirstOrDefault(d =>
                    d.ProviderDriverId == providerStanding.playerId && d.MotorsportLeague.Id == league.Id);
            }

            var standingEntry = new MotorsportDriverStanding
            {
                Points = providerStanding.points,
                Position = providerStanding.rank,
                MotorsportDriverId = repoDriver.Id,
                MotorsportDriver = repoDriver,
                MotorsportSeason = season,
                MotorsportSeasonId = season.Id,
                MotorsportLeagueId = league.Id
            };

            if (providerStanding.finishes != null)
            {
                standingEntry.Wins = providerStanding.finishes.first;
            }

            if (providerStanding.owner != null)
            {
                var repoTeam = _publicSportDataUnitOfWork.MotortsportTeams.FirstOrDefault(t =>
                    t.ProviderTeamId == providerStanding.owner.ownerId && t.MotorsportLeague.Id == league.Id);

                if (repoTeam is null)
                {
                    AddNewOwnerToRepo(providerStanding.owner, league);

                    await _publicSportDataUnitOfWork.SaveChangesAsync();

                    repoTeam = _publicSportDataUnitOfWork.MotortsportTeams.FirstOrDefault(t =>
                        t.ProviderTeamId == providerStanding.owner.ownerId && t.MotorsportLeague.Id == league.Id);
                }

                standingEntry.MotorsportTeam = repoTeam;
            }

            _publicSportDataUnitOfWork.MotorsportDriverStandings.Add(standingEntry);
        }

        private void UpdateTeamStandingInRepo(Team providerStanding, MotorsportTeamStanding repoStanding)
        {
            if (providerStanding is null) return;

            repoStanding.Points = providerStanding.points;
            repoStanding.Position = providerStanding.rank;

            _publicSportDataUnitOfWork.MotorsportTeamStandings.Update(repoStanding);
        }

        private async Task AddNewTeamStandingToRepo(Team providerTeam, MotorsportLeague league, MotorsportSeason season)
        {
            if (providerTeam is null) return;

            var teamFromRepo = _publicSportDataUnitOfWork.MotortsportTeams.FirstOrDefault(t =>
                t.ProviderTeamId == providerTeam.teamId && t.MotorsportLeague.Id == league.Id);

            if (teamFromRepo is null)
            {
                AddNewTeamToRepo(providerTeam, league);

                await _publicSportDataUnitOfWork.SaveChangesAsync();

                teamFromRepo = _publicSportDataUnitOfWork.MotortsportTeams.FirstOrDefault(t =>
                    t.ProviderTeamId == providerTeam.teamId && t.MotorsportLeague.Id == league.Id);
            }

            if (teamFromRepo is null) return;
            if (league is null) return;
            if (season is null) return;

            var teamStanding = new MotorsportTeamStanding
            {
                MotorsportTeamId = teamFromRepo.Id,
                MotorsportLeagueId = league.Id,
                MotorsportSeasonId = season.Id,
                Position = providerTeam.rank,
                Points = providerTeam.points,
                MotorsportSeason = season,
                MotorsportTeam = teamFromRepo
            };

            _publicSportDataUnitOfWork.MotorsportTeamStandings.Add(teamStanding);
        }

        private void UpdateSeasonInRepo(MotorsportSeason seasonInRepo, Season providerSeason)
        {
            if (providerSeason is null) return;

            seasonInRepo.Name = providerSeason.name;
            seasonInRepo.IsActive = providerSeason.isActive;
            seasonInRepo.DataProvider = DataProvider.Stats;

            var providerStartDate =
                providerSeason.eventType.FirstOrDefault()?.startDate.full;

            if (providerStartDate != null) seasonInRepo.StartDateTime = (DateTimeOffset)providerStartDate;

            var providerEndDate =
                providerSeason.eventType.FirstOrDefault()?.endDate.full;
            if (providerEndDate != null) seasonInRepo.EndDateTime = (DateTimeOffset)providerEndDate;

            _publicSportDataUnitOfWork.MotorsportSeasons.Update(seasonInRepo);
        }

        private void AddNewSeasonToRepo(Season season, MotorsportLeague league)
        {
            if (season is null) return;

            var motorsportSeason = new MotorsportSeason
            {
                Name = season.name,
                IsActive = season.isActive,
                ProviderSeasonId = season.season,
                MotorsportLeague = league,
                DataProvider = DataProvider.Stats
            };

            var providerStartDate =
                season.eventType.FirstOrDefault()?.startDate.full;

            if (providerStartDate != null) motorsportSeason.StartDateTime = (DateTimeOffset)providerStartDate;

            var providerEndDate =
                season.eventType.FirstOrDefault()?.endDate.full;
            if (providerEndDate != null) motorsportSeason.EndDateTime = (DateTimeOffset)providerEndDate;

            _publicSportDataUnitOfWork.MotorsportSeasons.Add(motorsportSeason);
        }

        private static IEnumerable<Season> ExtractSeasonsFromProviderResponse(MotorsportEntitiesResponse providerResponse)
        {
            if (providerResponse != null && providerResponse.recordCount <= 0)
                return null;

            var seasons = providerResponse
                ?.apiResults.FirstOrDefault()
                ?.league
                .seasons;

            return seasons;
        }

        private static IEnumerable<Result> ExtractResultsFromProviderResponse(MotorsportEntitiesResponse response)
        {
            if (response != null && response.recordCount <= 0)
                return null;

            var result = response
                ?.apiResults.FirstOrDefault()
                ?.league.subLeague
                ?.season
                ?.eventType.FirstOrDefault()
                ?.events.FirstOrDefault()
                ?.boxscore
                ?.results;

            return result;
        }

        private static IEnumerable<Result> ExtractRaceGridFromProviderResponse(MotorsportEntitiesResponse response)
        {
            return ExtractResultsFromProviderResponse(response);
        }

        private static IEnumerable<Race> ExtractRacesFromProviderResponse(MotorsportEntitiesResponse response)
        {
            if (response != null && response.recordCount <= 0)
                return null;

            var races = response
                ?.apiResults.FirstOrDefault()
                ?.league
                .races;

            return races;
        }

        private static IEnumerable<Event> ExtractRaceEventsFromProviderResponse(MotorsportEntitiesResponse response)
        {
            if (response != null && response.recordCount <= 0)
                return null;

            var races = response
                ?.apiResults.FirstOrDefault()
                ?.league
                .subLeague
                .season
                .eventType.FirstOrDefault()
                ?.events;

            return races;
        }

        private static IEnumerable<League> ExtractLeaguesFromProviderResponse(MotorsportEntitiesResponse response)
        {
            if (response != null && response.recordCount <= 0)
                return null;

            var leagues = response
                ?.apiResults.FirstOrDefault()
                ?.leagues;

            return leagues;
        }

        private static IEnumerable<Player> ExtractDriverStandingsFromProviderResponse(MotorsportEntitiesResponse response)
        {
            if (response != null && response.recordCount <= 0)
                return null;

            var result = response
                ?.apiResults.FirstOrDefault()
                ?.league
                .subLeague
                ?.season
                ?.standings
                ?.players;

            return result;
        }

        private static IEnumerable<Player> ExtractDriversFromProviderResponse(MotorsportEntitiesResponse response)
        {
            if (response != null && response.recordCount <= 0)
                return null;

            var players = response
                ?.apiResults
                ?.FirstOrDefault()
                ?.league.subLeague
                ?.players;

            return players;
        }

        private static IEnumerable<Team> ExtractTeamStandingsFromProviderResponse(MotorsportEntitiesResponse response)
        {
            if (response != null && response.recordCount <= 0)
                return null;

            var teams = response
                ?.apiResults.FirstOrDefault()
                ?.league.subLeague
                ?.season
                ?.standings
                ?.teams;

            return teams;
        }

        private static IEnumerable<Owner> ExtractOwnersFromProviderResponse(MotorsportEntitiesResponse response)
        {
            if (response != null && response.recordCount <= 0)
                return null;

            var results = response
                ?.apiResults.FirstOrDefault()
                ?.league
                .subLeague
                .season
                .owners;

            return results;
        }

        private static MotorsportRaceEventStatus MapProviderRaceEventStatusToInternal(MotorsportRaceEventStatusProvider providerStatus)
        {
            switch (providerStatus)
            {
                case MotorsportRaceEventStatusProvider.PreRace:
                    return MotorsportRaceEventStatus.PreRace;
                case MotorsportRaceEventStatusProvider.InProgress:
                    return MotorsportRaceEventStatus.InProgress;
                case MotorsportRaceEventStatusProvider.Postponed:
                    return MotorsportRaceEventStatus.Postponed;
                case MotorsportRaceEventStatusProvider.Suspended:
                    return MotorsportRaceEventStatus.Suspended;
                case MotorsportRaceEventStatusProvider.Delayed:
                    return MotorsportRaceEventStatus.Delayed;
                case MotorsportRaceEventStatusProvider.Cancelled:
                    return MotorsportRaceEventStatus.Cancelled;
                case MotorsportRaceEventStatusProvider.Final:
                    return MotorsportRaceEventStatus.Result;
                default:
                    return MotorsportRaceEventStatus.Unknown;
            }
        }
    }
}