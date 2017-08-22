using SuperSportDataEngine.ApplicationLogic.Entities;
using System.Collections.Generic;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces
{
    public interface IRugbyService
    {
        IEnumerable<LogEntity> GetLogs();
    }
}