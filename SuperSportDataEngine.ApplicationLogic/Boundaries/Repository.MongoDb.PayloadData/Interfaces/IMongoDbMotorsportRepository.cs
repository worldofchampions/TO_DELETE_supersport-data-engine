using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Models.Motorsport;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.MongoDb.PayloadData.Interfaces
{
    public interface IMongoDbMotorsportRepository
    {
        Task Save(MotorsportEntitiesResponse leagues);
    }
}
