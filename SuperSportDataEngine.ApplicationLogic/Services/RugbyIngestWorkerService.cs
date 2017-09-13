namespace SuperSportDataEngine.ApplicationLogic.Services
{
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.ResponseModels;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.MongoDb.PayloadData.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Interfaces;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Threading;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;

    public class RugbyIngestWorkerService : IRugbyIngestWorkerService
    {
        private readonly IStatsProzoneRugbyIngestService _statsProzoneIngestService;
        private readonly IMongoDbRugbyRepository _mongoDbRepository;
        private readonly IBaseEntityFrameworkRepository<RugbyTournament> _rugbyTournamentRepository;
        private readonly IBaseEntityFrameworkRepository<RugbySeason> _rugbySeasonRepository;

        public RugbyIngestWorkerService(
            IStatsProzoneRugbyIngestService statsProzoneIngestService,
            IMongoDbRugbyRepository mongoDbRepository,
            IBaseEntityFrameworkRepository<RugbyTournament> rugbyTournamentRepository,
            IBaseEntityFrameworkRepository<RugbySeason> rugbySeasonRepository)
        {
            _statsProzoneIngestService = statsProzoneIngestService;
            _mongoDbRepository = mongoDbRepository;
            _rugbyTournamentRepository = rugbyTournamentRepository;
            _rugbySeasonRepository = rugbySeasonRepository;
        }

        public void IngestRugbyReferenceData(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            var entitiesResponse =
                _statsProzoneIngestService.IngestRugbyReferenceData(cancellationToken);

            PersistRugbyTournamentsInRepositoryAsync(entitiesResponse, cancellationToken);
            IngestRugbyTournamentSeasons(cancellationToken);

            _mongoDbRepository.Save(entitiesResponse);
        }

        private void IngestRugbyTournamentSeasons(CancellationToken cancellationToken)
        {
            var activeTournaments = _rugbyTournamentRepository.Where(t => t.IsEnabled).ToList();
            foreach (var tournament in activeTournaments)
            {
                var season = _statsProzoneIngestService.IngestSeasonData(cancellationToken, tournament.ProviderTournamentId, DateTime.Now.Year);

                PersistRugbySeasonDataToSystemSportsDataRepository(cancellationToken, season);
            }
        }

        private void PersistRugbySeasonDataToSystemSportsDataRepository(CancellationToken cancellationToken, RugbySeasonResponse season)
        {
            //var providerTournamentId = season.RugbySeasons.competitionId;
            //var providerSeasonId = season.RugbySeasons.season.First().id;
            //var isSeasonCurrentlyActive = season.RugbySeasons.season.First().currentSeason;

            //var seasonEntry =
            //        _rugbySeasonRepository.All()
            //        .Where(s => s.RugbyTournament.ProviderTournamentId == providerTournamentId && s.ProviderSeasonId == providerSeasonId).ToList()
            //        .FirstOrDefault();

            //var tour = _rugbyTournamentRepository.Where(t => t.ProviderTournamentId == providerTournamentId).ToList().FirstOrDefault();
            //var newEntry = new RugbySeason()
            //{
            //    ProviderSeasonId = providerSeasonId,
            //    //RugbyTournament = tour,
            //    IsCurrent = isSeasonCurrentlyActive,
            //    Name = season.RugbySeasons.season.First().name
            //};

            //// Not in repo?
            //if (seasonEntry == null)
            //{
            //    _rugbySeasonRepository.Add(newEntry);
            //}
            //else
            //{
            //    _rugbySeasonRepository.Update(newEntry);
            //}

            //try
            //{
            //    cancellationToken.ThrowIfCancellationRequested();
            //    _rugbySeasonRepository.SaveAsync();
            //}
            //catch (Exception e)
            //{
            //    throw e;
            //}
        }

        public void IngestFixturesForActiveTournaments(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            var activeTournaments =
                _rugbyTournamentRepository.Where(t => t.IsEnabled);

            foreach (var tournament in activeTournaments)
            {
                var fixtures =
                    _statsProzoneIngestService.IngestFixturesForTournament(
                        tournament, cancellationToken);

                // TODO: Also persist in SQL DB.

                _mongoDbRepository.Save(fixtures);
            }
        }

        public void IngestLogsForActiveTournaments(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            var activeTournaments =
                _rugbyTournamentRepository.Where(t => t.IsEnabled);

            foreach (var tournament in activeTournaments)
            {
                var logs =
                    _statsProzoneIngestService.IngestLogsForTournament(
                        tournament, cancellationToken);

                // TODO: Also persist in SQL DB.

                _mongoDbRepository.Save(logs);
            }
        }

        private void PersistRugbyTournamentsInRepositoryAsync(RugbyEntitiesResponse entitiesResponse, CancellationToken cancellationToken)
        {
            foreach (var competition in entitiesResponse.Entities.competitions)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                var entry = _rugbyTournamentRepository
                    .Where(c => c.ProviderTournamentId == competition.id)
                    .FirstOrDefault();

                var newEntry = new RugbyTournament
                {
                    Id = entry != null && entry.Id != null ? entry.Id : Guid.NewGuid(),
                    ProviderTournamentId = competition.id,
                    Name = competition.name,
                    IsEnabled = entry != null,
                    LogoUrl = competition.CompetitionLogoURL,
                    Abbreviation = competition.CompetitionAbbrev,
                    Slug = "/competition/" + competition.id,
                    LegacyTournamentId = competition.id
                };

                if (entry == null)
                {
                    _rugbyTournamentRepository.Add(newEntry);
                }
                else
                {
                    _rugbyTournamentRepository.Update(newEntry);
                }
            }

            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                _rugbyTournamentRepository.SaveAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void IngestRugbyResultsForAllFixtures(CancellationToken cancellationToken)
        {
            var activeTournaments = _rugbyTournamentRepository.Where(t => t.IsEnabled);

            foreach (var tournament in activeTournaments)
            {
                var results = _statsProzoneIngestService.IngestFixturesForTournament(tournament, cancellationToken);
                PersistFixturesResultsInRepository(results, cancellationToken);
            }
        }

        private void PersistFixturesResultsInRepository(RugbyFixturesResponse allResults, CancellationToken cancellationToken)
        {
            // Only persist data for completed matches.
            // The provider endpoint for results is just a variation of the fixtures endpoint,
            // It will also return results for completed matches.
        }
    }
}
