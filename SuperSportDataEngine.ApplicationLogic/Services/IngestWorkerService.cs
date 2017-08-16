using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.MongoDb.PayloadData.Interfaces;
using SuperSportDataEngine.Boundaries.ApplicationLogic.Interfaces;
using SuperSportDataEngine.Gateway.Http.StatsProzone.Models;

namespace SuperSportDataEngine.ApplicationLogic.Services
{
    public class IngestWorkerService : IIngestWorkerService
    {
        private readonly IStatsProzoneRequestService _statsProzoneIngestService;
        private readonly IMongoDbRepository _mongoDbRepository;

        public IngestWorkerService(IStatsProzoneRequestService statsProzoneIngestService, IMongoDbRepository mongoDbRepository)
        {
            _statsProzoneIngestService = statsProzoneIngestService;
            _mongoDbRepository = mongoDbRepository;
        }

        public void IngestReferenceData()
        {
            var entities = _statsProzoneIngestService.RequestReferenceData();
            _mongoDbRepository.Save(entities);
        }
    }
}
