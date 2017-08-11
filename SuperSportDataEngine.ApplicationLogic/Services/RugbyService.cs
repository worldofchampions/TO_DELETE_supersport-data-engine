using System;
using System.Collections.Generic;
using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Entities;

namespace SuperSportDataEngine.ApplicationLogic.Services
{
    public class RugbyService : IRugbyService
    {
        public List<LogEntity> GetLogs()
        {
            return new List<LogEntity>();
        }
    }
}