using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Motorsport;

namespace SuperSportDataEngine.ApplicationLogic.Services
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
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

    public class MotorsportIngestWorkerService : IMotorsportIngestWorkerService
    {
        private readonly IStatsProzoneMotorIngestService _statsProzoneMotorIngestService;
        private readonly IPublicSportDataUnitOfWork _publicSportDataUnitOfWork;
        private readonly IMotorsportService _motorsportService;
        private readonly ILoggingService _loggingService;

        public MotorsportIngestWorkerService(
            IStatsProzoneMotorIngestService statsProzoneMotorIngestService,
            IPublicSportDataUnitOfWork publicSportDataUnitOfWork,
            ILoggingService loggingService,
            IMotorsportService motorsportService)
        {
            _statsProzoneMotorIngestService = statsProzoneMotorIngestService;
            _publicSportDataUnitOfWork = publicSportDataUnitOfWork;
            _loggingService = loggingService;
            _motorsportService = motorsportService;
        }

        public async Task IngestSeasons(CancellationToken cancellationToken)
        {
            var motorsportLeagues = await _motorsportService.GetActiveLeagues();

            foreach (var league in motorsportLeagues)
            {
                var leagues = _statsProzoneMotorIngestService.IngestLeagueSeasons(league.ProviderSlug);

                await PersistSeasonsInRepository(leagues, league, cancellationToken);
            }
        }

        public async Task IngestDriversForActiveLeagues(CancellationToken cancellationToken)
        {
            var activeLeagues = await _motorsportService.GetActiveLeagues();

            foreach (var league in activeLeagues)
            {
                var providerSeasonId = await _motorsportService.GetProviderSeasonIdForLeague(league.Id, cancellationToken);

                var leagueDrivers = _statsProzoneMotorIngestService.IngestDriversForLeague(league.ProviderSlug, providerSeasonId);

                await PersistLeagueDriversInRepository(leagueDrivers);
            }

        }

        public async Task IngestTeamsForActiveLeagues(CancellationToken cancellationToken)
        {
            var motorLeagues = _publicSportDataUnitOfWork.MotorsportLeagues.Where(league => league.IsEnabled).ToList();

            foreach (var league in motorLeagues)
            {
                if (league.ProviderSlug is null) continue;

                var teams = _statsProzoneMotorIngestService.IngestTeamsForLeague(league.ProviderSlug);

                await PersistTeamsInRepository(teams);
            }
        }

        public async Task IngestDriverStandingsForActiveLeagues(CancellationToken cancellationToken)
        {
            var motorLeagues = _publicSportDataUnitOfWork.MotorsportLeagues.Where(race => race.IsEnabled).ToList();

            foreach (var league in motorLeagues)
            {
                if (league.ProviderSlug is null) continue;

                var season = await _motorsportService.GetCurrentSeasonForLeague(league.Id, cancellationToken);

                var driverStandings = _statsProzoneMotorIngestService.IngestDriverStandings(league.ProviderSlug, season.ProviderSeasonId);

                await PersistDriverStandingsInRepository(driverStandings, league, season, cancellationToken);
            }
        }

        public async Task IngestTeamStandingsForActiveLeagues(CancellationToken cancellationToken)
        {
            var motorLeagues = _publicSportDataUnitOfWork.MotorsportLeagues.Where(league => league.IsEnabled).ToList();

            foreach (var league in motorLeagues)
            {
                if (league.ProviderSlug is null) continue;

                var season = await _motorsportService.GetCurrentSeasonForLeague(league.Id, cancellationToken);

                var teamStandings = _statsProzoneMotorIngestService.IngestTeamStandings(league.ProviderSlug, season.ProviderSeasonId);

                await PersistTeamStandingsInRepository(teamStandings, league, season, cancellationToken);
            }
        }

        public async Task IngestRacesForActiveLeagues(CancellationToken cancellationToken)
        {
            var motorLeagues = await _motorsportService.GetActiveLeagues();

            if (motorLeagues != null)
            {
                foreach (var league in motorLeagues)
                {
                    if (league.ProviderSlug is null) continue;

                    var providerSeasonId = await _motorsportService.GetProviderSeasonIdForLeague(league.Id, cancellationToken);

                    var races = _statsProzoneMotorIngestService.IngestLeagueRaces(league.ProviderSlug, providerSeasonId);

                    await PersistRacesInRepository(races, league, cancellationToken);
                }
            }
        }

        public async Task IngestResultsForActiveLeagues(CancellationToken cancellationToken)
        {
            var activeLeagues = await _motorsportService.GetActiveLeagues();

            foreach (var league in activeLeagues)
            {
                var season = await _motorsportService.GetCurrentSeasonForLeague(league.Id, cancellationToken);

                var motorsportRaces = await _motorsportService.GetRacesForLeague(league.Id);

                foreach (var race in motorsportRaces)
                {
                    var raceResults =
                        _statsProzoneMotorIngestService.IngestRaceResults(league.Slug, season.ProviderSeasonId, race.ProviderRaceId);

                    await PersistResultsInRepository(raceResults, race, cancellationToken);
                }
            }
        }

        public async Task IngestHistoricRaceEvents(CancellationToken cancellationToken)
        {
            var motorLeagues = await _motorsportService.GetActiveLeagues();

            if (motorLeagues != null)
            {
                foreach (var league in motorLeagues)
                {
                    if (league.ProviderSlug is null) continue;

                    var leagueRaces = await _motorsportService.GetRacesForLeague(league.Id);

                    var season = await _motorsportService.GetPastSeasonForLeague(league.Id, cancellationToken);

                    foreach (var race in leagueRaces)
                    {
                        var calendar =
                            _statsProzoneMotorIngestService.IngestLeagueCalendar(
                                league.ProviderSlug, season.ProviderSeasonId, race.ProviderRaceId);

                        await PersistCalendarInRepository(calendar, race, season, cancellationToken);
                    }
                }
            }
        }

        public async Task IngestRacesEvents(CancellationToken cancellationToken)
        {
            var motorLeagues = await _motorsportService.GetActiveLeagues();

            if (motorLeagues != null)
            {
                foreach (var league in motorLeagues)
                {
                    if (league.ProviderSlug is null) continue;

                    var leagueRaces = await _motorsportService.GetRacesForLeague(league.Id);

                    var season = await _motorsportService.GetCurrentSeasonForLeague(league.Id, cancellationToken);

                    foreach (var race in leagueRaces)
                    {
                        var calendar = _statsProzoneMotorIngestService.IngestLeagueCalendar(league.ProviderSlug, season.ProviderSeasonId, race.ProviderRaceId);

                        await PersistCalendarInRepository(calendar, race, season, cancellationToken);
                    }
                }
            }
        }

        public async Task IngestRacesEventsGrids(CancellationToken cancellationToken)
        {
            var activeLeagues = await _motorsportService.GetActiveLeagues();

            foreach (var league in activeLeagues)
            {
                var season = await _motorsportService.GetCurrentSeasonForLeague(league.Id, cancellationToken);

                var motorsportRaces = await _motorsportService.GetRacesForLeague(league.Id);

                var raceEvents = await _motorsportService.GetTodayEventsForRace(league.Id, season.Id);

                foreach (var raceEvent in raceEvents)
                {
                    var raceResults =
                        _statsProzoneMotorIngestService.IngestRaceGrid(league.Slug, season.ProviderSeasonId, raceEvent.MotorsportRace.ProviderRaceId);

                    await PersistGridInRepository(raceResults, raceEvent.MotorsportRace, cancellationToken);
                }

                foreach (var race in motorsportRaces)
                {
                    var raceResults =
                        _statsProzoneMotorIngestService.IngestRaceGrid(league.Slug, season.ProviderSeasonId, race.ProviderRaceId);

                    await PersistGridInRepository(raceResults, race, cancellationToken);
                }
            }
        }

        public async Task IngestLeagues(CancellationToken cancellationToken)
        {
            var leagues = _statsProzoneMotorIngestService.IngestLeagues();

            await PersistLeaguesInRepository(leagues, cancellationToken);
        }

        public async Task IngestHistoricRaces(CancellationToken cancellationToken)
        {
            var motorLeagues = await _motorsportService.GetActiveLeagues();

            if (motorLeagues != null)
            {
                foreach (var league in motorLeagues)
                {
                    if (league.ProviderSlug is null) continue;

                    var pastSeason = await _motorsportService.GetPastSeasonForLeague(league.Id, cancellationToken);

                    var races =
                        _statsProzoneMotorIngestService.IngestLeagueRaces(league.ProviderSlug, pastSeason.ProviderSeasonId);

                    await PersistRacesInRepository(races, league, cancellationToken);
                }
            }
        }

        public async Task IngestHistoricEventsResults(CancellationToken cancellationToken)
        {
            var activeLeagues = await _motorsportService.GetActiveLeagues();

            foreach (var league in activeLeagues)
            {
                var season = await _motorsportService.GetPastSeasonForLeague(league.Id, cancellationToken);

                var motorsportRaces = await _motorsportService.GetRacesForLeague(league.Id);

                foreach (var race in motorsportRaces)
                {
                    var raceResults =
                        _statsProzoneMotorIngestService.IngestRaceResults(league.Slug, season.ProviderSeasonId, race.ProviderRaceId);

                    await PersistResultsInRepository(raceResults, race, cancellationToken);
                }
            }
        }

        public async Task IngestHistoricTeamStandings(CancellationToken cancellationToken)
        {
            var motorLeagues = _publicSportDataUnitOfWork.MotorsportLeagues.Where(league => league.IsEnabled).ToList();

            foreach (var league in motorLeagues)
            {
                if (league.ProviderSlug is null) continue;

                var season = await _motorsportService.GetPastSeasonForLeague(league.Id, cancellationToken);

                var teamStandings = _statsProzoneMotorIngestService.IngestTeamStandings(league.ProviderSlug, season.ProviderSeasonId);

                await PersistTeamStandingsInRepository(teamStandings, league, season, cancellationToken);
            }
        }

        public async Task IngestHistoricDriverStandings(CancellationToken cancellationToken)
        {
            var motorLeagues = _publicSportDataUnitOfWork.MotorsportLeagues.Where(race => race.IsEnabled).ToList();

            foreach (var league in motorLeagues)
            {
                if (league.ProviderSlug is null) continue;

                var season = await _motorsportService.GetPastSeasonForLeague(league.Id, cancellationToken);

                var driverStandings = _statsProzoneMotorIngestService.IngestDriverStandings(league.ProviderSlug, season.ProviderSeasonId);

                await PersistDriverStandingsInRepository(driverStandings, league, season, cancellationToken);
            }
        }

        public async Task IngestLiveRaceEventData(MotorsportRace race, CancellationToken cancellationToken)
        {
            //TODO
            await Task.FromResult(0);
        }

        public async Task IngestHistoricEventsGrids(CancellationToken cancellationToken)
        {
            var activeLeagues = await _motorsportService.GetActiveLeagues();

            foreach (var league in activeLeagues)
            {
                var season = await _motorsportService.GetPastSeasonForLeague(league.Id, cancellationToken);

                var motorsportRaces = await _motorsportService.GetRacesForLeague(league.Id);

                foreach (var race in motorsportRaces)
                {
                    var raceResults =
                        _statsProzoneMotorIngestService.IngestRaceGrid(league.Slug, season.ProviderSeasonId, race.ProviderRaceId);

                    await PersistGridInRepository(raceResults, race, cancellationToken);
                }
            }
        }

        private async Task PersistLeagueDriversInRepository(MotorsportEntitiesResponse providerResponse)
        {
            var driversFromProvider = ExtractDriversFromProviderResponse(providerResponse);

            if (driversFromProvider != null)
            {
                foreach (var providerDriver in driversFromProvider)
                {
                    var driverInRepo =
                        _publicSportDataUnitOfWork.MotorsportDrivers.FirstOrDefault(d => d.ProviderDriverId == providerDriver.playerId);

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

        private async Task PersistRacesInRepository(
            MotorsportEntitiesResponse providerResponse,
            MotorsportLeague league,
            CancellationToken cancellationToken)
        {
            var racesFromProvider = ExtractRacesFromProviderResponse(providerResponse);

            if (racesFromProvider is null)
                return;

            foreach (var providerRace in racesFromProvider)
            {
                var raceInRepo = _publicSportDataUnitOfWork.MotorsportRaces.FirstOrDefault(r => r.ProviderRaceId == providerRace.raceId);

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

        private async Task PersistCalendarInRepository(
            MotorsportEntitiesResponse providerResponse,
            MotorsportRace race,
            MotorsportSeason season,
            CancellationToken cancellationToken)
        {
            var calendarFromProviderResponse = ExtractCalendarFromProviderResponse(providerResponse);

            if (calendarFromProviderResponse is null)
                return;

            foreach (var raceEvent in calendarFromProviderResponse)
            {
                var calendarInRepo =
                    _publicSportDataUnitOfWork.MotorsportRaceEvents.FirstOrDefault(r =>
                    r.MotorsportRace.ProviderRaceId == raceEvent.race.raceId
                    && r.MotorsportSeason.Id == season.Id);

                if (calendarInRepo is null)
                {
                    AddNewCalendarEventToRepo(raceEvent, race, season);
                }
                else
                {
                    UpdateCalendarEventInRepo(raceEvent, calendarInRepo);
                }
            }

            await _publicSportDataUnitOfWork.SaveChangesAsync();
        }

        private async Task PersistLeaguesInRepository(
            MotorsportEntitiesResponse providerResponse,
            CancellationToken cancellationToken)
        {
            var leaguesFromProvider = ExtractLeaguesFromProviderResponse(providerResponse);
            if (leaguesFromProvider != null)
            {
                foreach (var leagueFromProvider in leaguesFromProvider)
                {
                    var leagueInRepo =
                        _publicSportDataUnitOfWork.MotorsportLeagues.FirstOrDefault(l =>
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

        private async Task PersistSeasonsInRepository(
            MotorsportEntitiesResponse providerResponse,
            MotorsportLeague league,
            CancellationToken cancellationToken)
        {
            var seasonsFromProviderResponse = ExtractSeasonsFromProviderResponse(providerResponse);

            if (seasonsFromProviderResponse != null)
            {
                foreach (var providerSeason in seasonsFromProviderResponse)
                {
                    var seasonInRepo =
                        _publicSportDataUnitOfWork.MotorsportSeasons.FirstOrDefault(l =>
                            l.ProviderSeasonId == providerSeason.season);

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
        }

        private async Task PersistDriverStandingsInRepository(
            MotorsportEntitiesResponse providerResponse,
            MotorsportLeague league,
            MotorsportSeason season,
            CancellationToken cancellationToken)
        {
            var standingsFromProvider = ExtractDriverStandingsFromProviderResponse(providerResponse);
            if (standingsFromProvider != null)
            {
                foreach (var providerEntry in standingsFromProvider)
                {
                    var driverStanding =
                        _publicSportDataUnitOfWork.MotorsportDriverStandings.FirstOrDefault(s => s.MotorsportDriver.ProviderDriverId == providerEntry.playerId);
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
        }

        private async Task PersistTeamsInRepository(MotorsportEntitiesResponse response)
        {
            var ownersFromProvider = ExtractOwnersFromProviderResponse(response);

            if (ownersFromProvider is null)
            {
                return;
            }

            foreach (var owner in ownersFromProvider)
            {
                var ownerInRepo =
                    _publicSportDataUnitOfWork.MotortsportTeams.FirstOrDefault(t => t.ProviderTeamId == owner.ownerId);

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

        private async Task PersistTeamStandingsInRepository(
            MotorsportEntitiesResponse providerResponse,
            MotorsportLeague league,
            MotorsportSeason season,
            CancellationToken cancellationToken)
        {
            var teamStandingsFromProviderResponse = ExtractTeamStandingsFromProviderResponse(providerResponse);

            if (teamStandingsFromProviderResponse != null)
            {
                foreach (var providerStanding in teamStandingsFromProviderResponse)
                {
                    var repoStanding =
                        _publicSportDataUnitOfWork.MotorsportTeamStandings.FirstOrDefault(s => s.MotorsportTeam.ProviderTeamId == providerStanding.teamId);

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
        }

        private async Task PersistResultsInRepository(
            MotorsportEntitiesResponse response,
            MotorsportRace race,
            CancellationToken cancellationToken)
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
                    _publicSportDataUnitOfWork.MotorsportRaceEventResults.FirstOrDefault(
                        r => r.MotorsportDriver.ProviderDriverId == playerId
                        && r.MotorsportRaceEventId == race.Id);

                if (resultInRepo is null)
                {
                    AddNewResultsToRepo(result, race);
                }
                else
                {
                    UpdateResultsInRepo(resultInRepo, result);
                }
            }

            await _publicSportDataUnitOfWork.SaveChangesAsync();
        }

        private async Task PersistGridInRepository(
            MotorsportEntitiesResponse response,
            MotorsportRace race,
            CancellationToken cancellationToken)
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
                    _publicSportDataUnitOfWork.MotorsportRaceEventGrids.FirstOrDefault(
                        g => g.MotorsportDriver.ProviderDriverId == playerId
                        && g.MotorsportRaceEvent.MotorsportRace.Id == race.Id);

                if (gridInRepo is null)
                {
                    AddNewGridEntryToRepo(providerGridEntry, race);
                }
                else
                {
                    UpdateGridEntryInRepo(gridInRepo, providerGridEntry);
                }
            }

            await _publicSportDataUnitOfWork.SaveChangesAsync();
        }

        private void UpdateGridEntryInRepo(
            MotorsportRaceEventGrid raceEventGridInRepo,
            Result providerGridEntry)
        {
            var qualifyingRun = providerGridEntry?.qualifying?.qualifyingRuns?.FirstOrDefault();
            if (qualifyingRun is null) return;

            raceEventGridInRepo.GridPosition = providerGridEntry.carPosition.startingPosition;
            raceEventGridInRepo.QualifyingTimeMinutes = qualifyingRun.time.minutes;
            raceEventGridInRepo.QualifyingTimeSeconds = qualifyingRun.time.seconds;
            raceEventGridInRepo.QualifyingTimeMilliseconds = qualifyingRun.time.milliseconds;

            _publicSportDataUnitOfWork.MotorsportRaceEventGrids.Update(raceEventGridInRepo);
        }

        private void AddNewGridEntryToRepo(Result providerGridEntry, MotorsportRace race)
        {
            var driverInRepo = _publicSportDataUnitOfWork.MotorsportDrivers.FirstOrDefault(d => d.ProviderDriverId == providerGridEntry.player.playerId);
            if (driverInRepo is null) return;

            var teamInRepo = _publicSportDataUnitOfWork.MotortsportTeams.FirstOrDefault(d => d.ProviderTeamId == providerGridEntry.owner.ownerId);
            if (teamInRepo is null) return;

            var racecEvent =
                _publicSportDataUnitOfWork.MotorsportRaceEvents.FirstOrDefault(e => e.MotorsportRace.Id == race.Id);
            if (racecEvent is null) return;

            var newGridEntry = new MotorsportRaceEventGrid
            {
                MotorsportDriver = driverInRepo,
                MotorsportDriverId = driverInRepo.Id,
                GridPosition = providerGridEntry.carPosition.startingPosition,
                MotorsportTeam = teamInRepo,
                MotorsportTeamId = teamInRepo.Id,
                MotorsportRaceEventId = race.Id,
                MotorsportRaceEvent = racecEvent
            };

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

        private void UpdateResultsInRepo(MotorsportRaceEventResult eventResultInRepo, Result result)
        {
            eventResultInRepo.Points = int.Parse(result.points.driver.total);

            if (result.finishingTime != null)
            {
                eventResultInRepo.FinishingTimeHours = result.finishingTime.hours;
                eventResultInRepo.FinishingTimeMinutes = result.finishingTime.minutes;
                eventResultInRepo.FinishingTimeSeconds = result.finishingTime.seconds;
            }

            eventResultInRepo.Position = result.carPosition.position;
            eventResultInRepo.GridPosition = result.carPosition.startingPosition;
            eventResultInRepo.OutReason = result.carStatus.name;
            eventResultInRepo.CompletedRace = !result.carStatus.carStatusId.Equals(0);

            _publicSportDataUnitOfWork.MotorsportRaceEventResults.Update(eventResultInRepo);
        }

        private void AddNewResultsToRepo(Result result, MotorsportRace race)
        {
            var driver =
                _publicSportDataUnitOfWork.MotorsportDrivers.FirstOrDefault(d =>
                    d.ProviderDriverId == result.player.playerId);

            if (driver is null) return;

            var team =
                _publicSportDataUnitOfWork.MotortsportTeams.FirstOrDefault(
                    t => t.ProviderTeamId == result.owner.ownerId);

            if (team is null) return;

            var motorsportRaceResult = new MotorsportRaceEventResult
            {
                Position = result.carPosition.position,
                GridPosition = result.carPosition.startingPosition,
                Points = int.Parse(result.points.driver.total),

                LapsCompleted = result.laps.completed,
                MotorsportTeam = team,
                MotorsportDriver = driver,
                MotorsportDriverId = driver.Id,
                MotorsportRaceEventId = race.Id,
                OutReason = result.carStatus.name
            };

            if (result.finishingTime != null)
            {
                motorsportRaceResult.FinishingTimeHours = result.finishingTime.hours;
                motorsportRaceResult.FinishingTimeMinutes = result.finishingTime.minutes;
                motorsportRaceResult.FinishingTimeSeconds = result.finishingTime.seconds;
                motorsportRaceResult.FinishingTimeMilliseconds = result.finishingTime.milliseconds;
            }

            _publicSportDataUnitOfWork.MotorsportRaceEventResults.Add(motorsportRaceResult);
        }

        private void UpdateLeagueInRepo(League leagueFromProvider, MotorsportLeague leagueInRepo)
        {
            leagueInRepo.ProviderSlug = leagueFromProvider.league.subLeague.abbreviation;

            leagueInRepo.Name = leagueFromProvider.league.subLeague.name;

            _publicSportDataUnitOfWork.MotorsportLeagues.Update(leagueInRepo);
        }

        private void AddNewLeagueToRepo(League leagueFromProvider)
        {
            var league = new MotorsportLeague
            {
                ProviderSlug = leagueFromProvider.league.uriPaths.FirstOrDefault()?.path,
                Name = leagueFromProvider.league.subLeague.name,
                ProviderLeagueId = leagueFromProvider.league.subLeague.subLeagueId,
                Slug = leagueFromProvider.league.uriPaths.FirstOrDefault()?.path,
                DataProvider = DataProvider.Stats
            };

            _publicSportDataUnitOfWork.MotorsportLeagues.Add(league);
        }

        private void UpdateDriverInRepo(Player providerDriver, MotorsportDriver driverInRepo)
        {
            driverInRepo.FirstName = providerDriver.firstName;
            driverInRepo.LastName = providerDriver.lastName;
            driverInRepo.DataProvider = DataProvider.Stats;

            _publicSportDataUnitOfWork.MotorsportDrivers.Update(driverInRepo);
        }

        private void AddNewDriverToRepo(Player providerDriver)
        {
            var driver = new MotorsportDriver
            {
                ProviderDriverId = providerDriver.playerId,
                FirstName = providerDriver.firstName,
                LastName = providerDriver.lastName,
                DataProvider = DataProvider.Stats,
            };

            if (providerDriver.car != null)
            {
                if (providerDriver.car.make != null) driver.ProviderCarId = providerDriver.car.make.makeId;
                if (providerDriver.car.carNumber != null) driver.CarNumber = providerDriver.car.carNumber;
            }

            if (providerDriver.birth?.country != null) driver.CountryName = providerDriver.birth.country.name;

            _publicSportDataUnitOfWork.MotorsportDrivers.Add(driver);
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

        private void UpdateCalendarEventInRepo(Event providerRaceEvent, MotorsportRaceEvent eventInRepo)
        {
            eventInRepo.CityName = providerRaceEvent.venue.city;
            eventInRepo.CircuitName = providerRaceEvent.venue.name;
            eventInRepo.CountryName = providerRaceEvent.venue.country.name;
            eventInRepo.CountryAbbreviation = providerRaceEvent.venue.country.abbreviation;

            var providerStartDate =
                providerRaceEvent.startDate.FirstOrDefault(d => d.dateType.ToLowerInvariant().Equals("utc"));

            if (providerStartDate != null)
            {
                var startDateUtc = DateTimeOffset.Parse(providerStartDate.full.ToString(CultureInfo.InvariantCulture));

                eventInRepo.StartDateTimeUtc = startDateUtc;
            }

            _publicSportDataUnitOfWork.MotorsportRaceEvents.Update(eventInRepo);
        }

        private void AddNewCalendarEventToRepo(Event providerRaceEvent, MotorsportRace race, MotorsportSeason season)
        {
            var raceCalendar = new MotorsportRaceEvent
            {
                CityName = providerRaceEvent.venue.city,
                CircuitName = providerRaceEvent.venue.name,
                CountryName = providerRaceEvent.venue.country.name,
                CountryAbbreviation = providerRaceEvent.venue.country.abbreviation,
                MotorsportRace = race,
                MotorsportSeason = season
            };

            var providerStartDate =
                providerRaceEvent.startDate.FirstOrDefault(d => d.dateType.ToLowerInvariant().Equals("utc"));

            if (providerStartDate != null)
            {
                var startDateUtc = DateTimeOffset.Parse(providerStartDate.full.ToString(CultureInfo.InvariantCulture));

                raceCalendar.StartDateTimeUtc = startDateUtc;
            }

            _publicSportDataUnitOfWork.MotorsportRaceEvents.Add(raceCalendar);
        }

        private void UpdateOwnerInRepo(MotorsportTeam ownerInRepo, Owner owner)
        {
            if (owner is null || owner.name is null)
                return;

            ownerInRepo.Name = owner.name;

            _publicSportDataUnitOfWork.MotortsportTeams.Update(ownerInRepo);
        }

        private void AddNewOwnerToRepo(Owner owner)
        {
            if (owner is null || owner.name is null)
                return;

            var motorsportTeam = new MotorsportTeam
            {
                Name = owner.name,
                ProviderTeamId = owner.ownerId,
                DataProvider = DataProvider.Stats
            };

            _publicSportDataUnitOfWork.MotortsportTeams.Add(motorsportTeam);
        }

        private void AddNewTeamToRepo(Team team)
        {
            if (team is null || team.name is null)
                return;

            var motorsportTeam = new MotorsportTeam
            {
                Name = team.name,
                ProviderTeamId = team.teamId,
                DataProvider = DataProvider.Stats
            };

            _publicSportDataUnitOfWork.MotortsportTeams.Add(motorsportTeam);
        }

        private void UpdateDriverStandingInRepo(Player providerStanding, MotorsportDriverStanding repoStanding)
        {
            if (providerStanding is null || providerStanding.finishes is null)
                return;

            repoStanding.Points = providerStanding.points;
            repoStanding.Position = providerStanding.rank;
            repoStanding.Wins = providerStanding.finishes.first;

            _publicSportDataUnitOfWork.MotorsportDriverStandings.Update(repoStanding);
        }

        private async Task AddNewDriverStandingToRepo(Player providerStanding, MotorsportLeague league, MotorsportSeason season)
        {
            if (providerStanding is null || providerStanding.finishes is null)
                return;

            var repoDriver = _publicSportDataUnitOfWork.MotorsportDrivers.FirstOrDefault(d => d.ProviderDriverId == providerStanding.playerId);

            var repoTeam =
                _publicSportDataUnitOfWork.MotortsportTeams.FirstOrDefault(t => t.ProviderTeamId == providerStanding.owner.ownerId);

            if (repoDriver is null)
            {
                AddNewDriverToRepo(providerStanding);

                await _publicSportDataUnitOfWork.SaveChangesAsync();

                repoDriver =
                    _publicSportDataUnitOfWork.MotorsportDrivers.FirstOrDefault(d => d.ProviderDriverId == providerStanding.playerId);
            }

            if (repoTeam is null)
            {
                AddNewOwnerToRepo(providerStanding.owner);

                await _publicSportDataUnitOfWork.SaveChangesAsync();

                repoTeam =
                    _publicSportDataUnitOfWork.MotortsportTeams.FirstOrDefault(t => t.ProviderTeamId == providerStanding.owner.ownerId);
            }

            var standingEntry = new MotorsportDriverStanding
            {
                MotorsportLeague = league,
                Points = providerStanding.points,
                Position = providerStanding.rank,
                Wins = providerStanding.finishes.first,
                MotorsportDriverId = repoDriver.Id,
                MotorsportDriver = repoDriver,
                MotorsportTeam = repoTeam,
                MotorsportTeamId = repoTeam.Id,
                MotorsportSeason = season,
                MotorsportSeasonId = season.Id,
                MotorsportLeagueId = league.Id
            };

            _publicSportDataUnitOfWork.MotorsportDriverStandings.Add(standingEntry);
        }

        private void UpdateTeamStandingInRepo(Team providerStanding, MotorsportTeamStanding repoStanding)
        {
            if (providerStanding is null || providerStanding.finishes is null)
                return;

            repoStanding.Points = providerStanding.points;
            repoStanding.Position = providerStanding.rank;

            _publicSportDataUnitOfWork.MotorsportTeamStandings.Update(repoStanding);
        }

        private async Task AddNewTeamStandingToRepo(Team providerTeam, MotorsportLeague league, MotorsportSeason season)
        {
            if (providerTeam is null || providerTeam.finishes is null)
                return;

            var teamFromRepo = _publicSportDataUnitOfWork.MotortsportTeams.FirstOrDefault(t => t.ProviderTeamId == providerTeam.teamId);


            if (teamFromRepo is null)
            {
                AddNewTeamToRepo(providerTeam);

                await _publicSportDataUnitOfWork.SaveChangesAsync();

                teamFromRepo =
                    _publicSportDataUnitOfWork.MotortsportTeams.FirstOrDefault(t => t.ProviderTeamId == providerTeam.teamId);
            }

            var teamStanding = new MotorsportTeamStanding
            {
                MotorsportTeamId = teamFromRepo.Id,
                MotorsportLeagueId = league.Id,
                MotorsportSeasonId = season.Id,
                Position = providerTeam.rank,
                Points = providerTeam.points,
                MotorsportSeason = season,
                MotorsportTeam = teamFromRepo,
                MotorsportLeague = league
            };

            _publicSportDataUnitOfWork.MotorsportTeamStandings.Add(teamStanding);
        }

        private void UpdateSeasonInRepo(MotorsportSeason seasonInRepo, Season providerSeason)
        {
            if (providerSeason is null || providerSeason.name is null)
                return;

            seasonInRepo.Name = providerSeason.name;
            seasonInRepo.IsActive = providerSeason.isActive;
            seasonInRepo.DataProvider = DataProvider.Stats;

            _publicSportDataUnitOfWork.MotorsportSeasons.Update(seasonInRepo);
        }

        private void AddNewSeasonToRepo(Season season, MotorsportLeague league)
        {
            if (season is null || season.name is null)
                return;

            var motorsportSeason = new MotorsportSeason
            {
                Name = season.name,
                IsActive = season.isActive,
                ProviderSeasonId = season.season,
                MotorsportLeague = league,
                DataProvider = DataProvider.Stats
            };

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

        private static IEnumerable<Event> ExtractCalendarFromProviderResponse(MotorsportEntitiesResponse response)
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

    }
}