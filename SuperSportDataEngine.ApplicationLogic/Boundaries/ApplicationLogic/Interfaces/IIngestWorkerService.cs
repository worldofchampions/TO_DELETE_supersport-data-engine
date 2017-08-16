namespace SuperSportDataEngine.ApplicationLogic.Services
{
    using SuperSportDataEngine.Gateway.Http.StatsProzone.Models;

    public interface IIngestWorkerService
    {
        void IngestReferenceData();
    }
}
