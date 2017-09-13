using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
using SuperSportDataEngine.ApplicationLogic.Entities;
using System.Collections.Generic;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces
{
    public interface IRugbyService
    {
        IEnumerable<LogEntity> GetLogs(string tournamentName);
        IEnumerable<RugbyTournament> GetActiveTournaments();
        IEnumerable<RugbyTournament> GetCurrentTournaments();
   }
}