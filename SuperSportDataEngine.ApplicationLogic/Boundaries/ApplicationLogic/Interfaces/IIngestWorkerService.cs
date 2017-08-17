namespace SuperSportDataEngine.ApplicationLogic.Services
{
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.ResponseModels;

    public interface IIngestWorkerService
    {
        EntitiesResponse IngestReferenceData();
    }
}
