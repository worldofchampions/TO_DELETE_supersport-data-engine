namespace SuperSportDataEngine.ApplicationLogic.Services
{
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.ResponseModels;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.MongoDb.PayloadData.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Interfaces;
    using System;
    using System.Linq;

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

        public RugbyEntitiesResponse IngestRugbyReferenceData()
        {
            var entitiesResponse = _statsProzoneIngestService.IngestRugbyReferenceData();

            PersistSportTournamentsInrepository(entitiesResponse);

            _mongoDbRepository.Save(entitiesResponse);
            return entitiesResponse;
        }

        private void PersistSportTournamentsInrepository(RugbyEntitiesResponse entitiesResponse)
        {
            foreach(var competition in entitiesResponse.Entities.competitions)
            {
                var entry = _sportTournamentRepository
                    .Where(c => c.TournamentIndex == competition.id)
                    .FirstOrDefault();

                var newEntry = new SportTournament
                {
                    TournamentIndex = competition.id,
                    TournamentName = competition.name,
                    IsEnabled = false
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

            _sportTournamentRepository.SaveAsync();
        }
    }
}
