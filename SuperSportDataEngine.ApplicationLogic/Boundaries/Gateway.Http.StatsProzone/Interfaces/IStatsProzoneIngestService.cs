namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Interfaces
{
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.ResponseModels;

    public interface IStatsProzoneIngestService
    {
        RugbyEntitiesResponse IngestReferenceData();
    }
}
