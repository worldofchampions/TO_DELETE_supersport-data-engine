using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.ResponseModels;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.MongoDb.PayloadData.Interfaces
{
    public interface IMongoDbRepository
    {
        void Save(EntitiesResponse entities);
    }
}
