using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.Stats.Models.Tennis;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.Stats.Models.Tennis.TennisEventsResponse;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.Stats.Models.Tennis.TennisRankingsResponse;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.Stats.Models.Tennis.TennisResultsResponse;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.Stats.Models.Tennis.TennisSurfaceTypesResponse;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.Stats.Models.Tennis.TennisVenuesResponse;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models.Enums;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models.Tennis;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.UnitOfWork;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models.Enums;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models.Tennis;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.UnitOfWork;
using MatchType = SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.Stats.Models.Tennis.TennisResultsResponse.MatchType;

namespace SuperSportDataEngine.ApplicationLogic.Services.Tennis
{
    public class TennisStorageService : ITennisStorageService
    {
        private readonly IPublicSportDataUnitOfWork _publicSportDataUnitOfWork;
        private readonly ISystemSportDataUnitOfWork _systemSportDataUnitOfWork;

        public TennisStorageService(
            IPublicSportDataUnitOfWork publicSportDataUnitOfWork, 
            ISystemSportDataUnitOfWork systemSportDataUnitOfWork)
        {
            _publicSportDataUnitOfWork = publicSportDataUnitOfWork;
            _systemSportDataUnitOfWork = systemSportDataUnitOfWork;
        }

        public async Task AddOrUpdateTennisParticipants(
            List<Boundaries.Gateway.Http.Stats.Models.Tennis.TennisParticipantsResponse.Player> participants)
        {
            foreach (var participant in participants)
            {
                var dbPlayer = _publicSportDataUnitOfWork.TennisPlayers.FirstOrDefault(player =>
                    player.ProviderPlayerId == participant.playerId);

                IngestCountry(participant.nationality.countryId, participant.nationality.name, participant.nationality.abbreviation);

                var countryId = participant.nationality.countryId;
                var dbCountry = _publicSportDataUnitOfWork.TennisCountries.FirstOrDefault(
                    c => c.ProviderCountryId == countryId);

                var newPlayer = new TennisPlayer()
                {
                    FirstName = participant.firstName,
                    LastName = participant.lastName,
                    ProviderPlayerId = participant.playerId,
                    Handedness = GetHandedness(participant.handedness?.name),
                    DataProvider = DataProvider.Stats,
                    TennisCountry = dbCountry
                };

                if (dbPlayer == null)
                {
                    _publicSportDataUnitOfWork.TennisPlayers.Add(newPlayer);
                }
                else
                {
                    dbPlayer.FirstName = participant.firstName;
                    dbPlayer.LastName = participant.lastName;
                    dbPlayer.ProviderPlayerId = participant.playerId;
                    dbPlayer.Handedness = GetHandedness(participant.handedness?.name);
                    dbPlayer.DataProvider = DataProvider.Stats;
                    dbPlayer.TennisCountry = newPlayer.TennisCountry;

                    _publicSportDataUnitOfWork.TennisPlayers.Update(dbPlayer);
                }
            }

            await _publicSportDataUnitOfWork.SaveChangesAsync();
        }

        private TennisHandedness GetHandedness(string handedness)
        {
            handedness = handedness?.ToLower();

            switch (handedness)
            {
                case "right":
                    return TennisHandedness.RightHanded;
                case "left":
                    return TennisHandedness.LeftHanded;
            }

            return TennisHandedness.Unknown;
        }

        public IEnumerable<TennisLeague> GetEnabledLeagues()
        {
            return _publicSportDataUnitOfWork.TennisLeagues.Where(l => !l.IsDisabledInbound).ToList();
        }

        public TennisLeague GetTennisLeagueFor(int providerLeagueId)
        {
            return _publicSportDataUnitOfWork.TennisLeagues.FirstOrDefault(l => l.ProviderLeagueId == providerLeagueId);
        }

        public TennisLeague GetTennisLeagueFor(string providerSlug)
        {
            return _publicSportDataUnitOfWork.TennisLeagues.FirstOrDefault(l => l.ProviderSlug == providerSlug);
        }

        public TennisSeason GetCurrentTennisSeasonForLeague(string providerSlug)
        {
            return _publicSportDataUnitOfWork.TennisSeasons.FirstOrDefault(
                s => s.IsCurrent && s.TennisLeague.ProviderSlug == providerSlug);
        }

        public TennisTournament GetTennisTournamentFor(int providerTournamentId)
        {
            return _publicSportDataUnitOfWork.TennisTournaments.FirstOrDefault(t => t.ProviderTournamentId == providerTournamentId);
        }

        public async Task AddOrUpdateEventsFromProvider(TennisEventsResponse eventsForLeague, TennisLeague tennisLeague)
        {
            var events = eventsForLeague?.apiResults?.FirstOrDefault()?.league?.subLeague?.season?.eventType?.FirstOrDefault()?.events;
            var season = eventsForLeague?.apiResults?.FirstOrDefault()?.league?.subLeague?.season;

            if (events == null)
                return;

            foreach (var @event in events)
            {
                var dbEvent = _publicSportDataUnitOfWork.TennisEvents.FirstOrDefault(e => e.ProviderEventId == @event.eventId);
                var matchEvent = @event.matchTypes?.First(e => e.matchTypeId == 1 || e.matchTypeId == 3);

                var matchStartUtc = @event.startDate.First(e => e.dateType.ToLower() == "utc");
                var matchEndUtc = @event.endDate.First(e => e.dateType.ToLower() == "utc");
                var prize = matchEvent?.prizeMoney?.First().money;

                if (season == null)
                    continue;

                var providerSeasonId = season.season;
                var providerSurfaceId = @event.venue?.surface?.surfaceId;
                var providerVenueId = @event.venue?.venueId;

                var tournamentDb = _publicSportDataUnitOfWork.TennisTournaments.FirstOrDefault(t => t.ProviderTournamentId == @event.tournament.tournamentId);

                if (tournamentDb == null)
                    continue;

                var seasonDb = _publicSportDataUnitOfWork.TennisSeasons.FirstOrDefault(s => s.ProviderSeasonId == providerSeasonId && s.TennisLeague.ProviderLeagueId == tennisLeague.ProviderLeagueId);

                if (seasonDb == null)
                    continue;

                var venueDb = _publicSportDataUnitOfWork.TennisVenues.FirstOrDefault(v => v.ProviderVenueId == providerVenueId);
                var surfaceDb = _publicSportDataUnitOfWork.TennisSurfaceTypes.FirstOrDefault(s => s.ProviderSurfaceId == providerSurfaceId);

                var newEvent = new TennisEvent()
                {
                    ProviderEventId = @event.eventId,
                    EventName = @event.tournament.name,
                    StartDateUtc = new DateTimeOffset(new DateTime(
                        matchStartUtc.year,
                        matchStartUtc.month,
                        matchStartUtc.date), TimeSpan.FromHours(0)),
                    EndDateUtc = new DateTimeOffset(new DateTime(
                        matchEndUtc.year,
                        matchEndUtc.month,
                        matchEndUtc.date), TimeSpan.FromHours(0)),
                    DataProvider = DataProvider.Stats,
                    TennisTournament = tournamentDb,
                    TennisSeason = seasonDb,
                    TennisVenue = venueDb,
                    TennisSurfaceType = surfaceDb
                };

                TennisEvent thisEvent;

                if (dbEvent == null)
                {
                    _publicSportDataUnitOfWork.TennisEvents.Add(newEvent);
                    thisEvent = newEvent;
                }
                else
                {
                    dbEvent.StartDateUtc = newEvent.StartDateUtc;
                    dbEvent.EndDateUtc = newEvent.EndDateUtc;
                    dbEvent.TennisVenue = newEvent.TennisVenue;
                    dbEvent.TennisSurfaceType = newEvent.TennisSurfaceType;
                    dbEvent.EventName = newEvent.EventName;

                    _publicSportDataUnitOfWork.TennisEvents.Update(dbEvent);

                    thisEvent = dbEvent;
                }

                await _publicSportDataUnitOfWork.SaveChangesAsync();

                await AddEventToSystemSportData(thisEvent);

                // Create an association between League and Event
                var thisLeague = tennisLeague;
                var hasAssociation = _publicSportDataUnitOfWork.TennisEventTennisLeagues.FirstOrDefault(
                                        m => m.TennisLeague.ProviderLeagueId == thisLeague.ProviderLeagueId &&
                                             m.TennisEvent.ProviderEventId == thisEvent.ProviderEventId) != null;

                if (!hasAssociation)
                {
                    _publicSportDataUnitOfWork.TennisEventTennisLeagues.Add(
                        new TennisEventTennisLeagues()
                        {
                            TennisLeague = thisLeague,
                            TennisEvent = thisEvent,
                            TennisEventId = thisEvent.Id,
                            TennisLeagueId = thisLeague.Id,
                            Prize = prize
                        });
                }
            }

            await _publicSportDataUnitOfWork.SaveChangesAsync();
        }

        private async Task AddEventToSystemSportData(TennisEvent tennisEvent)
        {
            var trackingEvent =
                _systemSportDataUnitOfWork.SchedulerTrackingTennisEvents.FirstOrDefault(
                    e => e.TennisEventId == tennisEvent.Id);

            var newTrackingEvent = new SchedulerTrackingTennisEvent
            {
                TennisEventId = tennisEvent.Id,
                StartDateTime = tennisEvent.StartDateUtc,
                EndDateTime = tennisEvent.EndDateUtc
            };

            if (trackingEvent == null)
            {
                _systemSportDataUnitOfWork.SchedulerTrackingTennisEvents.Add(
                    newTrackingEvent);
            }
            else
            {
                trackingEvent.StartDateTime = newTrackingEvent.StartDateTime;
                trackingEvent.EndDateTime = newTrackingEvent.EndDateTime;

                _systemSportDataUnitOfWork.SchedulerTrackingTennisEvents.Update(trackingEvent);
            }

            await _systemSportDataUnitOfWork.SaveChangesAsync();
        }

        public async Task AddOrUpdateTennisTournamentsFromProvider(
            List<Boundaries.Gateway.Http.Stats.Models.Tennis.TennisLeagueTournamentsResponse.Tournament> tournaments,
            Boundaries.Gateway.Http.Stats.Models.Tennis.TennisLeagueTournamentsResponse.League providerTennisLeague, 
            TennisLeague dbTennisLeague)
        {
            foreach (var tournament in tournaments)
            {
                var dbTournament =
                        GetTennisTournamentFor(
                            tournament.tournamentId);

                var newTournament = new TennisTournament
                {
                    ProviderTournamentId = tournament.tournamentId,
                    ProviderTournamentName = tournament.name,
                    ProviderDisplayName = tournament.displayName,
                    Abbreviation = tournament.name,
                    Slug = string.Join("_", providerTennisLeague.subLeague.abbreviation.ToLower(), tournament.name.ToLower().Replace(" ", "")),
                    TennisLeagues = new List<TennisLeague>() { dbTennisLeague },
                    DataProvider = DataProvider.Stats,
                };

                if (dbTournament == null)
                {
                    _publicSportDataUnitOfWork.TennisTournaments.Add(newTournament);

                    var leagueHasAssociatedTournament =
                            dbTennisLeague.TennisTournaments?.ToList().Any(t =>
                                t.ProviderTournamentId == tournament.tournamentId) ?? false;

                    if (!leagueHasAssociatedTournament)
                    {
                        if (dbTennisLeague.TennisTournaments != null)
                            dbTennisLeague.TennisTournaments.Add(newTournament);
                        else
                            dbTennisLeague.TennisTournaments = new List<TennisTournament>() { newTournament };
                    }
                }
                else
                {
                    dbTournament.ProviderTournamentId = tournament.tournamentId;
                    dbTournament.ProviderTournamentName = tournament.name;

                    var containsAssociatedLeague = dbTournament.TennisLeagues?.ToList().Any(l => l.ProviderLeagueId == dbTennisLeague.ProviderLeagueId) ?? false;
                    if (!containsAssociatedLeague)
                    {
                        if (dbTournament.TennisLeagues != null)
                            dbTournament.TennisLeagues.Add(dbTennisLeague);
                        else
                            dbTournament.TennisLeagues = new List<TennisLeague>() { dbTennisLeague };
                    }

                    dbTournament.DataProvider = newTournament.DataProvider;

                    _publicSportDataUnitOfWork.TennisTournaments.Update(dbTournament);

                    var leagueHasAssociatedTournament =
                            dbTennisLeague.TennisTournaments?.ToList().Any(t =>
                                t.ProviderTournamentId == tournament.tournamentId) ?? false;

                    if (!leagueHasAssociatedTournament)
                    {
                        if (dbTennisLeague.TennisTournaments != null)
                            dbTennisLeague.TennisTournaments.Add(dbTournament);
                        else
                            dbTennisLeague.TennisTournaments = new List<TennisTournament>() { dbTournament };
                    }
                }
            }

            await _publicSportDataUnitOfWork.SaveChangesAsync();
        }
        
        public async Task AddOrUpdateSeasonsFromProvider(TennisSeasonsResponse seasonsResponse, TennisLeague tennisLeague)
        {
            var season = seasonsResponse?.apiResults?.FirstOrDefault()?.league?.seasons?.FirstOrDefault();
            if (season == null)
                return;

            var eventType = season.eventType.First();

            var newSeason = new TennisSeason()
            {
                ProviderSeasonId = season.season,
                Name = season.name,
                TennisLeague = tennisLeague,
                IsActive = season.isActive,
                IsCurrent = false,
                StartDateUtc = new DateTimeOffset(new DateTime(
                    eventType.startDate.year,
                    eventType.startDate.month,
                    eventType.startDate.date)),
                EndDateUtc = new DateTimeOffset(new DateTime(
                    eventType.endDate.year,
                    eventType.endDate.month,
                    eventType.endDate.date)),
                DataProvider = DataProvider.Stats
            };

            var league = seasonsResponse?.apiResults?.FirstOrDefault()?.league;
            if (league != null)
            {
                var leagueId = (int)league?.subLeague.subLeagueId;

                var dbSeason = _publicSportDataUnitOfWork.TennisSeasons.FirstOrDefault(dbS =>
                    dbS.ProviderSeasonId == season.season &&
                    dbS.TennisLeague.ProviderLeagueId == leagueId);

                if (dbSeason == null)
                {
                    _publicSportDataUnitOfWork.TennisSeasons.Add(newSeason);
                }
                else
                {
                    dbSeason.StartDateUtc = newSeason.StartDateUtc;
                    dbSeason.EndDateUtc = newSeason.EndDateUtc;
                    dbSeason.IsActive = newSeason.IsActive;
                    dbSeason.TennisLeague = newSeason.TennisLeague;
                    dbSeason.Name = newSeason.Name;

                    _publicSportDataUnitOfWork.TennisSeasons.Update(dbSeason);
                }
            }

            await _publicSportDataUnitOfWork.SaveChangesAsync();
        }

        public async Task AddOrUpdateTennisLeaguesFromProvider(Boundaries.Gateway.Http.Stats.Models.Tennis.TennisLeaguesResponse.ApiResult leagues)
        {
            foreach (var league in leagues.leagues)
            {
                var dbLeague = _publicSportDataUnitOfWork.TennisLeagues.FirstOrDefault(dbL =>
                    dbL.ProviderLeagueId == league.league.subLeague.subLeagueId);

                var newLeague = new TennisLeague()
                {
                    ProviderLeagueId = league.league.subLeague.subLeagueId,
                    Abbreviation = league.league.subLeague.abbreviation,
                    Slug = league.league.uriPaths.First().path,
                    ProviderSlug = league.league.uriPaths.First().path,
                    Name = league.league.subLeague.name,
                    IsDisabledInbound = true,
                    DataProvider = DataProvider.Stats
                };

                if (dbLeague == null)
                {
                    _publicSportDataUnitOfWork.TennisLeagues.Add(newLeague);
                }
                else
                {
                    dbLeague.ProviderLeagueId = newLeague.ProviderLeagueId;
                    dbLeague.Name = newLeague.Name;

                    _publicSportDataUnitOfWork.TennisLeagues.Update(dbLeague);
                }
            }

            await _publicSportDataUnitOfWork.SaveChangesAsync();
        }

        public async Task AddOrUpdateTennisVenuesFromProvider(TennisVenuesResponse venuesFromProvider)
        {
            if (venuesFromProvider == null)
                return;

            var venues = venuesFromProvider.apiResults.First().league.season.venues;

            foreach (var venue in venues)
            {
                IngestCountry(venue.country.countryId, venue.country.name, venue.country.abbreviation);

                var countryId = venue.country.countryId;
                var dbCountry = _publicSportDataUnitOfWork.TennisCountries.FirstOrDefault(
                    c => c.ProviderCountryId == countryId);

                var dbVenue = _publicSportDataUnitOfWork.TennisVenues.FirstOrDefault(dbV =>
                    dbV.ProviderVenueId == venue.venueId);

                var newVenue = new TennisVenue
                {
                    ProviderVenueId = venue.venueId,
                    Name = venue.name,
                    City = venue.city,
                    DataProvider = DataProvider.Stats,
                    TennisCountry = dbCountry
                };

                if (dbVenue == null)
                {
                    _publicSportDataUnitOfWork.TennisVenues.Add(newVenue);
                }
                else
                {
                    dbVenue.Name = venue.name;
                    dbVenue.City = venue.city;
                    dbVenue.DataProvider = DataProvider.Stats;
                    dbVenue.TennisCountry = newVenue.TennisCountry;

                    _publicSportDataUnitOfWork.TennisVenues.Update(dbVenue);
                }

                await _publicSportDataUnitOfWork.SaveChangesAsync();
            }
        }

        private void IngestCountry(int countryId, string countryName, string countryAbbreviation)
        {
            var dbCountry = _publicSportDataUnitOfWork.TennisCountries.FirstOrDefault(
                c => c.ProviderCountryId == countryId);

            var newCountry = new TennisCountry
            {
                ProviderCountryId = countryId,
                Country = countryName,
                CountryAbbreviation = countryAbbreviation,
            };

            if (dbCountry == null)
            {
                _publicSportDataUnitOfWork.TennisCountries.Add(newCountry);
            }
            else
            {
                dbCountry.Country = newCountry.Country;
                dbCountry.CountryAbbreviation = newCountry.CountryAbbreviation;
            }

            _publicSportDataUnitOfWork.SaveChanges();
        }

        public async Task AddOrUpdateSurfaceTypesFromProvider(TennisSurfaceTypesResponse surfaceTypesFromProvider)
        {
            if (surfaceTypesFromProvider == null)
                return;

            var surfaceTypes = surfaceTypesFromProvider.apiResults.surfaceTypes;

            foreach (var surfaceType in surfaceTypes)
            {
                var dbSurfaceType =
                    _publicSportDataUnitOfWork.TennisSurfaceTypes.FirstOrDefault(surface =>
                        surface.ProviderSurfaceId == surfaceType.surfaceId);

                var newSurfaceType = new TennisSurfaceType
                {
                    Name = surfaceType.name,
                    ProviderSurfaceId = surfaceType.surfaceId,
                    DataProvider = DataProvider.Stats,
                };

                if (dbSurfaceType == null)
                {
                    _publicSportDataUnitOfWork.TennisSurfaceTypes.Add(newSurfaceType);
                }
                else
                {
                    dbSurfaceType.Name = newSurfaceType.Name;
                    _publicSportDataUnitOfWork.TennisSurfaceTypes.Update(dbSurfaceType);
                }
            }

            await _publicSportDataUnitOfWork.SaveChangesAsync();
        }

        public async Task SetTennisTournamentTypes()
        {
            var dbTournaments = (await _publicSportDataUnitOfWork.TennisTournaments.AllAsync());

            foreach (var tournament in dbTournaments)
            {
                var countAssociatedLeagues = tournament.TennisLeagues.Count;

                if (countAssociatedLeagues == 1)
                {
                    var leagueSlug = tournament.TennisLeagues.First().Slug.ToLower();
                    if (leagueSlug == "atp")
                    {
                        tournament.TennisTournamentType = TennisTournamentType.ATP;
                    }

                    if (leagueSlug == "wta")
                    {
                        tournament.TennisTournamentType = TennisTournamentType.WTA;
                    }
                }
                else if (countAssociatedLeagues == 2)
                {
                    var grandSlamProviderTournamentIds = new List<int>() { 1, 2, 11, 13 };

                    if (grandSlamProviderTournamentIds.Contains(tournament.ProviderTournamentId))
                    {
                        tournament.TennisTournamentType = TennisTournamentType.GS;
                    }
                    else
                    {
                        tournament.TennisTournamentType = TennisTournamentType.CO;
                    }
                }

                _publicSportDataUnitOfWork.TennisTournaments.Update(tournament);
            }

            await _publicSportDataUnitOfWork.SaveChangesAsync();
        }

        public async Task AddOrUpdateRanking(TennisRankingType rankingType, List<Ranking> leaderboard, TennisLeague league, TennisSeason season, DateTimeOffset dateThroughDateTime)
        {
            foreach(var leader in leaderboard)
            {
                var dbLeader = _publicSportDataUnitOfWork.TennisRankings.FirstOrDefault(r => 
                    r.TennisSeason.ProviderSeasonId == season.ProviderSeasonId &&
                    r.TennisPlayer.ProviderPlayerId == leader.player.playerId &&
                    r.TennisLeague.ProviderLeagueId == league.ProviderLeagueId &&
                    r.TennisRankingType == rankingType);

                var player = _publicSportDataUnitOfWork.TennisPlayers.FirstOrDefault(p => p.ProviderPlayerId == leader.player.playerId);

                // We should ingest the player using STATS's Single Player endpoint.
                if (player == null)
                    continue;

                var newRanking = new TennisRanking()
                {
                    TennisRankingType = rankingType,
                    TennisLeague = league,
                    TennisPlayer = player,
                    TennisSeason = season,
                    Rank = leader.rank,
                    Points = int.Parse(leader.stat),
                    Movement = 0,
                    DataProvider = DataProvider.Stats,
                    RankingValidLastAt = dateThroughDateTime
                };

                if(dbLeader == null)
                {
                    _publicSportDataUnitOfWork.TennisRankings.Add(newRanking);
                }
                else
                {
                    dbLeader.Rank = newRanking.Rank;
                    dbLeader.Points = newRanking.Points;
                    dbLeader.Movement = newRanking.Movement;
                    dbLeader.RankingValidLastAt = newRanking.RankingValidLastAt;

                    _publicSportDataUnitOfWork.TennisRankings.Update(dbLeader);
                }
            }

            await _publicSportDataUnitOfWork.SaveChangesAsync();
        }

        public IEnumerable<TennisEvent> GetEventsForLeague(int providerLeagueId, int seasonId)
        {
            return _publicSportDataUnitOfWork.TennisEventTennisLeagues.Where(
                        m => m.TennisLeague.ProviderLeagueId == providerLeagueId &&
                             m.TennisEvent.TennisSeason.ProviderSeasonId == seasonId)
                        .Select(m => m.TennisEvent).ToList();
        }

        public TennisEventTennisLeagues GetMappingForTennisEventTennisLeague(int providerEventId, int providerLeagueId)
        {
            return _publicSportDataUnitOfWork.TennisEventTennisLeagues.FirstOrDefault(m =>
                  m.TennisLeague.ProviderLeagueId == providerLeagueId &&
                  m.TennisEvent.ProviderEventId == providerEventId);
        }

        public async Task AddOrUpdateTennisEventSeeds(IEnumerable<Seed> seeds, TennisEvent dbTennisEvent)
        {
            foreach (var seed in seeds)
            {
                var playerId = seed.players.First().playerId;
                var player = _publicSportDataUnitOfWork.TennisPlayers.FirstOrDefault(p => p.ProviderPlayerId == playerId);

                if (player == null)
                    continue;

                var seedValue = int.Parse(seed.seedValue);

                var seededPlayerForEvent = _publicSportDataUnitOfWork.TennisEventSeeds.FirstOrDefault(
                    s => s.TennisPlayer.ProviderPlayerId == player.ProviderPlayerId &&
                         s.TennisEvent.ProviderEventId == dbTennisEvent.ProviderEventId);

                var newSeed = new TennisEventSeed()
                {
                    TennisEvent = dbTennisEvent,
                    TennisPlayer = player,
                    SeedValue = seedValue,
                    DataProvider = DataProvider.Stats,
                };

                if (seededPlayerForEvent == null)
                {
                    _publicSportDataUnitOfWork.TennisEventSeeds.Add(newSeed);
                }
                else
                {
                    seededPlayerForEvent.SeedValue = newSeed.SeedValue;
                    _publicSportDataUnitOfWork.TennisEventSeeds.Update(seededPlayerForEvent);
                }
            }

            await _publicSportDataUnitOfWork.SaveChangesAsync();
        }

        public async Task AddOrUpdateTennisMatch(List<Match> matches, MatchType matchType, TennisEventTennisLeagues associatedTennisEventTennisLeague)
        {
            var players = (await _publicSportDataUnitOfWork.TennisPlayers.AllAsync()).ToList();

            foreach (var match in matches)
            {
                var dbMatch = _publicSportDataUnitOfWork.TennisMatches.FirstOrDefault(m =>
                    m.ProviderMatchId == match.matchId);

                var matchDate = match.matchTime.First(d => d.dateType.ToLower().Equals("utc"));
                var makeupDate = match.makeupDate?.First(d => d.dateType.ToLower().Equals("utc"));

                var newMatch = new TennisMatch()
                {
                    ProviderMatchId = match.matchId,
                    NumberOfSets = match.sides?.First()?.linescore?.sets?.Count ?? 0,
                    RoundNumber = match.round,
                    RoundType = match.roundType.name,
                    AssociatedTennisEventTennisLeague = associatedTennisEventTennisLeague,
                    TennisMatchStatus = GetMatchStateFromProviderState(match.matchStatus.matchStatusId),
                    StartDateTimeUtc = new DateTimeOffset(matchDate.full, TimeSpan.FromHours(0)),
                    MakeupDateTimeUtc = makeupDate == null ? (DateTimeOffset?) null : new DateTimeOffset(makeupDate.full, TimeSpan.FromHours(0)),
                    DataProvider = DataProvider.Stats,
                    DrawNumber = matchType.draw
                };

                if (match.sides == null || match.sides.Count < 2)
                    return;

                newMatch.TennisSideA = GetSideWithSets(match.sides[0], 1, players);
                newMatch.TennisSideB = GetSideWithSets(match.sides[1], 2, players);

                var numberOfSets = match.sides[0].linescore?.sets.Count ?? 0;
                newMatch.TennisSets = new List<TennisSet>();

                for (var i = 1; i <= numberOfSets; i++)
                {
                    var setA = match.sides[0].linescore?.sets.FirstOrDefault(s => s.setNumber == i);
                    var setB = match.sides[1].linescore?.sets.FirstOrDefault(s => s.setNumber == i);

                    newMatch.TennisSets.Add(
                        new TennisSet()
                        {
                            TennisSideA = newMatch.TennisSideA,
                            TennisSideB = newMatch.TennisSideB,
                            SideAGamesWon = setA?.games ?? 0,
                            SideBGamesWon = setB?.games ?? 0,
                            SideBHasWon = setB?.result == "win",
                            SideAHasWon = setA?.result == "win",
                            SideATieBreakerPoints = setA?.tiebreak,
                            SideBTieBreakerPoints = setB?.tiebreak,
                            SetIsTieBreaker = setA?.tiebreak == null || setB?.tiebreak == null,
                            DataProvider = DataProvider.Stats,
                            SetNumber = i
                        });
                }

                if (dbMatch == null)
                {
                    _publicSportDataUnitOfWork.TennisMatches.Add(newMatch);
                }
                else
                {
                    dbMatch.NumberOfSets = newMatch.NumberOfSets;
                    dbMatch.RoundNumber = newMatch.RoundNumber;
                    dbMatch.RoundType = newMatch.RoundType;
                    dbMatch.AssociatedTennisEventTennisLeague = newMatch.AssociatedTennisEventTennisLeague;
                    dbMatch.TennisMatchStatus = newMatch.TennisMatchStatus;
                    dbMatch.StartDateTimeUtc = newMatch.StartDateTimeUtc;
                    dbMatch.MakeupDateTimeUtc = newMatch.MakeupDateTimeUtc;
                    dbMatch.DrawNumber = newMatch.DrawNumber;
                    
                    UpdateSetsForMatch(dbMatch, newMatch);
                    UpdateSidesForMatch(dbMatch, newMatch);

                    _publicSportDataUnitOfWork.TennisMatches.Update(dbMatch);
                }
            }

            await _publicSportDataUnitOfWork.SaveChangesAsync();

            await AddOrUpdateSchedulerTrackingTennisMatch(matches, matchType, associatedTennisEventTennisLeague);
        }

        private async Task AddOrUpdateSchedulerTrackingTennisMatch(List<Match> matches, MatchType matchType, TennisEventTennisLeagues associatedTennisEventTennisLeague)
        {
            foreach (var match in matches)
            {
                var dbMatch = _publicSportDataUnitOfWork.TennisMatches.FirstOrDefault(m =>
                    m.ProviderMatchId == match.matchId);

                if (dbMatch == null)
                    continue;

                var status = GetMatchStateFromProviderState(match.matchStatus.matchStatusId);

                var dbTrackingMatch =
                    (await _systemSportDataUnitOfWork.SchedulerTrackingTennisMatches.AllAsync()).FirstOrDefault(
                        t => t.TennisMatchId == dbMatch.Id);

                var newTrackingMatch = new SchedulerTrackingTennisMatch()
                {
                    TennisMatchId = dbMatch.Id,
                    StartDateTime = dbMatch.StartDateTimeUtc,
                    EndDateTime = HasMatchEnded(status) ? DateTimeOffset.UtcNow : (DateTimeOffset?) null,
                    SchedulerStateForTennisMatchPolling = 
                        HasMatchEnded(status) && ((DateTimeOffset.UtcNow - dbMatch.StartDateTimeUtc) > TimeSpan.FromHours(1))
                            ? SchedulerStateForTennisMatchPolling.PollingComplete 
                            : SchedulerStateForTennisMatchPolling.NotStarted
                };

                if (dbTrackingMatch == null)
                {
                    _systemSportDataUnitOfWork.SchedulerTrackingTennisMatches.Add(newTrackingMatch);
                }
                else
                {
                    dbTrackingMatch.StartDateTime = newTrackingMatch.StartDateTime;
                    dbTrackingMatch.EndDateTime = dbTrackingMatch.EndDateTime ?? newTrackingMatch.EndDateTime;

                    if(dbTrackingMatch.SchedulerStateForTennisMatchPolling == SchedulerStateForTennisMatchPolling.NotStarted)
                        dbTrackingMatch.SchedulerStateForTennisMatchPolling =
                            newTrackingMatch.SchedulerStateForTennisMatchPolling;

                    _systemSportDataUnitOfWork.SchedulerTrackingTennisMatches.Update(dbTrackingMatch);
                }
            }

            await _systemSportDataUnitOfWork.SaveChangesAsync();
        }

        private bool HasMatchEnded(TennisMatchStatus status)
        {
            return
                status == TennisMatchStatus.Final ||
                status == TennisMatchStatus.Suspended;
        }

        public TennisMatchStatus GetMatchStateFromProviderState(int matchStatusMatchStatusId)
        {
            switch (matchStatusMatchStatusId)
            {
                case 1: // Pre-Game
                    return TennisMatchStatus.PreGame;
                case 2: // In Progress
                    return TennisMatchStatus.InProgress;
                case 4: // Final
                    return TennisMatchStatus.Final;
                case 5: // Postponed
                    return TennisMatchStatus.Postponed;
                case 6: // Suspended
                    return TennisMatchStatus.Suspended;
                case 21: // Final after suspension
                    return TennisMatchStatus.Final;
                case 22: // Final after postponed
                    return TennisMatchStatus.Final;
                case 23: // Delayed
                    return TennisMatchStatus.PreGame;
                case 24: // Bye
                    return TennisMatchStatus.Bye;
            }

            return TennisMatchStatus.PreGame;
        }

        public TennisEvent GetTennisEventFor(int providerEventId)
        {
            return _publicSportDataUnitOfWork.TennisEvents.FirstOrDefault(e => e.ProviderEventId == providerEventId);
        }

        public TennisMatch GetTennisMatchFor(int providerMatchId)
        {
            return _publicSportDataUnitOfWork.TennisMatches.FirstOrDefault(m => m.ProviderMatchId == providerMatchId);
        }

        private void UpdateSidesForMatch(TennisMatch dbMatch, TennisMatch newMatch)
        {
            dbMatch.TennisSideA.HasSideWon = newMatch.TennisSideA.HasSideWon;
            dbMatch.TennisSideB.HasSideWon = newMatch.TennisSideB.HasSideWon;
            dbMatch.TennisSideA.SideNumber = newMatch.TennisSideA.SideNumber;
            dbMatch.TennisSideB.SideNumber = newMatch.TennisSideB.SideNumber;
        }

        private static void UpdateSetsForMatch(TennisMatch dbMatch, TennisMatch newMatch)
        {
            if(dbMatch.TennisSets == null)
                dbMatch.TennisSets = new List<TennisSet>();

            foreach (var newMatchTennisSet in newMatch.TennisSets)
            {
                // Try find this set in the db.
                var dbSet = dbMatch.TennisSets?.FirstOrDefault(s => s.SetNumber == newMatchTennisSet.SetNumber);
                if (dbSet == null)
                {
                    dbMatch.TennisSets.Add(newMatchTennisSet);
                }
                else
                {
                    dbSet.SideAGamesWon = newMatchTennisSet.SideAGamesWon;
                    dbSet.SideAHasWon = newMatchTennisSet.SideAHasWon;
                    dbSet.SideBGamesWon = newMatchTennisSet.SideBGamesWon;
                    dbSet.SideBHasWon = newMatchTennisSet.SideBHasWon;
                    dbSet.SideATieBreakerPoints = newMatchTennisSet.SideATieBreakerPoints;
                    dbSet.SideBTieBreakerPoints = newMatchTennisSet.SideBTieBreakerPoints;
                    dbSet.SetIsTieBreaker = newMatchTennisSet.SetIsTieBreaker;
                }
            }
        }

        private static TennisSide GetSideWithSets(Side side, int sideNumber, IEnumerable<TennisPlayer> players)
        {
            var p1Id = side.players.First().playerId;

            var sideObject = new TennisSide()
            {
                SideNumber = sideNumber,
                TennisPlayerA = players.FirstOrDefault(p => p.ProviderPlayerId == p1Id),
                DataProvider = DataProvider.Stats,
                TennisPlayerB = null,
                HasSideWon = side.isWinner,
                SideStatus = side.reason
            };

            return sideObject;
        }
    }
}
