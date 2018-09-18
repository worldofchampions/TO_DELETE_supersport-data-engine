using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.Stats.Models.Tennis.TennisLeaguesResponse;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.StatsProzone.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.UnitOfWork;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.UnitOfWork;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.MongoDb.PayloadData.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Services.Tennis;
using SuperSportDataEngine.ApplicationLogic.Tests.Mocks;
using SuperSportDataEngine.Common.Logging;
using SuperSportDataEngine.Tests.Common.Repositories.Test;

namespace SuperSportDataEngine.ApplicationLogic.Tests.Services.Tennis
{
    [TestFixture]
    public class TennisIngestWorkerServiceTests
    {
        private IStatsTennisIngestService _mockStatsTennisIngestService;
        private TestPublicSportDataUnitOfWork _mockPublicSportDataUnitOfWork;
        private TestSystemSportDataUnitOfWork _mockSystemSportDataUnitOfWork;
        private ITennisStorageService _tennisStorageService;
        private Mock<ILoggingService> _mockLoggingService;
        private MockMongoTennisService _mockMongoDbTennisRepository;

        [SetUp]
        public void SetUp()
        {
            _mockStatsTennisIngestService = new MockStatsIngestService();
            _mockPublicSportDataUnitOfWork = new TestPublicSportDataUnitOfWork();
            _mockSystemSportDataUnitOfWork = new TestSystemSportDataUnitOfWork();

            _tennisStorageService = new TennisStorageService(
                _mockPublicSportDataUnitOfWork, 
                _mockSystemSportDataUnitOfWork);

            _mockLoggingService = new Mock<ILoggingService>();
            _mockMongoDbTennisRepository = new MockMongoTennisService();
        }

        [TearDown]
        public void TearDown()
        {
        }

        private TennisIngestWorkerService CreateTennisIngestService()
        {
            return new TennisIngestWorkerService(
                _mockStatsTennisIngestService,
                _mockPublicSportDataUnitOfWork,
                _mockSystemSportDataUnitOfWork,
                _tennisStorageService,
                _mockLoggingService.Object,
                _mockMongoDbTennisRepository);
        }

        [Test]
        public async Task ReferenceData_Leagues_Count()
        {
            var tennisIngestService = CreateTennisIngestService();

            await tennisIngestService.IngestReferenceData(CancellationToken.None);

            var numberOfLeaguesInDatabase = await _mockPublicSportDataUnitOfWork.TennisLeagues.CountAsync();
            var numberOfLeaguesFromMockedService = _mockStatsTennisIngestService.GetLeagues().apiResults.First().leagues.Count;
            Assert.AreEqual(numberOfLeaguesFromMockedService, numberOfLeaguesInDatabase);
        }

        [Test]
        public async Task ReferenceData_Tournaments_Count()
        {
            var tennisIngestService = CreateTennisIngestService();

            await tennisIngestService.IngestReferenceData(CancellationToken.None);

            var numberOfTournamentsInDatabase = await _mockPublicSportDataUnitOfWork.TennisTournaments.CountAsync();
            var numberOfTournamentsInMockedService = _mockStatsTennisIngestService.GetTournamentsForLeague("").apiResults.First().league.tournaments.Count;
            Assert.AreEqual(numberOfTournamentsInMockedService, numberOfTournamentsInDatabase);
        }

        [Test]
        public async Task ReferenceData_SurfaceTypes_Count()
        {
            var tennisIngestService = CreateTennisIngestService();

            await tennisIngestService.IngestReferenceData(CancellationToken.None);

            // We have to enable the league in order for ingest to run for the league.
            (await _mockPublicSportDataUnitOfWork.TennisLeagues.AllAsync()).ToList().ForEach(
                l => l.IsDisabledInbound = false);

            await tennisIngestService.IngestReferenceData(CancellationToken.None);

            var numberOfSurfaceTypesInDatabase = await _mockPublicSportDataUnitOfWork.TennisSurfaceTypes.CountAsync();
            var numberOfSurfaceTypesInMockedService = _mockStatsTennisIngestService.GetSurfaceTypes("").apiResults?.surfaceTypes.Count;
            Assert.AreEqual(numberOfSurfaceTypesInMockedService, numberOfSurfaceTypesInDatabase);
        }

        [Test]
        public async Task ReferenceData_Participants_Count()
        {
            var tennisIngestService = CreateTennisIngestService();

            await tennisIngestService.IngestReferenceData(CancellationToken.None);

            // We have to enable the league in order for ingest to run for the league.
            (await _mockPublicSportDataUnitOfWork.TennisLeagues.AllAsync()).ToList().ForEach(
                l => l.IsDisabledInbound = false);

            await tennisIngestService.IngestReferenceData(CancellationToken.None);

            var numberOfParticipantsInDatabase = await _mockPublicSportDataUnitOfWork.TennisPlayers.CountAsync();
            var numberOfParticipantsInMockedService = _mockStatsTennisIngestService.GetParticipants("").apiResults?.First().league.subLeague.players.Count;
            Assert.AreEqual(numberOfParticipantsInMockedService, numberOfParticipantsInDatabase);
        }

        [Test]
        public async Task ReferenceData_Seasons_Count()
        {
            var tennisIngestService = CreateTennisIngestService();

            await tennisIngestService.IngestReferenceData(CancellationToken.None);

            // We have to enable the league in order for ingest to run for the league.
            (await _mockPublicSportDataUnitOfWork.TennisLeagues.AllAsync()).ToList().ForEach(
                l => l.IsDisabledInbound = false);

            await tennisIngestService.IngestReferenceData(CancellationToken.None);

            var numberOfSeasonsInDatabase = await _mockPublicSportDataUnitOfWork.TennisSeasons.CountAsync();
            var numberOfSeasonsInMockedService = _mockStatsTennisIngestService.GetSeasonForLeague("").apiResults?.First().league.seasons.Count;
            Assert.AreEqual(numberOfSeasonsInMockedService, numberOfSeasonsInDatabase);
        }
    }
}
