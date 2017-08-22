namespace SuperSportDataEngine.ApplicationLogic.Services
{
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.ResponseModels;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.MongoDb.PayloadData.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Interfaces;

    public class IngestWorkerService : IIngestWorkerService
    {
        private readonly IStatsProzoneIngestService _statsProzoneIngestService;
        private readonly IMongoDbRepository _mongoDbRepository;

        public IngestWorkerService(IStatsProzoneIngestService statsProzoneIngestService, IMongoDbRepository mongoDbRepository)
        {
            _statsProzoneIngestService = statsProzoneIngestService;
            _mongoDbRepository = mongoDbRepository;
        }

        public RugbyEntitiesResponse IngestReferenceData()
        {
            var entitiesResponse = _statsProzoneIngestService.IngestReferenceData();
            _mongoDbRepository.Save(entitiesResponse);
            return entitiesResponse;
        }
    }
}
