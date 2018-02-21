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

        IBaseEntityFrameworkRepository<MotorDriver> MotorDrivers { get; set; }
        IBaseEntityFrameworkRepository<MotorLeague> MotorLeagues { get; set; }
        IBaseEntityFrameworkRepository<MotorRace> MotorRaces { get; set; }
        IBaseEntityFrameworkRepository<MotorDriverStanding> MotorDriverStandings { get; set; }
        IBaseEntityFrameworkRepository<MotorTeamStanding> MotorTeamStandings { get; set; }
        IBaseEntityFrameworkRepository<MotorTeam> MotortTeams { get; set; }
        IBaseEntityFrameworkRepository<MotorRaceResult> MotorTeamResults { get; set; }
        IBaseEntityFrameworkRepository<MotorRaceCalendar> MotorSchedules { get; set; }
        IBaseEntityFrameworkRepository<MotorGrid> MotorGrids { get; set; }

        int SaveChanges();
        Task<int> SaveChangesAsync();
    }
}
