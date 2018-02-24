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
        private readonly IMotorsportService _motorService;
        private readonly ILoggingService _loggingService;

        public MotorsportIngestWorkerService(
            IStatsProzoneMotorIngestService statsProzoneMotorIngestService,
            IPublicSportDataUnitOfWork publicSportDataUnitOfWork,
            ILoggingService loggingService,
            IMotorsportService motorService)
        {
            _statsProzoneMotorIngestService = statsProzoneMotorIngestService;
            _publicSportDataUnitOfWork = publicSportDataUnitOfWork;
            _loggingService = loggingService;
            _motorService = motorService;
        }

        public async Task IngestDriversForActiveLeagues(CancellationToken cancellationToken)
        {
            var activeLeagues = await _motorService.GetActiveLeagues();

            foreach (var league in activeLeagues)
            {
                var providerSeasonId = await _motorService.GetProviderSeasonIdForLeague(league.Id, cancellationToken);

                var leagueDrivers = _statsProzoneMotorIngestService.IngestDriversForLeague(league.ProviderSlug, providerSeasonId);

                await PersistLeagueDriversInRepository(leagueDrivers);
            }

        }

        public async Task IngestTeamsForActiveLeagues(CancellationToken cancellationToken)
        {
            var motorLeagues = _publicSportDataUnitOfWork.MotorsportLeagues.Where(league => league.IsEnabled);
            if (motorLeagues != null)
            {
                foreach (var league in motorLeagues)
                {
                    if (league.ProviderSlug is null) continue;

                    var tournamentTeams = _statsProzoneMotorIngestService.IngestTeamsForLeague(league.ProviderSlug);
                    await PersistTournamentTeamsInRepository(tournamentTeams);
                }
            }
        }

        public async Task IngestDriverStandingsForActiveLeagues(CancellationToken cancellationToken)
        {
            var motorLeagues = _publicSportDataUnitOfWork.MotorsportLeagues.Where(race => race.IsEnabled);
            if (motorLeagues != null)
            {
                foreach (var league in motorLeagues)
                {
                    if (league.ProviderSlug is null) continue;

                    var providerSeason = await _motorService.GetProviderSeasonIdForLeague(league.Id, cancellationToken);

                    var driverStandings = _statsProzoneMotorIngestService.IngestDriverStandings(league.ProviderSlug, providerSeason);

                    await PersistDriverStandingsInRepository(driverStandings, league, cancellationToken);
                }
            }
        }

        public async Task IngestTeamStandingsForActiveLeagues(CancellationToken cancellationToken)
        {
            var motorLeagues = _publicSportDataUnitOfWork.MotorsportLeagues.Where(league => league.IsEnabled);

            if (motorLeagues != null)
            {
                foreach (var league in motorLeagues)
                {
                    if (league.ProviderSlug is null) continue;

                    var providerSeason = await _motorService.GetProviderSeasonIdForLeague(league.Id, cancellationToken);

                    var teamStandings = _statsProzoneMotorIngestService.IngestDriverStandings(league.ProviderSlug, providerSeason);

                    await PersistTeamStandingsInRepository(teamStandings, cancellationToken);
                }
            }
        }

        public async Task IngestRacesForActiveLeagues(CancellationToken cancellationToken)
        {
            var motorLeagues = await _motorService.GetActiveLeagues();

            if (motorLeagues != null)
            {
                foreach (var league in motorLeagues)
                {
                    if (league.ProviderSlug is null) continue;

                    var providerSeasonId = await _motorService.GetProviderSeasonIdForLeague(league.Id, cancellationToken);

                    var races = _statsProzoneMotorIngestService.IngestLeagueCalendar(league.ProviderSlug, providerSeasonId);

                    await PersistRacesInRepository(races, league, cancellationToken);
                }
            }
        }

        public async Task IngestRaceResults(CancellationToken cancellationToken)
        {
            var currentYear = DateTime.Now.Year;

            var activeLeagues = await _motorService.GetActiveLeagues();

            foreach (var league in activeLeagues)
            {
                for (var providerSeasonId = currentYear - 2; providerSeasonId <= currentYear; providerSeasonId++)
                {
                    var motorsportRaces =
                        await _motorService.GetLeagueRacesByProviderSeasonId(league.Id, providerSeasonId);

                    foreach (var race in motorsportRaces)
                    {
                        var tournamentGrid = _statsProzoneMotorIngestService.IngestRaceGrid(league.Slug, providerSeasonId, race.ProviderRaceId);

                        await PersistGridInRepository(tournamentGrid, cancellationToken); 
                    }
                }
            }
        }

        public async Task IngestRaceGridsForPastSeasons(CancellationToken cancellationToken)
        {
            var currentYear = DateTime.Now.Year;

            var activeLeagues = await _motorService.GetActiveLeagues();

            foreach (var league in activeLeagues)
            {
                for (var providerSeasonId = currentYear - 2; providerSeasonId <= currentYear; providerSeasonId++)
                {
                    var motorsportRaces =
                        await _motorService.GetLeagueRacesByProviderSeasonId(league.Id, providerSeasonId);

                    foreach (var race in motorsportRaces)
                    {
                        var tournamentGrid = _statsProzoneMotorIngestService.IngestRaceGrid(league.Slug, providerSeasonId, race.ProviderRaceId);

                        await PersistGridInRepository(tournamentGrid, cancellationToken);
                    }
                }
            }
        }

        public async Task IngestLeagues(CancellationToken cancellationToken)
        {
            var leagues = _statsProzoneMotorIngestService.IngestLeagues();

            await PersistLeaguesInRepository(leagues, cancellationToken);
        }

       

        private async Task PersistLeagueDriversInRepository(MotorEntitiesResponse providerResponse)
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

        private async Task PersistRacesInRepository(MotorEntitiesResponse providerResponse,
            MotorsportLeague league, CancellationToken cancellationToken)
        {
            var racesFromProvider = ExtractRacesFromProviderResponse(providerResponse);
            if (racesFromProvider is null)
                return;

            foreach (var providerRace in racesFromProvider)
            {
                var raceInRepo = _publicSportDataUnitOfWork.MotorsportRaces.FirstOrDefault(r => r.ProviderRaceId == providerRace.race.raceId);
                if (raceInRepo is null)
                {
                    AddNewRaceEventToRepo(providerRace, league);
                }
                else
                {
                    UpdateRaceEventInRepo(providerRace, raceInRepo);
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

        private async Task PersistDriverStandingsInRepository(MotorEntitiesResponse providerResponse, MotorsportLeague league,
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
                        _publicSportDataUnitOfWork.MotorsportTeamStandings.FirstOrDefault(s => s.MotorsportTeam.ProviderTeamId == team.teamId);
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
                var ownerInRepo = _publicSportDataUnitOfWork.MotortsportTeams.FirstOrDefault(o => o.ProviderTeamId == owner.ownerId);

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
                    _publicSportDataUnitOfWork.MotorsportRaceResults.FirstOrDefault(r => r.MotorsportDriver.ProviderDriverId == playerId);

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
                    _publicSportDataUnitOfWork.MotorsportGrids.FirstOrDefault(r => r.MotorsportDriver.ProviderDriverId == playerId);

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

        private void UpdateGridEntryInRepo(MotorsportGrid gridInRepo, Result providerGridEntry)
        {
            var qualifyingRun = providerGridEntry?.qualifying?.qualifyingRuns?.FirstOrDefault();
            if (qualifyingRun is null) return;

            gridInRepo.Position = providerGridEntry.carPosition.startingPosition;
            gridInRepo.QualifyingTimeMinutes = qualifyingRun.time.minutes;
            gridInRepo.QualifyingTimeSeconds = qualifyingRun.time.seconds;
            gridInRepo.QualifyingTimeMilliseconds = qualifyingRun.time.milliseconds;

            _publicSportDataUnitOfWork.MotorsportGrids.Update(gridInRepo);
        }

        private void AddNewGridEntryToRepo(Result providerGridEntry)
        {
            var qualifyingRun = providerGridEntry?.qualifying?.qualifyingRuns?.FirstOrDefault();
            if (qualifyingRun is null) return;

            var driverInRepo = _publicSportDataUnitOfWork.MotorsportDrivers.FirstOrDefault(d => d.ProviderDriverId == providerGridEntry.player.playerId);
            if (driverInRepo is null) return;

            var newGridEntry = new MotorsportGrid
            {
                MotorsportDriver = driverInRepo,
                DriverId = driverInRepo.Id,
                Position = providerGridEntry.carPosition.startingPosition,
                QualifyingTimeMinutes = qualifyingRun.time.minutes,
                QualifyingTimeSeconds = qualifyingRun.time.seconds,
                QualifyingTimeMilliseconds = qualifyingRun.time.milliseconds
            };

            _publicSportDataUnitOfWork.MotorsportGrids.Add(newGridEntry);
        }

        private void UpdateResultsInRepo(MotorsportRaceResult resultInRepo, Result result)
        {
            resultInRepo.DriverTotalPoints = int.Parse(result.points.driver.total);
            resultInRepo.FinishingTimeHours = result.finishingTime.hours;
            resultInRepo.FinishingTimeMinutes = result.finishingTime.minutes;
            resultInRepo.FinishingTimeSeconds = result.finishingTime.seconds;
            resultInRepo.Position = result.carPosition.position;
            resultInRepo.GridPosition = result.carPosition.startingPosition;

            _publicSportDataUnitOfWork.MotorsportRaceResults.Update(resultInRepo);
        }

        private void AddNewResultsToRepo(Result result)
        {
            var newEntry = new MotorsportRaceResult
            {
                Position = result.carPosition.position,
                GridPosition = result.carPosition.startingPosition,
                DriverTotalPoints = int.Parse(result.points.driver.total),
                FinishingTimeHours = result.finishingTime.hours,
                FinishingTimeMinutes = result.finishingTime.minutes,
                FinishingTimeSeconds = result.finishingTime.seconds,
                FinishingTimeMilliseconds = result.finishingTime.milliseconds,
                LapsCompleted = result.laps.completed,
            };

            var driver = _publicSportDataUnitOfWork.MotorsportDrivers.FirstOrDefault(d => d.ProviderDriverId == result.player.playerId);
            newEntry.MotorsportDriverId = driver.Id;

            _publicSportDataUnitOfWork.MotorsportRaceResults.Add(newEntry);
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
            driverInRepo.HeightInCentimeters = providerDriver.height.centimeters;
            driverInRepo.WeightInKilograms = providerDriver.weight.kilograms;
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
                HeightInCentimeters = providerDriver.height.centimeters,
                WeightInKilograms = providerDriver.weight.kilograms,
                CountryName = providerDriver.birth.country.name,
                ProviderCarId = providerDriver.car.make.makeId,
                CarNumber = providerDriver.car.carNumber,
                DataProvider = DataProvider.Stats
            };

            _publicSportDataUnitOfWork.MotorsportDrivers.Add(driver);
        }

        private void UpdateRaceEventInRepo(Event providerRaceEvent, MotorsportRace raceInRepo)
        {
            var startDate = providerRaceEvent.startDate.FirstOrDefault(d => d.dateType.ToLowerInvariant().Equals("utc"));
            raceInRepo.StartDateTimeUtc = DateTimeOffset.Parse(startDate?.full.ToString(CultureInfo.InvariantCulture));
            raceInRepo.RaceName = providerRaceEvent.race.name;
            raceInRepo.RaceNameAbbreviation = providerRaceEvent.race.name;
            raceInRepo.CountryName = providerRaceEvent.venue.country.name;
            raceInRepo.CountryAbbreviation = providerRaceEvent.venue.country.abbreviation;
            raceInRepo.CityName = providerRaceEvent.venue.city;
            raceInRepo.VenueName = providerRaceEvent.venue.name;
            raceInRepo.DataProvider = DataProvider.Stats;

            _publicSportDataUnitOfWork.MotorsportRaces.Update(raceInRepo);
        }

        private void AddNewRaceEventToRepo(Event providerRaceEvent, MotorsportLeague motorsportLeague)
        {
            var startDate = providerRaceEvent.startDate.FirstOrDefault(d => d.dateType.ToLowerInvariant().Equals("utc"));
            var newRace = new MotorsportRace
            {
                ProviderRaceId = providerRaceEvent.race.raceId,
                RaceName = providerRaceEvent.race.name,
                RaceNameAbbreviation = providerRaceEvent.race.name,
                MotorsportLeague = motorsportLeague,
                CountryName = providerRaceEvent.venue.country.name,
                CountryAbbreviation = providerRaceEvent.venue.country.abbreviation,
                CityName = providerRaceEvent.venue.city,
                VenueName = providerRaceEvent.venue.name,
                StartDateTimeUtc = DateTimeOffset.Parse(startDate?.full.ToString(CultureInfo.InvariantCulture)),

                DataProvider = DataProvider.Stats
            };

            _publicSportDataUnitOfWork.MotorsportRaces.Add(newRace);
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

            var newOwner = new MotorsportTeam
            {
                Name = owner.name,
                ProviderTeamId = owner.ownerId,
                DataProvider = DataProvider.Stats
            };

            _publicSportDataUnitOfWork.MotortsportTeams.Add(newOwner);
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

        private void AddNewDriverStandingToRepo(Player providerStanding, MotorsportLeague league)
        {
            if (providerStanding is null || providerStanding.finishes is null)
                return;

            var repoDriver = _publicSportDataUnitOfWork.MotorsportDrivers.FirstOrDefault(d => d.ProviderDriverId == providerStanding.playerId);
            if (repoDriver is null)
            {
                AddNewDriverToRepo(providerStanding);
                _publicSportDataUnitOfWork.SaveChangesAsync();
                repoDriver = _publicSportDataUnitOfWork.MotorsportDrivers.FirstOrDefault(d => d.ProviderDriverId == providerStanding.playerId);
            }

            var standingEntry = new MotorsportDriverStanding
            {
                MotorsportLeague = league,
                Points = providerStanding.points,
                Position = providerStanding.rank,
                Wins = providerStanding.finishes.first,

                MotorsportDriver = repoDriver
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

        private void AddNewTeamStandingToRepo(Team providerTeam)
        {
            if (providerTeam is null || providerTeam.finishes is null)
                return;

            var teamFromRepo = _publicSportDataUnitOfWork.MotortsportTeams.FirstOrDefault(t => t.ProviderTeamId == providerTeam.teamId);

            if (teamFromRepo is null)
                return;

            var teamStanding = new MotorsportTeamStanding
            {
                Points = providerTeam.points,
                Position = providerTeam.rank
            };

            _publicSportDataUnitOfWork.MotorsportTeamStandings.Add(teamStanding);
        }

        private static IEnumerable<Result> ExtractResultsFromProviderResponse(MotorEntitiesResponse response)
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

        private static IEnumerable<Result> ExtractRaceGridFromProviderResponse(MotorEntitiesResponse response)
        {
            return ExtractResultsFromProviderResponse(response);
        }

        private static IEnumerable<Event> ExtractRacesFromProviderResponse(MotorEntitiesResponse response)
        {
            if (response != null && response.recordCount <= 0)
                return null;

            var races = response
                ?.apiResults.FirstOrDefault()
                ?.league
                .subLeague
                .season
                .eventType
                .FirstOrDefault()
                ?.events;

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

        private static async Task PersistTournamentTeamsInRepository(MotorEntitiesResponse response)
        {
            // STATS API does not provide such data under teams end-point. 
            // We Ingest it using their "Owners" end-point.
        }
    }
}