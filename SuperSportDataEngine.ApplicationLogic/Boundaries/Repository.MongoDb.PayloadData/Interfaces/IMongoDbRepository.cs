using SuperSportDataEngine.Gateway.Http.StatsProzone.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.MongoDb.PayloadData.Interfaces
{
    public interface IMongoDbRepository
    {
        void Save(Entities entities);
    }
}
