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

    public class RugbyIngestWorkerService : IRugbyIngestWorkerService
    {
        private readonly IStatsProzoneRugbyIngestService _statsProzoneIngestService;
        private readonly IMongoDbRugbyRepository _mongoDbRepository;
        private readonly IBaseEntityFrameworkRepository<SportTournament> _sportTournamentRepository;

        public RugbyIngestWorkerService(
            IStatsProzoneRugbyIngestService statsProzoneIngestService, 
            IMongoDbRugbyRepository mongoDbRepository,
            IBaseEntityFrameworkRepository<SportTournament> sportTournamentRepository)
        {
            _statsProzoneIngestService = statsProzoneIngestService;
            _mongoDbRepository = mongoDbRepository;
            _sportTournamentRepository = sportTournamentRepository;
        }

        public async Task IngestRugbyReferenceData(CancellationToken cancellationToken)
        {
            // This is for delaying the job. 
            // Testing whether the job runs for an extended period of time.
            //cancellationToken.WaitHandle.WaitOne(TimeSpan.FromHours(4.5));

            if (cancellationToken.IsCancellationRequested)
                return;

            var entitiesResponse = 
                _statsProzoneIngestService.IngestRugbyReferenceData(cancellationToken);

            await PersistSportTournamentsInRepositoryAsync(entitiesResponse, cancellationToken);

            _mongoDbRepository.Save(entitiesResponse);
        }

        public async Task IngestFixturesForActiveTournaments(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            var activeTournaments = 
                _sportTournamentRepository.Where(t => t.IsEnabled);

            foreach (var tournament in activeTournaments)
            {
                var fixtures = 
                    _statsProzoneIngestService.IngestFixturesForTournament(
                        tournament, cancellationToken);
            }
        }

        private async Task PersistSportTournamentsInRepositoryAsync(RugbyEntitiesResponse entitiesResponse, CancellationToken cancellationToken)
        {
            foreach (var competition in entitiesResponse.Entities.competitions)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                var entry = _sportTournamentRepository
                    .Where(c => c.TournamentIndex == competition.id)
                    .FirstOrDefault();

                var newEntry = new SportTournament
                {
                    TournamentIndex = competition.id,
                    TournamentName = competition.name,
                    IsEnabled = entry != null
                };

                if (entry == null)
                {
                    _sportTournamentRepository.Add(newEntry);
                }
                else
                {
                    _sportTournamentRepository.Update(newEntry);
                }
            }

            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                await _sportTournamentRepository.SaveAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task IngestRugbyResultsForAllFixtures(CancellationToken cancellation)
        {
            Thread.Sleep(1000);
            return;
        }
    }
}
