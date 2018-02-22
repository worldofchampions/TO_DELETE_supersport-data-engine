using System;
using System.Threading.Tasks;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.UnitOfWork
{
    public interface IPublicSportDataUnitOfWork : IDisposable
    {
        IBaseEntityFrameworkRepository<RugbyCommentary> RugbyCommentaries { get; }
        IBaseEntityFrameworkRepository<RugbyEventType> RugbyEventTypes { get; }
        IBaseEntityFrameworkRepository<RugbyEventTypeProviderMapping> RugbyEventTypeProviderMappings { get; }
        IBaseEntityFrameworkRepository<RugbyFixture> RugbyFixtures { get; }
        IBaseEntityFrameworkRepository<RugbyFlatLog> RugbyFlatLogs { get; }
        IBaseEntityFrameworkRepository<RugbyGroupedLog> RugbyGroupedLogs { get; }
        IBaseEntityFrameworkRepository<RugbyLogGroup> RugbyLogGroups { get; }
        IBaseEntityFrameworkRepository<RugbyMatchEvent> RugbyMatchEvents { get; }
        IBaseEntityFrameworkRepository<RugbyMatchStatistics> RugbyMatchStatistics { get; }
        IBaseEntityFrameworkRepository<RugbyPlayer> RugbyPlayers { get; }
        IBaseEntityFrameworkRepository<RugbyPlayerLineup> RugbyPlayerLineups { get; }
        IBaseEntityFrameworkRepository<RugbySeason> RugbySeasons { get; }
        IBaseEntityFrameworkRepository<RugbyTeam> RugbyTeams { get; }
        IBaseEntityFrameworkRepository<RugbyTournament> RugbyTournaments { get; }
        IBaseEntityFrameworkRepository<RugbyVenue> RugbyVenues { get; }

        IBaseEntityFrameworkRepository<MotorsportDriver> MotorsportDrivers { get; set; }
        IBaseEntityFrameworkRepository<MotorsportDriverStanding> MotorsportDriverStandings { get; set; }
        IBaseEntityFrameworkRepository<MotorsportGrid> MotorsportGrids { get; set; }
        IBaseEntityFrameworkRepository<MotorsportLeague> MotorsportLeagues { get; set; }
        IBaseEntityFrameworkRepository<MotorsportRace> MotorsportRaces { get; set; }
        IBaseEntityFrameworkRepository<MotorsportRaceResult> MotorsportRaceResults { get; set; }
        IBaseEntityFrameworkRepository<MotorsportSeason> MotorsportSeasons { get; set; }
        IBaseEntityFrameworkRepository<MotorsportTeam> MotortsportTeams { get; set; }
        IBaseEntityFrameworkRepository<MotorsportTeamStanding> MotorsportTeamStandings { get; set; }

        int SaveChanges();
        Task<int> SaveChangesAsync();
    }
}
