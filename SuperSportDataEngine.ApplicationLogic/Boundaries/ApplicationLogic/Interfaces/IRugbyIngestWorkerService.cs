namespace SuperSportDataEngine.ApplicationLogic.Services
{
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.ResponseModels;
    using System.Threading.Tasks;

    public interface IRugbyIngestWorkerService
    {
        Task<RugbyEntitiesResponse> IngestRugbyReferenceData();
    }
}
