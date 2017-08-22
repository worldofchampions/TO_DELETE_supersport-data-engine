namespace SuperSportDataEngine.ApplicationLogic.Services
{
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.ResponseModels;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.MongoDb.PayloadData.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Interfaces;

    public class RugbyIngestWorkerService : IRugbyIngestWorkerService
    {
        private readonly IStatsProzoneRugbyIngestService _statsProzoneIngestService;
        private readonly IMongoDbRugbyRepository _mongoDbRepository;

        public RugbyIngestWorkerService(IStatsProzoneRugbyIngestService statsProzoneIngestService, IMongoDbRugbyRepository mongoDbRepository)
        {
            _statsProzoneIngestService = statsProzoneIngestService;
            _mongoDbRepository = mongoDbRepository;
        }

        public RugbyEntitiesResponse IngestRugbyReferenceData()
        {
            var entitiesResponse = _statsProzoneIngestService.IngestRugbyReferenceData();
            _mongoDbRepository.Save(entitiesResponse);
            return entitiesResponse;
        }
    }
}
