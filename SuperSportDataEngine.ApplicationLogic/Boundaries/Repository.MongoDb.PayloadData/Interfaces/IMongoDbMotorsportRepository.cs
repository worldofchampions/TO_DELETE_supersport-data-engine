namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.MongoDb.PayloadData.Interfaces
{
    using System.Threading.Tasks;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.Stats.Models.Motorsport;

    public interface IMongoDbMotorsportRepository
    {
        Task Save(MotorsportEntitiesResponse leagues);
    }
}
