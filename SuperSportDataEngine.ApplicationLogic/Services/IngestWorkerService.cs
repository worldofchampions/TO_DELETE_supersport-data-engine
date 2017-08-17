using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.ResponseModels;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.MongoDb.PayloadData.Interfaces;
using SuperSportDataEngine.Boundaries.ApplicationLogic.Interfaces;
using SuperSportDataEngine.Gateway.Http.StatsProzone.Models;

namespace SuperSportDataEngine.ApplicationLogic.Services
{
    public class IngestWorkerService : IIngestWorkerService
    {
        private readonly IStatsProzoneIngestService _statsProzoneIngestService;
        private readonly IMongoDbRepository _mongoDbRepository;

        public IngestWorkerService(IStatsProzoneIngestService statsProzoneIngestService, IMongoDbRepository mongoDbRepository)
        {
            _statsProzoneIngestService = statsProzoneIngestService;
            _mongoDbRepository = mongoDbRepository;
        }

        public EntitiesResponse IngestReferenceData()
        {
            var entitiesResponse = _statsProzoneIngestService.IngestReferenceData();
            _mongoDbRepository.Save(entitiesResponse);
            return entitiesResponse;
        }
    }
}
