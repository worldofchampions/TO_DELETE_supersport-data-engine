using System;
using System.Collections.Generic;
using System.Linq;
using Hangfire;
using Moq;
using NUnit.Framework;
using SuperSportDataEngine.Application.Service.SchedulerClient.ScheduledManager.Tennis;
using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.UnitOfWork;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.UnitOfWork;
using System.Threading.Tasks;
using Hangfire.Common;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models.Enums;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models.Tennis;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models.Enums;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models.Tennis;
using SuperSportDataEngine.Tests.Common.Repositories.Test;

namespace SuperSportDataEngine.Application.Service.SchedulerClient.Tests.ScheduledManager.Tennis
{
    [TestFixture]
    public class TennisLiveManagerJobTests
    {
        private IPublicSportDataUnitOfWork _testPublicSportDataUnitOfWork;
        private ISystemSportDataUnitOfWork _testSystemSportDataUnitOfWork;
        private Mock<IRecurringJobManager> _mockRecurringJobManager;
        private Mock<ITennisIngestWorkerService> _mockTennisIngestWorkerService;

        [SetUp]
        public void SetUp()
        {
            _testPublicSportDataUnitOfWork = new TestPublicSportDataUnitOfWork();
            _testSystemSportDataUnitOfWork = new TestSystemSportDataUnitOfWork();
            _mockRecurringJobManager = new Mock<IRecurringJobManager>();

            _mockRecurringJobManager.Setup(c =>
                c.AddOrUpdate(It.IsAny<string>(), It.IsAny<Job>(), It.IsAny<string>(), It.IsAny<RecurringJobOptions>()));

            _mockRecurringJobManager.Setup(c =>
                c.RemoveIfExists(It.IsAny<string>()));

            _mockTennisIngestWorkerService = new Mock<ITennisIngestWorkerService>();
        }

        [TearDown]
        public void TearDown()
        {
        }

        private TennisLiveManagerJob CreateTennisLiveManagerJob()
        {
            return new TennisLiveManagerJob(
                _testPublicSportDataUnitOfWork,
                _testSystemSportDataUnitOfWork,
                _mockRecurringJobManager.Object,
                _mockTennisIngestWorkerService.Object);
        }

        [Test]
        public async Task DoWorkAsync_ThrowsNoExceptions()
        {
            // Arrange
            var unitUnderTest = CreateTennisLiveManagerJob();

            // Act
            try
            {
                await unitUnderTest.DoWorkAsync();
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }

        [Test]
        public async Task CreateLiveJob_MatchUpcoming()
        {
            // Arrange
            var unitUnderTest = CreateTennisLiveManagerJob();
            SetUp_MatchUpcoming();

            // Act
            await unitUnderTest.DoWorkAsync();

            _mockRecurringJobManager
                .Verify(
                    m => m.AddOrUpdate(
                        "Tennis→Stats→ChildJob→LiveMatch→Test Event→" + 0,
                        It.IsAny<Job>(),
                        "0 0 29 2/12000 WED",
                        It.IsAny<RecurringJobOptions>()),
                    Times.Once);
        }

        [Test]
        public async Task DeleteLiveJob_MatchCompleted()
        {
            // Arrange
            var unitUnderTest = CreateTennisLiveManagerJob();
            SetUp_MatchCompleted();

            // Act
            await unitUnderTest.DoWorkAsync();

            _mockRecurringJobManager
                .Verify(
                    m => m.RemoveIfExists(
                        "Tennis→Stats→ChildJob→LiveMatch→Test Event→" + 0),
                    Times.Once);
        }

        [Test]
        public async Task WhenJobCreate_SchedulerStateUpdated()
        {
            // Arrange
            var unitUnderTest = CreateTennisLiveManagerJob();
            SetUp_MatchUpcoming();

            // Act
            await unitUnderTest.DoWorkAsync();

            var trackingState = (await _testSystemSportDataUnitOfWork.SchedulerTrackingTennisMatches.AllAsync()).FirstOrDefault()?.SchedulerStateForTennisMatchPolling;
            Assert.AreEqual(SchedulerStateForTennisMatchPolling.LivePolling, trackingState);
        }

        [Test]
        public async Task DontCreateLiveJob_MatchFuture()
        {
            // Arrange
            var unitUnderTest = CreateTennisLiveManagerJob();
            SetUp_MatchFuture();

            // Act
            await unitUnderTest.DoWorkAsync();

            _mockRecurringJobManager
                .Verify(
                    m => m.AddOrUpdate(
                        "Tennis→Stats→ChildJob→LiveMatch→Test Event→" + 0,
                        It.IsAny<Job>(),
                        "0 0 29 2/12000 WED",
                        It.IsAny<RecurringJobOptions>()),
                    Times.Never);
        }

        private TennisLeague AddNewTennisLeagueInRepository()
        {
            var leagueId = Guid.NewGuid();
            var tennisLeague = new TennisLeague
            {
                Slug = "test-slug",
                ProviderSlug = "test-slug",
                LegacyLeagueId = 0,
                ProviderLeagueId = 0,
                Abbreviation = "test",
                DataProvider = DataProvider.Stats,
                Gender = TennisGender.Male,
                Id = leagueId
            };

            _testPublicSportDataUnitOfWork.TennisLeagues.Add(tennisLeague);
            return tennisLeague;
        }

        private TennisTournament AddNewTennisTournamentInRepository(TennisLeague tennisLeague)
        {
            var tournamentId = Guid.NewGuid();
            var tennisTournament = new TennisTournament
            {
                Slug = "test-tournament",
                ProviderDisplayName = "Test",
                Abbreviation = "test",
                DataProvider = DataProvider.Stats,
                Id = tournamentId,
                IsDisabledInbound = false,
                IsDisabledOutbound = false,
                LegacyTournamentId = 0,
                NameCmsOverride = null,
                ProviderTournamentId = 0,
                TennisLeagues = new List<TennisLeague> { tennisLeague }
            };

            _testPublicSportDataUnitOfWork.TennisTournaments.Add(tennisTournament);
            return tennisTournament;
        }

        private TennisSeason AddNewTennisSeasonInRepository(TennisLeague tennisLeague, DateTimeOffset start, DateTimeOffset end)
        {
            var seasonId = Guid.NewGuid();
            var tennisSeason = new TennisSeason
            {
                Id = seasonId,
                DataProvider = DataProvider.Stats,
                EndDateUtc = end,
                StartDateUtc = start,
                IsActive = true,
                IsCurrent = true,
                Name = "Test Season",
                ProviderSeasonId = 2018,
                TennisLeague = tennisLeague
            };

            _testPublicSportDataUnitOfWork.TennisSeasons.Add(tennisSeason);
            return tennisSeason;
        }

        private TennisEvent AddNewTennisEventInRepository(TennisSeason tennisSeason, TennisTournament tennisTournament, DateTimeOffset start, DateTimeOffset end)
        {
            var eventId = Guid.NewGuid();
            var tennisEvent = new TennisEvent
            {
                Id = eventId,
                DataProvider = DataProvider.Stats,
                EndDateUtc = end,
                EventName = "Test Event",
                EventNameCmsOverride = null,
                LegacyEventId = 0,
                ProviderEventId = 0,
                StartDateUtc = start,
                TennisSeason = tennisSeason,
                TennisTournament = tennisTournament
            };

            _testPublicSportDataUnitOfWork.TennisEvents.Add(tennisEvent);
            return tennisEvent;
        }

        private TennisMatch SetUp_MatchUpcoming()
        {
            return AddNewMatchInRepository(DateTimeOffset.UtcNow + TimeSpan.FromMinutes(10), SchedulerStateForTennisMatchPolling.NotStarted);
        }

        private TennisMatch SetUp_MatchFuture()
        {
            return AddNewMatchInRepository(DateTimeOffset.UtcNow + TimeSpan.FromMinutes(60), SchedulerStateForTennisMatchPolling.NotStarted);
        }

        private TennisMatch SetUp_MatchCompleted()
        {
            return AddNewMatchInRepository(DateTimeOffset.UtcNow - TimeSpan.FromMinutes(60), SchedulerStateForTennisMatchPolling.PollingComplete);
        }

        private TennisMatch AddNewMatchInRepository(DateTimeOffset start, SchedulerStateForTennisMatchPolling pollingState)
        {
            var tennisLeague = AddNewTennisLeagueInRepository();
            var tennisTournament = AddNewTennisTournamentInRepository(tennisLeague);
            var tennisSeason = AddNewTennisSeasonInRepository(tennisLeague,
                DateTimeOffset.UtcNow - TimeSpan.FromHours(12), DateTimeOffset.UtcNow + TimeSpan.FromHours(24));

            var tennisEvent = AddNewTennisEventInRepository(tennisSeason, tennisTournament,
                DateTimeOffset.UtcNow - TimeSpan.FromHours(12), DateTimeOffset.UtcNow + TimeSpan.FromHours(24));

            var association = new TennisEventTennisLeagues
            {
                TennisEventId = tennisEvent.Id,
                TennisEvent = tennisEvent,
                TennisLeague = tennisLeague,
                TennisLeagueId = tennisLeague.Id
            };

            var match = new TennisMatch
            {
                AssociatedTennisEventTennisLeague = association,
                ProviderMatchId = 0,
                LegacyMatchId = 0,
                CourtName = "Test",
                DataProvider = DataProvider.Stats,
                DrawNumber = 0,
                Id = Guid.NewGuid(),
                NumberOfSets = 0,
                RoundNumber = 0,
                StartDateTimeUtc = start
            };

            _testPublicSportDataUnitOfWork.TennisMatches.Add(match);

            var trackingEvent = new SchedulerTrackingTennisMatch
            {
                TennisMatchId = match.Id,
                SchedulerStateForTennisMatchPolling = pollingState,
                StartDateTime = start
            };

            _testSystemSportDataUnitOfWork.SchedulerTrackingTennisMatches.Add(trackingEvent);

            return match;
        }
    }
}
