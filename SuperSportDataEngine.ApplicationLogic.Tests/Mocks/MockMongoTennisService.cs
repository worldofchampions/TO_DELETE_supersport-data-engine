using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.Stats.Models.Tennis;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.Stats.Models.Tennis.TennisLeaguesResponse;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.Stats.Models.Tennis.TennisLeagueTournamentsResponse;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.Stats.Models.Tennis.TennisParticipantsResponse;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.Stats.Models.Tennis.TennisRankingsResponse;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.Stats.Models.Tennis.TennisResultsResponse;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.Stats.Models.Tennis.TennisSurfaceTypesResponse;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.Stats.Models.Tennis.TennisVenuesResponse;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.MongoDb.PayloadData.Interfaces;

namespace SuperSportDataEngine.ApplicationLogic.Tests.Mocks
{
    class MockMongoTennisService : IMongoDbTennisRepository
    {
        public async Task Save(TennisLeaguesResponse leagues)
        {
        }

        public async Task Save(TennisLeagueTournamentsResponse apiResponse)
        {
        }

        public async Task Save(TennisRankingsResponse apiResponse)
        {
        }

        public async Task Save(TennisSeasonsResponse seasonsResponse)
        {
        }

        public async Task Save(TennisVenuesResponse venuesFromProvider)
        {
        }

        public async Task Save(TennisSurfaceTypesResponse surfaceTypesFromProvider)
        {
        }

        public async Task Save(TennisParticipantsResponse participantsForLeague)
        {
        }

        public async Task Save(TennisResultsResponse results)
        {
        }
    }
}
