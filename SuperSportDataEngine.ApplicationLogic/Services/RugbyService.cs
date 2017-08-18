using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Entities;
using System.Collections.Generic;

namespace SuperSportDataEngine.ApplicationLogic.Services
{
    public class RugbyService : IRugbyService
    {
        public List<LogEntity> GetLogs()
        {
            //TODO: Get data from the DB
            return new List<LogEntity>();
        }
    }
}