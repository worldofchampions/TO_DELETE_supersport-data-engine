using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.ResponseModels;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.MongoDb.PayloadData.Interfaces
{
    public interface IMongoDbRugbyRepository
    {
        void Save(RugbyEntitiesResponse entities);
        void Save(RugbyFixturesResponse fixtures);
        void Save(RugbyFlatLogsResponse logs);
    }
}
