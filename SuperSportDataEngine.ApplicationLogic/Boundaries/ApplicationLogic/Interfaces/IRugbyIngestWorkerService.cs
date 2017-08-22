namespace SuperSportDataEngine.ApplicationLogic.Services
{
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.ResponseModels;

    public interface IRugbyIngestWorkerService
    {
        RugbyEntitiesResponse IngestRugbyReferenceData();
    }
}
