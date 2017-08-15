namespace SuperSportDataEngine.Boundaries.ApplicationLogic.Interfaces
{
    using SuperSportDataEngine.Gateway.Http.StatsProzone.Models;

    public interface IStatsProzoneIngestService
    {
        Entities IngestReferenceData();
    }
}
