using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
using SuperSportDataEngine.ApplicationLogic.Entities.SystemAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.CmsLogic.Interfaces
{
    public interface IRugbyCmsService
    {
        Task<IEnumerable<RugbyTournamentEntity>> GetAllTournaments(int pageIndex, int pageSize);
        Task<RugbyTournamentEntity> GetTournamentById(int id);
        Task<bool> UpdateTournament(int tournamentId, RugbyTournamentEntity rugbyTournamentEntity);
    }
}
