using SuperSportDataEngine.ApplicationLogic.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces
{
    public interface IRugbyService
    {
        Task<IEnumerable<LogEntity>> GetLogs(string tournamentName);
    }
}