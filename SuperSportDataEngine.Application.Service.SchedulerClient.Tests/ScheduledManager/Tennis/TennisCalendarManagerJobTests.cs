using System;
using System.Collections.Generic;
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
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models.Tennis;
using SuperSportDataEngine.Tests.Common.Repositories.Test;

namespace SuperSportDataEngine.Application.Service.SchedulerClient.Tests.ScheduledManager.Tennis
{
    [TestFixture]
    public class TennisCalendarManagerJobTests
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

        private TennisCalendarManagerJob CreateTennisCalendarManagerJob()
        {
            return new TennisCalendarManagerJob(
                _testPublicSportDataUnitOfWork,
                _testSystemSportDataUnitOfWork,
                _mockRecurringJobManager.Object,
                _mockTennisIngestWorkerService.Object);
        }

        [Test]
        public async Task DoWorkAsync_ThrowsNoExceptions()
        {
            // Arrange
            var unitUnderTest = CreateTennisCalendarManagerJob();

            // Act and Assert
            try
            {
                await unitUnderTest.DoWorkAsync();
            }
            catch (Exception)
            {
                Assert.Fail();
            }

            Assert.Pass();
        }

        [Test]
        public async Task CreateCalendarJob_EventCurrent()
        {
            // Arrange
            var unitUnderTest = CreateTennisCalendarManagerJob();
            Setup_CurrentEventInRepository();

            // Act and Assert.
            await unitUnderTest.DoWorkAsync();

            _mockRecurringJobManager
                .Verify(
                    m => m.AddOrUpdate(
                        "Tennis→Stats→ChildJob→Calendar→" + "test",
                        It.IsAny<Job>(),
                        "*/15 * * * *",
                        It.IsAny<RecurringJobOptions>()),
                    Times.Exactly(1));
        }

        [Test]
        public async Task DontCreateCalendarJob_EventFuture()
        {
            // Arrange
            var unitUnderTest = CreateTennisCalendarManagerJob();
            Setup_FutureEventInRepository();

            // Act and Assert.
            await unitUnderTest.DoWorkAsync();

            _mockRecurringJobManager
                .Verify(
                    m => m.AddOrUpdate(
                        "Tennis→Stats→ChildJob→Calendar→" + "test",
                        It.IsAny<Job>(),
                        "*/15 * * * *",
                        It.IsAny<RecurringJobOptions>()),
                    Times.Never);
        }

        [Test]
        public async Task DontCreateCalendarJob_EventEnded()
        {
            // Arrange
            var unitUnderTest = CreateTennisCalendarManagerJob();
            Setup_EndedEventInRepository();

            // Act and Assert.
            await unitUnderTest.DoWorkAsync();

            _mockRecurringJobManager
                .Verify(
                    m => m.AddOrUpdate(
                        "Tennis→Stats→ChildJob→Calendar→" + "test",
                        It.IsAny<Job>(),
                        "*/15 * * * *",
                        It.IsAny<RecurringJobOptions>()),
                    Times.Never);
        }

        [Test]
        public async Task DeleteCalendarJobs_EventEnded()
        {
            // Arrange
            var unitUnderTest = CreateTennisCalendarManagerJob();
            Setup_EndedEventInRepository();

            // Act and Assert.
            await unitUnderTest.DoWorkAsync();

            _mockRecurringJobManager
                .Verify(
                    m => m.RemoveIfExists(
                        "Tennis→Stats→ChildJob→Calendar→" + "test"),
                    Times.Once);
        }

        [Test]
        public async Task DeleteCalendarJobs_EventFuture()
        {
            // Arrange
            var unitUnderTest = CreateTennisCalendarManagerJob();
            Setup_FutureEventInRepository();

            // Act and Assert.
            await unitUnderTest.DoWorkAsync();

            _mockRecurringJobManager
                .Verify(
                    m => m.RemoveIfExists(
                        "Tennis→Stats→ChildJob→Calendar→" + "test"),
                    Times.Exactly(1));
        }

        [Test]
        public async Task DontDeleteCalendarJobs_EventCurrent()
        {
            // Arrange
            var unitUnderTest = CreateTennisCalendarManagerJob();
            Setup_CurrentEventInRepository();

            // Act and Assert.
            await unitUnderTest.DoWorkAsync();

            _mockRecurringJobManager
                .Verify(
                    m => m.RemoveIfExists(
                        "Tennis→Stats→ChildJob→Calendar→" + "test"),
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

        private void AddTennisEventTrackingEntryInRepository(TennisEvent tennisEvent, DateTimeOffset start, DateTimeOffset end)
        {
            var trackingEvent = new SchedulerTrackingTennisEvent
            {
                TennisEventId = tennisEvent.Id,
                StartDateTime = start,
                EndDateTime = end
            };

            _testSystemSportDataUnitOfWork.SchedulerTrackingTennisEvents.Add(trackingEvent);
        }

        private void Setup_CurrentEventInRepository()
        {
            var startDate = DateTimeOffset.UtcNow - TimeSpan.FromHours(6);
            var endDate = DateTimeOffset.UtcNow + TimeSpan.FromHours(24);

            var tennisLeague = AddNewTennisLeagueInRepository();
            var tennisTournament = AddNewTennisTournamentInRepository(tennisLeague);
            var tennisSeason = AddNewTennisSeasonInRepository(tennisLeague, startDate, endDate);
            var tennisEvent = AddNewTennisEventInRepository(tennisSeason, tennisTournament, startDate, endDate);

            AddTennisEventTrackingEntryInRepository(tennisEvent, startDate, endDate);

            _testPublicSportDataUnitOfWork.SaveChanges();
            _testSystemSportDataUnitOfWork.SaveChanges();
        }

        private void Setup_EndedEventInRepository()
        {
            var startDate = DateTimeOffset.UtcNow - TimeSpan.FromHours(24);
            var endDate = DateTimeOffset.UtcNow - TimeSpan.FromHours(12);

            var tennisLeague = AddNewTennisLeagueInRepository();
            var tennisTournament = AddNewTennisTournamentInRepository(tennisLeague);
            var tennisSeason = AddNewTennisSeasonInRepository(tennisLeague, startDate, endDate);
            var tennisEvent = AddNewTennisEventInRepository(tennisSeason, tennisTournament, startDate, endDate);

            AddTennisEventTrackingEntryInRepository(tennisEvent, startDate, endDate);

            _testPublicSportDataUnitOfWork.SaveChanges();
            _testSystemSportDataUnitOfWork.SaveChanges();
        }

        private void Setup_FutureEventInRepository()
        {
            var startDate = DateTimeOffset.UtcNow + TimeSpan.FromHours(12);
            var endDate = DateTimeOffset.UtcNow + TimeSpan.FromHours(24);

            var tennisLeague = AddNewTennisLeagueInRepository();
            var tennisTournament = AddNewTennisTournamentInRepository(tennisLeague);
            var tennisSeason = AddNewTennisSeasonInRepository(tennisLeague, startDate, endDate);
            var tennisEvent = AddNewTennisEventInRepository(tennisSeason, tennisTournament, startDate, endDate);

            AddTennisEventTrackingEntryInRepository(tennisEvent, startDate, endDate);

            _testPublicSportDataUnitOfWork.SaveChanges();
            _testSystemSportDataUnitOfWork.SaveChanges();
        }
    }
}
