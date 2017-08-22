namespace SuperSportDataEngine.ApplicationLogic.Services
{
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.ResponseModels;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.MongoDb.PayloadData.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;

    public class IngestWorkerService : IIngestWorkerService
    {
        private readonly IStatsProzoneRugbyIngestService _statsProzoneIngestService;
        private readonly IMongoDbRugbyRepository _mongoDbRepository;

        public IngestWorkerService(IStatsProzoneRugbyIngestService statsProzoneIngestService, IMongoDbRugbyRepository mongoDbRepository)
        {
            _statsProzoneIngestService = statsProzoneIngestService;
            _mongoDbRepository = mongoDbRepository;
        }

        public RugbyEntitiesResponse IngestReferenceData()
        {
            var entitiesResponse = _statsProzoneIngestService.IngestRugbyReferenceData();
            _mongoDbRepository.Save(entitiesResponse);
            return entitiesResponse;
        }
    }
}
