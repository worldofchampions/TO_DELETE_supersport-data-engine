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
        IBaseEntityFrameworkRepository<RugbyPlayerStatistics> RugbyPlayerStatistics { get; }

        IBaseEntityFrameworkRepository<RugbyPlayerLineup> RugbyPlayerLineups { get; }
        IBaseEntityFrameworkRepository<RugbySeason> RugbySeasons { get; }
        IBaseEntityFrameworkRepository<RugbyTeam> RugbyTeams { get; }
        IBaseEntityFrameworkRepository<RugbyTournament> RugbyTournaments { get; }
        IBaseEntityFrameworkRepository<RugbyVenue> RugbyVenues { get; }

        IBaseEntityFrameworkRepository<MotorsportDriver> MotorsportDrivers { get; }
        IBaseEntityFrameworkRepository<MotorsportDriverStanding> MotorsportDriverStandings { get; }
        IBaseEntityFrameworkRepository<MotorsportRaceGrid> MotorsportRaceGrids { get; }
        IBaseEntityFrameworkRepository<MotorsportLeague> MotorsportLeagues { get; }
        IBaseEntityFrameworkRepository<MotorsportRace> MotorsportRaces { get; }
        IBaseEntityFrameworkRepository<MotorsportRaceResult> MotorsportRaceResults { get; }
        IBaseEntityFrameworkRepository<MotorsportSeason> MotorsportSeasons { get; }
        IBaseEntityFrameworkRepository<MotorsportTeam> MotortsportTeams { get; }
        IBaseEntityFrameworkRepository<MotorsportTeamStanding> MotorsportTeamStandings { get; }
        IBaseEntityFrameworkRepository<MotorsportRaceCalendar> MotorsportRaceCalendars { get; set; }

        int SaveChanges();
        Task<int> SaveChangesAsync();
    }
}
