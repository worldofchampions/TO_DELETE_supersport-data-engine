namespace SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces
{
    using System.Threading;
    using System.Threading.Tasks;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.Stats.Models.Motorsport;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;

    public interface IMotorsportStorageService
    {
        Task PersistLeagueDriversInRepository(MotorsportEntitiesResponse providerResponse, MotorsportLeague league);

        Task PersistRacesInRepository(
            MotorsportEntitiesResponse providerResponse,
            MotorsportLeague league,
            CancellationToken cancellationToken);

        Task PersistRaceEventsInRepository(
            MotorsportEntitiesResponse providerResponse,
            MotorsportRace race,
            MotorsportSeason season,
            CancellationToken cancellationToken);

        Task PersistLeaguesInRepository(
            MotorsportEntitiesResponse providerResponse,
            CancellationToken cancellationToken);

        Task PersistSeasonsInRepository(
            MotorsportEntitiesResponse providerResponse,
            MotorsportLeague league,
            CancellationToken cancellationToken);

        Task PersistDriverStandingsInRepository(
            MotorsportEntitiesResponse providerResponse,
            MotorsportLeague league,
            MotorsportSeason season,
            CancellationToken cancellationToken);

        Task PersistTeamsInRepository(MotorsportEntitiesResponse response, MotorsportLeague league);

        Task PersistTeamStandingsInRepository(
            MotorsportEntitiesResponse providerResponse,
            MotorsportLeague league,
            MotorsportSeason season,
            CancellationToken cancellationToken);

        Task PersistResultsInRepository(
            MotorsportEntitiesResponse response,
            MotorsportRaceEvent raceEvent,
            MotorsportLeague league);

        Task PersistGridInRepository(
            MotorsportEntitiesResponse response,
            MotorsportRaceEvent raceEvent,
            MotorsportLeague league);
    }

}
