using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.UnitOfWork
{
    public interface IPublicSportDataUnitOfWork
    {
        IBaseEntityFrameworkRepository<RugbyCommentary> RugbyCommentaries { get; set; }
        IBaseEntityFrameworkRepository<RugbyEventType> RugbyEventTypes { get; set; }
        IBaseEntityFrameworkRepository<RugbyEventTypeProviderMapping> RugbyEventTypeProviderMappings { get; set; }
        IBaseEntityFrameworkRepository<RugbyFixture> RugbyFixtures { get; set; }
        IBaseEntityFrameworkRepository<RugbyFlatLog> RugbyFlatLogs { get; set; }
        IBaseEntityFrameworkRepository<RugbyGroupedLog> RugbyGroupedLogs { get; set; }
        IBaseEntityFrameworkRepository<RugbyLogGroup> RugbyLogGroups { get; set; }
        IBaseEntityFrameworkRepository<RugbyMatchEvent> RugbyMatchEvents { get; set; }
        IBaseEntityFrameworkRepository<RugbyMatchStatistics> RugbyMatchStatistics { get; set; }
        IBaseEntityFrameworkRepository<RugbyPlayer> RugbyPlayers { get; set; }
        IBaseEntityFrameworkRepository<RugbyPlayerLineup> RugbyPlayerLineups { get; set; }
        IBaseEntityFrameworkRepository<RugbySeason> RugbySeasons { get; set; }
        IBaseEntityFrameworkRepository<RugbyTeam> RugbyTeams { get; set; }
        IBaseEntityFrameworkRepository<RugbyTournament> RugbyTournaments { get; set; }
        IBaseEntityFrameworkRepository<RugbyVenue> RugbyVenues { get; set; }

        int SaveChanges();
    }
}
