using NUnit.Framework;
using Moq;
using SuperSportDataEngine.ApplicationLogic.Services;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models;
using SuperSportDataEngine.Tests.Common.Repositories.Test;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models.Enums;
using SuperSportDataEngine.ApplicationLogic.Entities.Legacy;
using SuperSportDataEngine.Common.Logging;
using System.Threading;
using System.Linq;

namespace SuperSportDataEngine.ApplicationLogic.Tests
{
    public class RugbyService_Test
    {
        RugbyService RugbyService;
        Mock<TestEntityFrameworkRepository<SchedulerTrackingRugbyFixture>> MockSchedulerTrackingFixtureRepository;
        Mock<TestEntityFrameworkRepository<SchedulerTrackingRugbySeason>> MockSchedulerTrackingSeasonRepository;
        Mock<TestEntityFrameworkRepository<SchedulerTrackingRugbyTournament>> MockSchedulerTrackingTournamentsRepository;
        Mock<TestEntityFrameworkRepository<RugbyFixture>> MockFixtureRepository;
        Mock<TestEntityFrameworkRepository<RugbySeason>> MockSeasonRepository;
        Mock<TestEntityFrameworkRepository<RugbyTournament>> MockTournamentRepository;
        Mock<TestEntityFrameworkRepository<RugbyFlatLog>> MockFlatLogRepository;
        Mock<TestEntityFrameworkRepository<RugbyGroupedLog>> MockGroupedLogRepository;
        Mock<TestEntityFrameworkRepository<RugbyMatchDetailsEntity>> MockMatchDetailsRepository;
        Mock<TestEntityFrameworkRepository<RugbyCommentary>> MockCommentaryRepository;
        Mock<TestEntityFrameworkRepository<RugbyPlayerLineup>> MockLineupRepository;
        Mock<TestEntityFrameworkRepository<RugbyMatchStatistics>> MockMatchStatisticsRepository;
        Mock<TestEntityFrameworkRepository<RugbyMatchEvent>> MockMatchEventsRepository;
        Mock<ILoggingService> MockLogger;

        [SetUp]
        public void SetUp()
        {
            MockSchedulerTrackingFixtureRepository =
                    new Mock<TestEntityFrameworkRepository<SchedulerTrackingRugbyFixture>>(new List<SchedulerTrackingRugbyFixture>());

            MockSchedulerTrackingSeasonRepository =
                    new Mock<TestEntityFrameworkRepository<SchedulerTrackingRugbySeason>>(new List<SchedulerTrackingRugbySeason>());

            MockSchedulerTrackingTournamentsRepository =
                    new Mock<TestEntityFrameworkRepository<SchedulerTrackingRugbyTournament>>(new List<SchedulerTrackingRugbyTournament>());

            MockFixtureRepository =
                    new Mock<TestEntityFrameworkRepository<RugbyFixture>>(new List<RugbyFixture>());

            MockTournamentRepository =
                    new Mock<TestEntityFrameworkRepository<RugbyTournament>>(new List<RugbyTournament>());

            MockSeasonRepository =
                    new Mock<TestEntityFrameworkRepository<RugbySeason>>(new List<RugbySeason>());

            MockFlatLogRepository =
                    new Mock<TestEntityFrameworkRepository<RugbyFlatLog>>(new List<RugbyFlatLog>());

            MockGroupedLogRepository =
                    new Mock<TestEntityFrameworkRepository<RugbyGroupedLog>>(new List<RugbyGroupedLog>());

            MockMatchDetailsRepository =
                    new Mock<TestEntityFrameworkRepository<RugbyMatchDetailsEntity>>(new List<RugbyMatchDetailsEntity>());

            MockCommentaryRepository =
                     new Mock<TestEntityFrameworkRepository<RugbyCommentary>>(new List<RugbyCommentary>());

            MockLineupRepository =
                    new Mock<TestEntityFrameworkRepository<RugbyPlayerLineup>>(new List<RugbyPlayerLineup>());

            MockMatchStatisticsRepository =
                    new Mock<TestEntityFrameworkRepository<RugbyMatchStatistics>>(new List<RugbyMatchStatistics>());

            MockMatchEventsRepository =
                    new Mock<TestEntityFrameworkRepository<RugbyMatchEvent>>(new List<RugbyMatchEvent>());

            MockLogger =
                    new Mock<ILoggingService>();

            RugbyService = new RugbyService(
                MockMatchEventsRepository.Object,
                MockMatchStatisticsRepository.Object,
                MockLineupRepository.Object,
                MockCommentaryRepository.Object,
                MockGroupedLogRepository.Object,
                MockFlatLogRepository.Object,
                MockTournamentRepository.Object,
                MockSeasonRepository.Object,
                MockSchedulerTrackingSeasonRepository.Object,
                MockFixtureRepository.Object,
                MockSchedulerTrackingTournamentsRepository.Object,
                MockSchedulerTrackingFixtureRepository.Object);
        }

        [Test]
        public async Task CleanupFixturesTable_OneFixtureTrackedToday_NotDeleted()
        {
            MockSchedulerTrackingFixtureRepository.Object.Add(new SchedulerTrackingRugbyFixture() {
                    FixtureId = Guid.NewGuid(),
                    StartDateTime = DateTimeOffset.UtcNow,
                    RugbyFixtureStatus = RugbyFixtureStatus.PreMatch
            });
            
            await RugbyService.CleanupSchedulerTrackingRugbyFixturesTable();

            Assert.AreEqual(1, await MockSchedulerTrackingFixtureRepository.Object.CountAsync());
        }

        [Test]
        public async Task CleanupFixturesTable_OneFixtureTracked6MonthsAgo_Deleted()
        {
            MockSchedulerTrackingFixtureRepository.Object.Add(new SchedulerTrackingRugbyFixture()
            {
                FixtureId = Guid.NewGuid(),
                StartDateTime = DateTimeOffset.UtcNow - TimeSpan.FromDays(181),
                RugbyFixtureStatus = RugbyFixtureStatus.Result
            });

            await MockSchedulerTrackingFixtureRepository.Object.SaveAsync();

            await RugbyService.CleanupSchedulerTrackingRugbyFixturesTable();

            Assert.AreEqual(0, await MockSchedulerTrackingFixtureRepository.Object.CountAsync());
        }

        [Test]
        public async Task CleanupFixturesTable_OneFixtureTrackedLessThan6MonthsAgo_NotDeleted()
        {
            MockSchedulerTrackingFixtureRepository.Object.Add(new SchedulerTrackingRugbyFixture()
            {
                FixtureId = Guid.NewGuid(),
                StartDateTime = DateTimeOffset.UtcNow - TimeSpan.FromDays(179),
                RugbyFixtureStatus = RugbyFixtureStatus.Result
            });

            await MockSchedulerTrackingFixtureRepository.Object.SaveAsync();

            await RugbyService.CleanupSchedulerTrackingRugbyFixturesTable();

            Assert.AreEqual(1, await MockSchedulerTrackingFixtureRepository.Object.CountAsync());
        }

        [Test]
        public async Task CleanupSeasonsTable_SeasonIsCurrent_NotDeleted()
        {
            MockSchedulerTrackingSeasonRepository.Object.Add(new SchedulerTrackingRugbySeason()
            {
                SeasonId = Guid.NewGuid(),
                RugbySeasonStatus = RugbySeasonStatus.InProgress
            });

            await RugbyService.CleanupSchedulerTrackingRugbySeasonsTable();

            Assert.AreEqual(1, await MockSchedulerTrackingSeasonRepository.Object.CountAsync());
        }

        [Test]
        public async Task CleanupSeasonsTable_SeasonIsEnded_IsDeleted()
        {
            MockSchedulerTrackingSeasonRepository.Object.Add(new SchedulerTrackingRugbySeason()
            {
                SeasonId = Guid.NewGuid(),
                RugbySeasonStatus = RugbySeasonStatus.Ended
            });

            await MockSchedulerTrackingSeasonRepository.Object.SaveAsync();

            await RugbyService.CleanupSchedulerTrackingRugbySeasonsTable();

            Assert.AreEqual(0, await MockSchedulerTrackingSeasonRepository.Object.CountAsync());
        }

        [Test]
        public async Task CleanupTournamentsTable_TournamentIsEnabled_NotDeleted()
        {
            var tournamentId = Guid.NewGuid();

            MockTournamentRepository.Object.Add(new RugbyTournament()
            {
                Id = tournamentId,
                IsEnabled = true
            });

            await MockTournamentRepository.Object.SaveAsync();

            MockSchedulerTrackingTournamentsRepository.Object.Add(new SchedulerTrackingRugbyTournament()
            {
                TournamentId = tournamentId
            });

            await MockSchedulerTrackingSeasonRepository.Object.SaveAsync();

            await RugbyService.CleanupSchedulerTrackingRugbySeasonsTable();

            Assert.AreEqual(1, await MockSchedulerTrackingTournamentsRepository.Object.CountAsync());
        }

        [Test]
        public async Task CleanupTournamentsTable_TournamentIsDisabled_IsDeleted()
        {
            var tournamentId = Guid.NewGuid();

            MockTournamentRepository.Object.Add(new RugbyTournament()
            {
                Id = tournamentId,
                IsEnabled = false
            });

            await MockTournamentRepository.Object.SaveAsync();

            MockSchedulerTrackingTournamentsRepository.Object.Add(new SchedulerTrackingRugbyTournament()
            {
                TournamentId = tournamentId
            });

            await MockSchedulerTrackingSeasonRepository.Object.SaveAsync();

            await RugbyService.CleanupSchedulerTrackingRugbyTournamentsTable();

            Assert.AreEqual(0, await MockSchedulerTrackingTournamentsRepository.Object.CountAsync());
        }

        [Test]
        public async Task LiveGame_LiveGameCount()
        {
            Guid fixtureGuid = Guid.NewGuid();
            DateTime fixtureStartDate = DateTime.Now - TimeSpan.FromMinutes(1);

            MockFixtureRepository.Object.Add(
                new RugbyFixture()
                {
                    Id = fixtureGuid,
                    RugbyFixtureStatus = RugbyFixtureStatus.PreMatch,
                    StartDateTime = fixtureStartDate,
                });

            MockSchedulerTrackingFixtureRepository.Object.Add(
                new SchedulerTrackingRugbyFixture()
                {
                    FixtureId = fixtureGuid,
                    RugbyFixtureStatus = RugbyFixtureStatus.PreMatch,
                    SchedulerStateFixtures = SchedulerStateForRugbyFixturePolling.PreLivePolling
                });

            Assert.AreEqual(1, await RugbyService.GetLiveFixturesCount(CancellationToken.None));
        }

        [Test]
        public async Task LiveGame_LiveGames_Getter_Count()
        {
            Guid tournamentGuid = Guid.NewGuid();

            var TestTournament = new RugbyTournament()
            {
                Id = tournamentGuid,
                Name = "testTournament",
                IsEnabled = true
            };

            MockTournamentRepository.Object.Add(
                TestTournament);

            Guid fixtureGuid = Guid.NewGuid();
            DateTime fixtureStartDate = DateTime.Now - TimeSpan.FromMinutes(1);

            MockFixtureRepository.Object.Add(
                new RugbyFixture()
                {
                    Id = fixtureGuid,
                    RugbyTournament = TestTournament,
                    RugbyFixtureStatus = RugbyFixtureStatus.PreMatch,
                    StartDateTime = fixtureStartDate,
                });

            MockSchedulerTrackingFixtureRepository.Object.Add(
                new SchedulerTrackingRugbyFixture()
                {
                    FixtureId = fixtureGuid,
                    TournamentId = tournamentGuid,
                    RugbyFixtureStatus = RugbyFixtureStatus.PreMatch,
                    SchedulerStateFixtures = SchedulerStateForRugbyFixturePolling.PreLivePolling
                });

            var liveGames = await RugbyService.GetLiveFixturesForCurrentTournament(CancellationToken.None, tournamentGuid);

            Assert.AreEqual(1, liveGames.Count());
        }

        [Test]
        public async Task LiveGame_Status()
        {
            Guid tournamentGuid = Guid.NewGuid();

            var TestTournament = new RugbyTournament()
            {
                Id = tournamentGuid,
                Name = "testTournament",
                IsEnabled = true
            };

            MockTournamentRepository.Object.Add(
                TestTournament);

            Guid fixtureGuid = Guid.NewGuid();
            DateTime fixtureStartDate = DateTime.Now - TimeSpan.FromMinutes(1);

            MockFixtureRepository.Object.Add(
                new RugbyFixture()
                {
                    Id = fixtureGuid,
                    RugbyTournament = TestTournament,
                    RugbyFixtureStatus = RugbyFixtureStatus.PreMatch,
                    StartDateTime = fixtureStartDate,
                });

            MockSchedulerTrackingFixtureRepository.Object.Add(
                new SchedulerTrackingRugbyFixture()
                {
                    FixtureId = fixtureGuid,
                    TournamentId = tournamentGuid,
                    RugbyFixtureStatus = RugbyFixtureStatus.PreMatch,
                    SchedulerStateFixtures = SchedulerStateForRugbyFixturePolling.PreLivePolling
                });

            var liveGames = await RugbyService.GetLiveFixturesForCurrentTournament(CancellationToken.None, tournamentGuid);

            Assert.AreEqual(RugbyFixtureStatus.PreMatch, liveGames.First().RugbyFixtureStatus);
        }

        [Test]
        public async Task LiveGame_Getter_Count_0_NotLive()
        {
            Guid tournamentGuid = Guid.NewGuid();

            var TestTournament = new RugbyTournament()
            {
                Id = tournamentGuid,
                Name = "testTournament",
                IsEnabled = true
            };

            MockTournamentRepository.Object.Add(
                TestTournament);

            Guid fixtureGuid = Guid.NewGuid();
            DateTime fixtureStartDate = DateTime.Now + TimeSpan.FromMinutes(20);

            MockFixtureRepository.Object.Add(
                new RugbyFixture()
                {
                    Id = fixtureGuid,
                    RugbyTournament = TestTournament,
                    RugbyFixtureStatus = RugbyFixtureStatus.PreMatch,
                    StartDateTime = fixtureStartDate,
                });

            MockSchedulerTrackingFixtureRepository.Object.Add(
                new SchedulerTrackingRugbyFixture()
                {
                    FixtureId = fixtureGuid,
                    TournamentId = tournamentGuid,
                    RugbyFixtureStatus = RugbyFixtureStatus.PreMatch,
                    SchedulerStateFixtures = SchedulerStateForRugbyFixturePolling.SchedulingNotYetStarted
                });

            var liveGames = await RugbyService.GetLiveFixturesForCurrentTournament(CancellationToken.None, tournamentGuid);

            Assert.AreEqual(0, liveGames.Count());
        }

        [Test]
        public async Task LiveGame_Getter_Count_1_Live()
        {
            Guid tournamentGuid = Guid.NewGuid();

            var TestTournament = new RugbyTournament()
            {
                Id = tournamentGuid,
                Name = "testTournament",
                IsEnabled = true
            };

            MockTournamentRepository.Object.Add(
                TestTournament);

            Guid fixtureGuid = Guid.NewGuid();
            DateTime fixtureStartDate = DateTime.Now + TimeSpan.FromMinutes(14);

            MockFixtureRepository.Object.Add(
                new RugbyFixture()
                {
                    Id = fixtureGuid,
                    RugbyTournament = TestTournament,
                    RugbyFixtureStatus = RugbyFixtureStatus.PreMatch,
                    StartDateTime = fixtureStartDate,
                });

            MockSchedulerTrackingFixtureRepository.Object.Add(
                new SchedulerTrackingRugbyFixture()
                {
                    FixtureId = fixtureGuid,
                    TournamentId = tournamentGuid,
                    RugbyFixtureStatus = RugbyFixtureStatus.PreMatch,
                    SchedulerStateFixtures = SchedulerStateForRugbyFixturePolling.PreLivePolling
                });

            var liveGames = await RugbyService.GetLiveFixturesForCurrentTournament(CancellationToken.None, tournamentGuid);

            Assert.AreEqual(1, liveGames.Count());
        }

        [Test]
        public async Task CompletedFixture_Getter_Count_0_NotLive()
        {
            Guid tournamentGuid = Guid.NewGuid();

            var TestTournament = new RugbyTournament()
            {
                Id = tournamentGuid,
                Name = "testTournament",
                IsEnabled = true
            };

            MockTournamentRepository.Object.Add(
                TestTournament);

            Guid fixtureGuid = Guid.NewGuid();
            DateTime fixtureStartDate = DateTime.Now + TimeSpan.FromMinutes(20);

            MockFixtureRepository.Object.Add(
                new RugbyFixture()
                {
                    Id = fixtureGuid,
                    RugbyTournament = TestTournament,
                    RugbyFixtureStatus = RugbyFixtureStatus.FirstHalf,
                    StartDateTime = fixtureStartDate,
                });

            MockSchedulerTrackingFixtureRepository.Object.Add(
                new SchedulerTrackingRugbyFixture()
                {
                    FixtureId = fixtureGuid,
                    TournamentId = tournamentGuid,
                    RugbyFixtureStatus = RugbyFixtureStatus.PreMatch,
                    SchedulerStateFixtures = SchedulerStateForRugbyFixturePolling.LivePolling
                });

            var completedFixtures = await RugbyService.GetCompletedFixtures();

            Assert.AreEqual(0, completedFixtures.Count());
        }

        [Test]
        public async Task CompletedFixture_Getter_Count_1_NotLive()
        {
            Guid tournamentGuid = Guid.NewGuid();

            var TestTournament = new RugbyTournament()
            {
                Id = tournamentGuid,
                Name = "testTournament",
                IsEnabled = true
            };

            MockTournamentRepository.Object.Add(
                TestTournament);

            Guid fixtureGuid = Guid.NewGuid();
            DateTime fixtureStartDate = DateTime.Now + TimeSpan.FromMinutes(20);

            MockFixtureRepository.Object.Add(
                new RugbyFixture()
                {
                    Id = fixtureGuid,
                    RugbyTournament = TestTournament,
                    RugbyFixtureStatus = RugbyFixtureStatus.Result,
                    StartDateTime = fixtureStartDate,
                });

            MockSchedulerTrackingFixtureRepository.Object.Add(
                new SchedulerTrackingRugbyFixture()
                {
                    FixtureId = fixtureGuid,
                    TournamentId = tournamentGuid,
                    RugbyFixtureStatus = RugbyFixtureStatus.PreMatch,
                    SchedulerStateFixtures = SchedulerStateForRugbyFixturePolling.SchedulingCompleted
                });

            var completedFixtures = await RugbyService.GetCompletedFixtures();

            Assert.AreEqual(1, completedFixtures.Count());
        }

        [Test]
        public async Task PostponedFixture_Getter_Count_0()
        {
            Guid tournamentGuid = Guid.NewGuid();

            var TestTournament = new RugbyTournament()
            {
                Id = tournamentGuid,
                Name = "testTournament",
                IsEnabled = true
            };

            MockTournamentRepository.Object.Add(
                TestTournament);

            Guid fixtureGuid = Guid.NewGuid();
            DateTime fixtureStartDate = DateTime.Now - TimeSpan.FromHours(2);

            MockFixtureRepository.Object.Add(
                new RugbyFixture()
                {
                    Id = fixtureGuid,
                    RugbyTournament = TestTournament,
                    RugbyFixtureStatus = RugbyFixtureStatus.PreMatch,
                    StartDateTime = fixtureStartDate,
                });

            MockSchedulerTrackingFixtureRepository.Object.Add(
                new SchedulerTrackingRugbyFixture()
                {
                    FixtureId = fixtureGuid,
                    TournamentId = tournamentGuid,
                    RugbyFixtureStatus = RugbyFixtureStatus.PreMatch,
                    SchedulerStateFixtures = SchedulerStateForRugbyFixturePolling.SchedulingCompleted
                });

            var postponedFixture = await RugbyService.GetPostponedFixtures();

            Assert.AreEqual(0, postponedFixture.Count());
        }

        [Test]
        public async Task PostponedFixture_Getter_Count_1()
        {
            Guid tournamentGuid = Guid.NewGuid();

            var TestTournament = new RugbyTournament()
            {
                Id = tournamentGuid,
                Name = "testTournament",
                IsEnabled = true
            };

            MockTournamentRepository.Object.Add(
                TestTournament);

            Guid fixtureGuid = Guid.NewGuid();
            DateTime fixtureStartDate = DateTime.Now - TimeSpan.FromHours(4);

            MockFixtureRepository.Object.Add(
                new RugbyFixture()
                {
                    Id = fixtureGuid,
                    RugbyTournament = TestTournament,
                    RugbyFixtureStatus = RugbyFixtureStatus.PreMatch,
                    StartDateTime = fixtureStartDate,
                });

            MockSchedulerTrackingFixtureRepository.Object.Add(
                new SchedulerTrackingRugbyFixture()
                {
                    FixtureId = fixtureGuid,
                    TournamentId = tournamentGuid,
                    RugbyFixtureStatus = RugbyFixtureStatus.PreMatch,
                    SchedulerStateFixtures = SchedulerStateForRugbyFixturePolling.SchedulingCompleted
                });

            var postponedFixture = await RugbyService.GetPostponedFixtures();

            Assert.AreEqual(1, postponedFixture.Count());
        }
    }
}
