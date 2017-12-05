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
    public class RugbyServiceTest
    {
        RugbyService _rugbyService;
        Mock<TestEntityFrameworkRepository<SchedulerTrackingRugbyFixture>> _mockSchedulerTrackingFixtureRepository;
        Mock<TestEntityFrameworkRepository<SchedulerTrackingRugbySeason>> _mockSchedulerTrackingSeasonRepository;
        Mock<TestEntityFrameworkRepository<SchedulerTrackingRugbyTournament>> _mockSchedulerTrackingTournamentsRepository;
        Mock<TestEntityFrameworkRepository<RugbyFixture>> _mockFixtureRepository;
        Mock<TestEntityFrameworkRepository<RugbySeason>> _mockSeasonRepository;
        Mock<TestEntityFrameworkRepository<RugbyTournament>> _mockTournamentRepository;
        Mock<TestEntityFrameworkRepository<RugbyFlatLog>> _mockFlatLogRepository;
        Mock<TestEntityFrameworkRepository<RugbyGroupedLog>> _mockGroupedLogRepository;
        Mock<TestEntityFrameworkRepository<RugbyCommentary>> _mockCommentaryRepository;
        Mock<TestEntityFrameworkRepository<RugbyPlayerLineup>> _mockLineupRepository;
        Mock<TestEntityFrameworkRepository<RugbyMatchStatistics>> _mockMatchStatisticsRepository;
        Mock<TestEntityFrameworkRepository<RugbyMatchEvent>> _mockMatchEventsRepository;
        Mock<ILoggingService> _mockLogger;

        [SetUp]
        public void SetUp()
        {
            _mockSchedulerTrackingFixtureRepository =
                    new Mock<TestEntityFrameworkRepository<SchedulerTrackingRugbyFixture>>(new List<SchedulerTrackingRugbyFixture>());

            _mockSchedulerTrackingSeasonRepository =
                    new Mock<TestEntityFrameworkRepository<SchedulerTrackingRugbySeason>>(new List<SchedulerTrackingRugbySeason>());

            _mockSchedulerTrackingTournamentsRepository =
                    new Mock<TestEntityFrameworkRepository<SchedulerTrackingRugbyTournament>>(new List<SchedulerTrackingRugbyTournament>());

            _mockFixtureRepository =
                    new Mock<TestEntityFrameworkRepository<RugbyFixture>>(new List<RugbyFixture>());

            _mockTournamentRepository =
                    new Mock<TestEntityFrameworkRepository<RugbyTournament>>(new List<RugbyTournament>());

            _mockSeasonRepository =
                    new Mock<TestEntityFrameworkRepository<RugbySeason>>(new List<RugbySeason>());

            _mockFlatLogRepository =
                    new Mock<TestEntityFrameworkRepository<RugbyFlatLog>>(new List<RugbyFlatLog>());

            _mockGroupedLogRepository =
                    new Mock<TestEntityFrameworkRepository<RugbyGroupedLog>>(new List<RugbyGroupedLog>());

            new Mock<TestEntityFrameworkRepository<RugbyMatchDetailsEntity>>(new List<RugbyMatchDetailsEntity>());

            _mockCommentaryRepository =
                     new Mock<TestEntityFrameworkRepository<RugbyCommentary>>(new List<RugbyCommentary>());

            _mockLineupRepository =
                    new Mock<TestEntityFrameworkRepository<RugbyPlayerLineup>>(new List<RugbyPlayerLineup>());

            _mockMatchStatisticsRepository =
                    new Mock<TestEntityFrameworkRepository<RugbyMatchStatistics>>(new List<RugbyMatchStatistics>());

            _mockMatchEventsRepository =
                    new Mock<TestEntityFrameworkRepository<RugbyMatchEvent>>(new List<RugbyMatchEvent>());

            _mockLogger =
                    new Mock<ILoggingService>();

            _rugbyService = new RugbyService(
                _mockMatchEventsRepository.Object,
                _mockMatchStatisticsRepository.Object,
                _mockLineupRepository.Object,
                _mockCommentaryRepository.Object,
                _mockGroupedLogRepository.Object,
                _mockFlatLogRepository.Object,
                _mockTournamentRepository.Object,
                _mockSeasonRepository.Object,
                _mockSchedulerTrackingSeasonRepository.Object,
                _mockFixtureRepository.Object,
                _mockSchedulerTrackingTournamentsRepository.Object,
                _mockSchedulerTrackingFixtureRepository.Object,
                _mockLogger.Object);
        }

        [Test]
        public async Task CleanupFixturesTable_OneFixtureTrackedToday_NotDeleted()
        {
            _mockSchedulerTrackingFixtureRepository.Object.Add(new SchedulerTrackingRugbyFixture() {
                    FixtureId = Guid.NewGuid(),
                    StartDateTime = DateTimeOffset.UtcNow,
                    RugbyFixtureStatus = RugbyFixtureStatus.PreMatch
            });
            
            await _rugbyService.CleanupSchedulerTrackingRugbyFixturesTable();

            Assert.AreEqual(1, await _mockSchedulerTrackingFixtureRepository.Object.CountAsync());
        }

        [Test]
        public async Task CleanupFixturesTable_OneFixtureTracked6MonthsAgo_Deleted()
        {
            _mockSchedulerTrackingFixtureRepository.Object.Add(new SchedulerTrackingRugbyFixture()
            {
                FixtureId = Guid.NewGuid(),
                StartDateTime = DateTimeOffset.UtcNow - TimeSpan.FromDays(181),
                RugbyFixtureStatus = RugbyFixtureStatus.Result
            });

            await _mockSchedulerTrackingFixtureRepository.Object.SaveAsync();

            await _rugbyService.CleanupSchedulerTrackingRugbyFixturesTable();

            Assert.AreEqual(0, await _mockSchedulerTrackingFixtureRepository.Object.CountAsync());
        }

        [Test]
        public async Task CleanupFixturesTable_OneFixtureTrackedLessThan6MonthsAgo_NotDeleted()
        {
            _mockSchedulerTrackingFixtureRepository.Object.Add(new SchedulerTrackingRugbyFixture()
            {
                FixtureId = Guid.NewGuid(),
                StartDateTime = DateTimeOffset.UtcNow - TimeSpan.FromDays(179),
                RugbyFixtureStatus = RugbyFixtureStatus.Result
            });

            await _mockSchedulerTrackingFixtureRepository.Object.SaveAsync();

            await _rugbyService.CleanupSchedulerTrackingRugbyFixturesTable();

            Assert.AreEqual(1, await _mockSchedulerTrackingFixtureRepository.Object.CountAsync());
        }

        [Test]
        public async Task CleanupSeasonsTable_SeasonIsCurrent_NotDeleted()
        {
            _mockSchedulerTrackingSeasonRepository.Object.Add(new SchedulerTrackingRugbySeason()
            {
                SeasonId = Guid.NewGuid(),
                RugbySeasonStatus = RugbySeasonStatus.InProgress
            });

            await _rugbyService.CleanupSchedulerTrackingRugbySeasonsTable();

            Assert.AreEqual(1, await _mockSchedulerTrackingSeasonRepository.Object.CountAsync());
        }

        [Test]
        public async Task CleanupSeasonsTable_SeasonIsEnded_IsDeleted()
        {
            _mockSchedulerTrackingSeasonRepository.Object.Add(new SchedulerTrackingRugbySeason()
            {
                SeasonId = Guid.NewGuid(),
                RugbySeasonStatus = RugbySeasonStatus.Ended
            });

            await _mockSchedulerTrackingSeasonRepository.Object.SaveAsync();

            await _rugbyService.CleanupSchedulerTrackingRugbySeasonsTable();

            Assert.AreEqual(0, await _mockSchedulerTrackingSeasonRepository.Object.CountAsync());
        }

        [Test]
        public async Task CleanupTournamentsTable_TournamentIsEnabled_NotDeleted()
        {
            var tournamentId = Guid.NewGuid();

            _mockTournamentRepository.Object.Add(new RugbyTournament()
            {
                Id = tournamentId,
                IsEnabled = true
            });

            await _mockTournamentRepository.Object.SaveAsync();

            _mockSchedulerTrackingTournamentsRepository.Object.Add(new SchedulerTrackingRugbyTournament()
            {
                TournamentId = tournamentId
            });

            await _mockSchedulerTrackingSeasonRepository.Object.SaveAsync();

            await _rugbyService.CleanupSchedulerTrackingRugbySeasonsTable();

            Assert.AreEqual(1, await _mockSchedulerTrackingTournamentsRepository.Object.CountAsync());
        }

        [Test]
        public async Task CleanupTournamentsTable_TournamentIsDisabled_IsDeleted()
        {
            var tournamentId = Guid.NewGuid();

            _mockTournamentRepository.Object.Add(new RugbyTournament()
            {
                Id = tournamentId,
                IsEnabled = false
            });

            await _mockTournamentRepository.Object.SaveAsync();

            _mockSchedulerTrackingTournamentsRepository.Object.Add(new SchedulerTrackingRugbyTournament()
            {
                TournamentId = tournamentId
            });

            await _mockSchedulerTrackingSeasonRepository.Object.SaveAsync();

            await _rugbyService.CleanupSchedulerTrackingRugbyTournamentsTable();

            Assert.AreEqual(0, await _mockSchedulerTrackingTournamentsRepository.Object.CountAsync());
        }

        [Test]
        public async Task LiveGame_LiveGameCount()
        {
            Guid fixtureGuid = Guid.NewGuid();
            DateTime fixtureStartDate = DateTime.UtcNow - TimeSpan.FromMinutes(1);

            _mockFixtureRepository.Object.Add(
                new RugbyFixture()
                {
                    Id = fixtureGuid,
                    RugbyFixtureStatus = RugbyFixtureStatus.PreMatch,
                    StartDateTime = fixtureStartDate,
                });

            _mockSchedulerTrackingFixtureRepository.Object.Add(
                new SchedulerTrackingRugbyFixture()
                {
                    FixtureId = fixtureGuid,
                    RugbyFixtureStatus = RugbyFixtureStatus.PreMatch,
                    SchedulerStateFixtures = SchedulerStateForRugbyFixturePolling.PreLivePolling
                });

            Assert.AreEqual(1, await _rugbyService.GetLiveFixturesCount(CancellationToken.None));
        }

        [Test]
        public async Task LiveGame_LiveGames_Getter_Count()
        {
            Guid tournamentGuid = Guid.NewGuid();

            var testTournament = new RugbyTournament()
            {
                Id = tournamentGuid,
                Name = "testTournament",
                IsEnabled = true
            };

            _mockTournamentRepository.Object.Add(
                testTournament);

            Guid fixtureGuid = Guid.NewGuid();
            DateTime fixtureStartDate = DateTime.UtcNow - TimeSpan.FromMinutes(1);

            _mockFixtureRepository.Object.Add(
                new RugbyFixture()
                {
                    Id = fixtureGuid,
                    RugbyTournament = testTournament,
                    RugbyFixtureStatus = RugbyFixtureStatus.PreMatch,
                    StartDateTime = fixtureStartDate,
                });

            _mockSchedulerTrackingFixtureRepository.Object.Add(
                new SchedulerTrackingRugbyFixture()
                {
                    FixtureId = fixtureGuid,
                    TournamentId = tournamentGuid,
                    RugbyFixtureStatus = RugbyFixtureStatus.PreMatch,
                    SchedulerStateFixtures = SchedulerStateForRugbyFixturePolling.PreLivePolling
                });

            var liveGames = await _rugbyService.GetLiveFixturesForCurrentTournament(CancellationToken.None, tournamentGuid);

            Assert.AreEqual(1, liveGames.Count());
        }

        [Test]
        public async Task LiveGame_Status()
        {
            Guid tournamentGuid = Guid.NewGuid();

            var testTournament = new RugbyTournament()
            {
                Id = tournamentGuid,
                Name = "testTournament",
                IsEnabled = true
            };

            _mockTournamentRepository.Object.Add(
                testTournament);

            Guid fixtureGuid = Guid.NewGuid();
            DateTime fixtureStartDate = DateTime.UtcNow - TimeSpan.FromMinutes(1);

            _mockFixtureRepository.Object.Add(
                new RugbyFixture()
                {
                    Id = fixtureGuid,
                    RugbyTournament = testTournament,
                    RugbyFixtureStatus = RugbyFixtureStatus.PreMatch,
                    StartDateTime = fixtureStartDate,
                });

            _mockSchedulerTrackingFixtureRepository.Object.Add(
                new SchedulerTrackingRugbyFixture()
                {
                    FixtureId = fixtureGuid,
                    TournamentId = tournamentGuid,
                    RugbyFixtureStatus = RugbyFixtureStatus.PreMatch,
                    SchedulerStateFixtures = SchedulerStateForRugbyFixturePolling.PreLivePolling
                });

            var liveGames = await _rugbyService.GetLiveFixturesForCurrentTournament(CancellationToken.None, tournamentGuid);

            Assert.AreEqual(RugbyFixtureStatus.PreMatch, liveGames.First().RugbyFixtureStatus);
        }

        [Test]
        public async Task LiveGame_Getter_Count_0_NotLive()
        {
            Guid tournamentGuid = Guid.NewGuid();

            var testTournament = new RugbyTournament()
            {
                Id = tournamentGuid,
                Name = "testTournament",
                IsEnabled = true
            };

            _mockTournamentRepository.Object.Add(
                testTournament);

            Guid fixtureGuid = Guid.NewGuid();
            DateTime fixtureStartDate = DateTime.UtcNow + TimeSpan.FromMinutes(20);

            _mockFixtureRepository.Object.Add(
                new RugbyFixture()
                {
                    Id = fixtureGuid,
                    RugbyTournament = testTournament,
                    RugbyFixtureStatus = RugbyFixtureStatus.PreMatch,
                    StartDateTime = fixtureStartDate,
                });

            _mockSchedulerTrackingFixtureRepository.Object.Add(
                new SchedulerTrackingRugbyFixture()
                {
                    FixtureId = fixtureGuid,
                    TournamentId = tournamentGuid,
                    RugbyFixtureStatus = RugbyFixtureStatus.PreMatch,
                    SchedulerStateFixtures = SchedulerStateForRugbyFixturePolling.SchedulingNotYetStarted
                });

            var liveGames = await _rugbyService.GetLiveFixturesForCurrentTournament(CancellationToken.None, tournamentGuid);

            Assert.AreEqual(0, liveGames.Count());
        }

        [Test]
        public async Task LiveGame_Getter_Count_1_Live()
        {
            Guid tournamentGuid = Guid.NewGuid();

            var testTournament = new RugbyTournament()
            {
                Id = tournamentGuid,
                Name = "testTournament",
                IsEnabled = true
            };

            _mockTournamentRepository.Object.Add(
                testTournament);

            Guid fixtureGuid = Guid.NewGuid();
            DateTime fixtureStartDate = DateTime.UtcNow + TimeSpan.FromMinutes(14);

            _mockFixtureRepository.Object.Add(
                new RugbyFixture()
                {
                    Id = fixtureGuid,
                    RugbyTournament = testTournament,
                    RugbyFixtureStatus = RugbyFixtureStatus.PreMatch,
                    StartDateTime = fixtureStartDate,
                });

            _mockSchedulerTrackingFixtureRepository.Object.Add(
                new SchedulerTrackingRugbyFixture()
                {
                    FixtureId = fixtureGuid,
                    TournamentId = tournamentGuid,
                    RugbyFixtureStatus = RugbyFixtureStatus.PreMatch,
                    SchedulerStateFixtures = SchedulerStateForRugbyFixturePolling.PreLivePolling
                });

            var liveGames = await _rugbyService.GetLiveFixturesForCurrentTournament(CancellationToken.None, tournamentGuid);

            Assert.AreEqual(1, liveGames.Count());
        }

        [Test]
        public async Task CompletedFixture_Getter_Count_0_NotLive()
        {
            Guid tournamentGuid = Guid.NewGuid();

            var testTournament = new RugbyTournament()
            {
                Id = tournamentGuid,
                Name = "testTournament",
                IsEnabled = true
            };

            _mockTournamentRepository.Object.Add(
                testTournament);

            Guid fixtureGuid = Guid.NewGuid();
            DateTime fixtureStartDate = DateTime.UtcNow + TimeSpan.FromMinutes(20);

            _mockFixtureRepository.Object.Add(
                new RugbyFixture()
                {
                    Id = fixtureGuid,
                    RugbyTournament = testTournament,
                    RugbyFixtureStatus = RugbyFixtureStatus.FirstHalf,
                    StartDateTime = fixtureStartDate,
                });

            _mockSchedulerTrackingFixtureRepository.Object.Add(
                new SchedulerTrackingRugbyFixture()
                {
                    FixtureId = fixtureGuid,
                    TournamentId = tournamentGuid,
                    RugbyFixtureStatus = RugbyFixtureStatus.PreMatch,
                    SchedulerStateFixtures = SchedulerStateForRugbyFixturePolling.LivePolling
                });

            var completedFixtures = await _rugbyService.GetCompletedFixtures();

            Assert.AreEqual(0, completedFixtures.Count());
        }

        [Test]
        public async Task CompletedFixture_Getter_Count_1_NotLive()
        {
            Guid tournamentGuid = Guid.NewGuid();

            var testTournament = new RugbyTournament()
            {
                Id = tournamentGuid,
                Name = "testTournament",
                IsEnabled = true
            };

            _mockTournamentRepository.Object.Add(
                testTournament);

            Guid fixtureGuid = Guid.NewGuid();
            DateTime fixtureStartDate = DateTime.UtcNow + TimeSpan.FromMinutes(20);

            _mockFixtureRepository.Object.Add(
                new RugbyFixture()
                {
                    Id = fixtureGuid,
                    RugbyTournament = testTournament,
                    RugbyFixtureStatus = RugbyFixtureStatus.Result,
                    StartDateTime = fixtureStartDate,
                });

            _mockSchedulerTrackingFixtureRepository.Object.Add(
                new SchedulerTrackingRugbyFixture()
                {
                    FixtureId = fixtureGuid,
                    TournamentId = tournamentGuid,
                    RugbyFixtureStatus = RugbyFixtureStatus.PreMatch,
                    SchedulerStateFixtures = SchedulerStateForRugbyFixturePolling.SchedulingCompleted
                });

            var completedFixtures = await _rugbyService.GetCompletedFixtures();

            Assert.AreEqual(1, completedFixtures.Count());
        }

        [Test]
        public async Task PostponedFixture_Getter_Count_0()
        {
            Guid tournamentGuid = Guid.NewGuid();

            var testTournament = new RugbyTournament()
            {
                Id = tournamentGuid,
                Name = "testTournament",
                IsEnabled = true
            };

            _mockTournamentRepository.Object.Add(
                testTournament);

            Guid fixtureGuid = Guid.NewGuid();
            DateTime fixtureStartDate = DateTime.UtcNow - TimeSpan.FromHours(2);

            _mockFixtureRepository.Object.Add(
                new RugbyFixture()
                {
                    Id = fixtureGuid,
                    RugbyTournament = testTournament,
                    RugbyFixtureStatus = RugbyFixtureStatus.PreMatch,
                    StartDateTime = fixtureStartDate,
                });

            _mockSchedulerTrackingFixtureRepository.Object.Add(
                new SchedulerTrackingRugbyFixture()
                {
                    FixtureId = fixtureGuid,
                    TournamentId = tournamentGuid,
                    RugbyFixtureStatus = RugbyFixtureStatus.PreMatch,
                    SchedulerStateFixtures = SchedulerStateForRugbyFixturePolling.SchedulingCompleted
                });

            var postponedFixture = await _rugbyService.GetPostponedFixtures();

            Assert.AreEqual(0, postponedFixture.Count());
        }

        [Test]
        public async Task PostponedFixture_Getter_Count_1()
        {
            Guid tournamentGuid = Guid.NewGuid();

            var testTournament = new RugbyTournament()
            {
                Id = tournamentGuid,
                Name = "testTournament",
                IsEnabled = true
            };

            _mockTournamentRepository.Object.Add(
                testTournament);

            Guid fixtureGuid = Guid.NewGuid();
            DateTime fixtureStartDate = DateTime.UtcNow - TimeSpan.FromHours(4);

            _mockFixtureRepository.Object.Add(
                new RugbyFixture()
                {
                    Id = fixtureGuid,
                    RugbyTournament = testTournament,
                    RugbyFixtureStatus = RugbyFixtureStatus.PreMatch,
                    StartDateTime = fixtureStartDate,
                });

            _mockSchedulerTrackingFixtureRepository.Object.Add(
                new SchedulerTrackingRugbyFixture()
                {
                    FixtureId = fixtureGuid,
                    TournamentId = tournamentGuid,
                    RugbyFixtureStatus = RugbyFixtureStatus.PreMatch,
                    SchedulerStateFixtures = SchedulerStateForRugbyFixturePolling.SchedulingCompleted
                });

            var postponedFixture = await _rugbyService.GetPostponedFixtures();

            Assert.AreEqual(1, postponedFixture.Count());
        }

        [Test]
        public async Task GetPastDaysFixtures_Return_0()
        {
            Assert.AreEqual(0, (await _rugbyService.GetPastDaysFixtures(4)).Count());
        }

        [Test]
        public async Task GetPastDaysFixtures_Return_1()
        {
            _mockFixtureRepository.Object.Add(
                new RugbyFixture()
                {
                    Id = Guid.NewGuid(),
                    StartDateTime = DateTime.UtcNow
                });
            Assert.AreEqual(1, (await _rugbyService.GetPastDaysFixtures(4)).Count());
        }

        [Test]
        public async Task GetPastDaysFixtures_Return_1_With_1_Fixture_TooFarInThePast()
        {
            _mockFixtureRepository.Object.AddRange(
                new List<RugbyFixture>()
                {
                    new RugbyFixture()
                    {
                        Id = Guid.NewGuid(),
                        StartDateTime = DateTime.UtcNow
                    },
                    new RugbyFixture()
                    {
                        Id = Guid.NewGuid(),
                        StartDateTime = DateTime.UtcNow - TimeSpan.FromDays(5)
                    }
                });

            Assert.AreEqual(1, (await _rugbyService.GetPastDaysFixtures(4)).Count());
        }

        [Test]
        public async Task GetPastDaysFixtures_Return_2_With_0_Fixture_TooFarInThePast()
        {
            _mockFixtureRepository.Object.AddRange(
                new List<RugbyFixture>()
                {
                    new RugbyFixture()
                    {
                        Id = Guid.NewGuid(),
                        StartDateTime = DateTime.UtcNow
                    },
                    new RugbyFixture()
                    {
                        Id = Guid.NewGuid(),
                        StartDateTime = DateTime.UtcNow - TimeSpan.FromDays(1)
                    }
                });

            Assert.AreEqual(2, (await _rugbyService.GetPastDaysFixtures(4)).Count());
        }

        [Test]
        public async Task GetPastDaysFixtures_Return_0_With_2_Fixture_TooFarInThePast()
        {
            _mockFixtureRepository.Object.AddRange(
                new List<RugbyFixture>()
                {
                    new RugbyFixture()
                    {
                        Id = Guid.NewGuid(),
                        StartDateTime = DateTime.UtcNow - TimeSpan.FromDays(9)
                    },
                    new RugbyFixture()
                    {
                        Id = Guid.NewGuid(),
                        StartDateTime = DateTime.UtcNow - TimeSpan.FromDays(5)
                    }
                });

            Assert.AreEqual(0, (await _rugbyService.GetPastDaysFixtures(4)).Count());
        }
    }
}
