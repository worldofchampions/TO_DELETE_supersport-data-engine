using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.Stats.Models.Tennis.TennisLeaguesResponse;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.MongoDb.PayloadData.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Gateway.Http.Stats.Models.Tennis;
    using Gateway.Http.Stats.Models.Tennis.TennisLeagueTournamentsResponse;
    using Gateway.Http.Stats.Models.Tennis.TennisParticipantsResponse;
    using Gateway.Http.Stats.Models.Tennis.TennisRankingsResponse;
    using Gateway.Http.Stats.Models.Tennis.TennisSurfaceTypesResponse;
    using Gateway.Http.Stats.Models.Tennis.TennisVenuesResponse;
    using EntityFramework.PublicSportData.Models.Tennis;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.Stats.Models.Tennis.TennisResultsResponse;

    public interface IMongoDbTennisRepository
    {
        Task Save(TennisLeaguesResponse leagues);
        Task Save(TennisLeagueTournamentsResponse apiResponse);
        Task Save(TennisRankingsResponse apiResponse);
        Task Save(TennisSeasonsResponse seasonsResponse);
        Task Save(TennisVenuesResponse venuesFromProvider);
        Task Save(TennisSurfaceTypesResponse surfaceTypesFromProvider);
        Task Save(TennisParticipantsResponse participantsForLeague);
        Task Save(TennisResultsResponse results);
    }
}
