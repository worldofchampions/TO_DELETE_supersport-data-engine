namespace SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces
{
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.ResponseModels;

    public interface IStatsProzoneRugbyIngestService
    {
        RugbyEntitiesResponse IngestRugbyReferenceData();
    }
}
