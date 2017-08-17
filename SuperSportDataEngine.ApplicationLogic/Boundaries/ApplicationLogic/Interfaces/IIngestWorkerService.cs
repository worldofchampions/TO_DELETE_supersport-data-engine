namespace SuperSportDataEngine.ApplicationLogic.Services
{
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.ResponseModels;
    using SuperSportDataEngine.Gateway.Http.StatsProzone.Models;

    public interface IIngestWorkerService
    {
        EntitiesResponse IngestReferenceData();
    }
}
