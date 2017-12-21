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
using SuperSportDataEngine.Common.Logging;
using System.Threading;
using System.Linq;

namespace SuperSportDataEngine.ApplicationLogic.Tests
{
    [Category("RugbyServiceTest")]
    public class RugbyServiceTest
    {
        RugbyService _rugbyService;
        TestPublicSportDataUnitOfWork _publicSportDataUnitOfWork;
        TestSystemSportDataUnitOfWork _systemSportDataUnitOfWork;

        [SetUp]
        public void SetUp()
        {
            _publicSportDataUnitOfWork = new TestPublicSportDataUnitOfWork();
            _systemSportDataUnitOfWork = new TestSystemSportDataUnitOfWork();

            _rugbyService = new RugbyService(
                _publicSportDataUnitOfWork,
                _systemSportDataUnitOfWork);
        }

        [Test]
        public async Task CleanupFixturesTable_OneFixtureTrackedToday_NotDeleted()
        {
            _systemSportDataUnitOfWork.SchedulerTrackingRugbyFixtures.Add(new SchedulerTrackingRugbyFixture() {
                    FixtureId = Guid.NewGuid(),
                    StartDateTime = DateTimeOffset.UtcNow,
                    RugbyFixtureStatus = RugbyFixtureStatus.PreMatch
            });
            
            await _rugbyService.CleanupSchedulerTrackingRugbyFixturesTable();

            Assert.AreEqual(1, await _systemSportDataUnitOfWork.SchedulerTrackingRugbyFixtures.CountAsync());
        }

        [Test]
        public async Task CleanupFixturesTable_OneFixtureTracked6MonthsAgo_Deleted()
        {
            _systemSportDataUnitOfWork.SchedulerTrackingRugbyFixtures.Add(new SchedulerTrackingRugbyFixture()
            {
                FixtureId = Guid.NewGuid(),
                StartDateTime = DateTimeOffset.UtcNow - TimeSpan.FromDays(181),
                RugbyFixtureStatus = RugbyFixtureStatus.Result
            });
            
            await _rugbyService.CleanupSchedulerTrackingRugbyFixturesTable();

            Assert.AreEqual(0, await _systemSportDataUnitOfWork.SchedulerTrackingRugbyFixtures.CountAsync());
        }

        [Test]
        public async Task CleanupFixturesTable_OneFixtureTrackedLessThan6MonthsAgo_NotDeleted()
        {
            _systemSportDataUnitOfWork.SchedulerTrackingRugbyFixtures.Add(new SchedulerTrackingRugbyFixture()
            {
                FixtureId = Guid.NewGuid(),
                StartDateTime = DateTimeOffset.UtcNow - TimeSpan.FromDays(179),
                RugbyFixtureStatus = RugbyFixtureStatus.Result
            });

            await _rugbyService.CleanupSchedulerTrackingRugbyFixturesTable();

            Assert.AreEqual(1, await _systemSportDataUnitOfWork.SchedulerTrackingRugbyFixtures.CountAsync());
        }

        [Test]
        public async Task CleanupSeasonsTable_SeasonIsCurrent_NotDeleted()
        {
            _systemSportDataUnitOfWork.SchedulerTrackingRugbySeasons.Add(new SchedulerTrackingRugbySeason()
            {
                SeasonId = Guid.NewGuid(),
                RugbySeasonStatus = RugbySeasonStatus.InProgress
            });

            await _rugbyService.CleanupSchedulerTrackingRugbySeasonsTable();

            Assert.AreEqual(1, await _systemSportDataUnitOfWork.SchedulerTrackingRugbySeasons.CountAsync());
        }

        [Test]
        public async Task CleanupSeasonsTable_SeasonIsEnded_IsDeleted()
        {
            _systemSportDataUnitOfWork.SchedulerTrackingRugbySeasons.Add(new SchedulerTrackingRugbySeason()
            {
                SeasonId = Guid.NewGuid(),
                RugbySeasonStatus = RugbySeasonStatus.Ended
            });
            
            await _rugbyService.CleanupSchedulerTrackingRugbySeasonsTable();

            Assert.AreEqual(0, await _systemSportDataUnitOfWork.SchedulerTrackingRugbySeasons.CountAsync());
        }

        [Test]
        public async Task CleanupTournamentsTable_TournamentIsEnabled_NotDeleted()
        {
            var tournamentId = Guid.NewGuid();

            _publicSportDataUnitOfWork.RugbyTournaments.Add(new RugbyTournament()
            {
                Id = tournamentId,
                IsEnabled = true
            });

            _systemSportDataUnitOfWork.SchedulerTrackingRugbyTournaments.Add(new SchedulerTrackingRugbyTournament()
            {
                TournamentId = tournamentId
            });
            
            await _rugbyService.CleanupSchedulerTrackingRugbySeasonsTable();

            Assert.AreEqual(1, await _systemSportDataUnitOfWork.SchedulerTrackingRugbyTournaments.CountAsync());
        }

        [Test]
        public async Task CleanupTournamentsTable_TournamentIsDisabled_IsDeleted()
        {
            var tournamentId = Guid.NewGuid();

            _publicSportDataUnitOfWork.RugbyTournaments.Add(new RugbyTournament()
            {
                Id = tournamentId,
                IsEnabled = false
            });

            _systemSportDataUnitOfWork.SchedulerTrackingRugbyTournaments.Add(new SchedulerTrackingRugbyTournament()
            {
                TournamentId = tournamentId
            });

            await _rugbyService.CleanupSchedulerTrackingRugbyTournamentsTable();

            Assert.AreEqual(0, await _systemSportDataUnitOfWork.SchedulerTrackingRugbyTournaments.CountAsync());
        }

        [Test]
        public async Task LiveGame_LiveGameCount()
        {
            Guid fixtureGuid = Guid.NewGuid();
            DateTime fixtureStartDate = DateTime.UtcNow - TimeSpan.FromMinutes(1);

            _publicSportDataUnitOfWork.RugbyFixtures.Add(
                new RugbyFixture()
                {
                    Id = fixtureGuid,
                    RugbyFixtureStatus = RugbyFixtureStatus.PreMatch,
                    StartDateTime = fixtureStartDate,
                });

            _systemSportDataUnitOfWork.SchedulerTrackingRugbyFixtures.Add(
                new SchedulerTrackingRugbyFixture()
                {
                    FixtureId = fixtureGuid,
                    RugbyFixtureStatus = RugbyFixtureStatus.PreMatch,
                    SchedulerStateFixtures = SchedulerStateForRugbyFixturePolling.PreLivePolling
                });

            Assert.AreEqual(1, await _rugbyService.GetLiveFixturesCount(CancellationToken.None));
        }

        [Test]
        public async Task LiveGame_LiveGameCount_OneFixtureDisabledInbound()
        {
            Guid fixtureGuid = Guid.NewGuid();
            DateTime fixtureStartDate = DateTime.UtcNow - TimeSpan.FromMinutes(1);

            _publicSportDataUnitOfWork.RugbyFixtures.Add(
                new RugbyFixture()
                {
                    Id = fixtureGuid,
                    RugbyFixtureStatus = RugbyFixtureStatus.PreMatch,
                    StartDateTime = fixtureStartDate,
                    IsDisabledInbound = true
                });

            _systemSportDataUnitOfWork.SchedulerTrackingRugbyFixtures.Add(
                new SchedulerTrackingRugbyFixture()
                {
                    FixtureId = fixtureGuid,
                    RugbyFixtureStatus = RugbyFixtureStatus.PreMatch,
                    SchedulerStateFixtures = SchedulerStateForRugbyFixturePolling.PreLivePolling
                });

            Assert.AreEqual(0, await _rugbyService.GetLiveFixturesCount(CancellationToken.None));
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

            _publicSportDataUnitOfWork.RugbyTournaments.Add(
                testTournament);

            Guid fixtureGuid = Guid.NewGuid();
            DateTime fixtureStartDate = DateTime.UtcNow - TimeSpan.FromMinutes(1);

            _publicSportDataUnitOfWork.RugbyFixtures.Add(
                new RugbyFixture()
                {
                    Id = fixtureGuid,
                    RugbyTournament = testTournament,
                    RugbyFixtureStatus = RugbyFixtureStatus.PreMatch,
                    StartDateTime = fixtureStartDate,
                });

            _systemSportDataUnitOfWork.SchedulerTrackingRugbyFixtures.Add(
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
        public async Task LiveGame_LiveGames_Getter_Count_OneFixtureDisabledInbound()
        {
            Guid tournamentGuid = Guid.NewGuid();

            var testTournament = new RugbyTournament()
            {
                Id = tournamentGuid,
                Name = "testTournament",
                IsEnabled = true
            };

            _publicSportDataUnitOfWork.RugbyTournaments.Add(
                testTournament);

            Guid fixtureGuid = Guid.NewGuid();
            DateTime fixtureStartDate = DateTime.UtcNow - TimeSpan.FromMinutes(1);

            _publicSportDataUnitOfWork.RugbyFixtures.Add(
                new RugbyFixture()
                {
                    Id = fixtureGuid,
                    RugbyTournament = testTournament,
                    RugbyFixtureStatus = RugbyFixtureStatus.PreMatch,
                    StartDateTime = fixtureStartDate,
                    IsDisabledInbound = true
                });

            _systemSportDataUnitOfWork.SchedulerTrackingRugbyFixtures.Add(
                new SchedulerTrackingRugbyFixture()
                {
                    FixtureId = fixtureGuid,
                    TournamentId = tournamentGuid,
                    RugbyFixtureStatus = RugbyFixtureStatus.PreMatch,
                    SchedulerStateFixtures = SchedulerStateForRugbyFixturePolling.PreLivePolling
                });

            var liveGames = await _rugbyService.GetLiveFixturesForCurrentTournament(CancellationToken.None, tournamentGuid);

            Assert.AreEqual(0, liveGames.Count());
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

            _publicSportDataUnitOfWork.RugbyTournaments.Add(
                testTournament);

            Guid fixtureGuid = Guid.NewGuid();
            DateTime fixtureStartDate = DateTime.UtcNow - TimeSpan.FromMinutes(1);

            _publicSportDataUnitOfWork.RugbyFixtures.Add(
                new RugbyFixture()
                {
                    Id = fixtureGuid,
                    RugbyTournament = testTournament,
                    RugbyFixtureStatus = RugbyFixtureStatus.PreMatch,
                    StartDateTime = fixtureStartDate,
                });

            _systemSportDataUnitOfWork.SchedulerTrackingRugbyFixtures.Add(
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

            _publicSportDataUnitOfWork.RugbyTournaments.Add(
                testTournament);

            Guid fixtureGuid = Guid.NewGuid();
            DateTime fixtureStartDate = DateTime.UtcNow + TimeSpan.FromMinutes(20);

            _publicSportDataUnitOfWork.RugbyFixtures.Add(
                new RugbyFixture()
                {
                    Id = fixtureGuid,
                    RugbyTournament = testTournament,
                    RugbyFixtureStatus = RugbyFixtureStatus.PreMatch,
                    StartDateTime = fixtureStartDate,
                });

            _systemSportDataUnitOfWork.SchedulerTrackingRugbyFixtures.Add(
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

            _publicSportDataUnitOfWork.RugbyTournaments.Add(
                testTournament);

            Guid fixtureGuid = Guid.NewGuid();
            DateTime fixtureStartDate = DateTime.UtcNow + TimeSpan.FromMinutes(14);

            _publicSportDataUnitOfWork.RugbyFixtures.Add(
                new RugbyFixture()
                {
                    Id = fixtureGuid,
                    RugbyTournament = testTournament,
                    RugbyFixtureStatus = RugbyFixtureStatus.PreMatch,
                    StartDateTime = fixtureStartDate,
                });

            _systemSportDataUnitOfWork.SchedulerTrackingRugbyFixtures.Add(
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

            _publicSportDataUnitOfWork.RugbyTournaments.Add(
                testTournament);

            Guid fixtureGuid = Guid.NewGuid();
            DateTime fixtureStartDate = DateTime.UtcNow + TimeSpan.FromMinutes(20);

            _publicSportDataUnitOfWork.RugbyFixtures.Add(
                new RugbyFixture()
                {
                    Id = fixtureGuid,
                    RugbyTournament = testTournament,
                    RugbyFixtureStatus = RugbyFixtureStatus.FirstHalf,
                    StartDateTime = fixtureStartDate,
                });

            _systemSportDataUnitOfWork.SchedulerTrackingRugbyFixtures.Add(
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

            _publicSportDataUnitOfWork.RugbyTournaments.Add(
                testTournament);

            Guid fixtureGuid = Guid.NewGuid();
            DateTime fixtureStartDate = DateTime.UtcNow + TimeSpan.FromMinutes(20);

            _publicSportDataUnitOfWork.RugbyFixtures.Add(
                new RugbyFixture()
                {
                    Id = fixtureGuid,
                    RugbyTournament = testTournament,
                    RugbyFixtureStatus = RugbyFixtureStatus.Result,
                    StartDateTime = fixtureStartDate,
                });

            _systemSportDataUnitOfWork.SchedulerTrackingRugbyFixtures.Add(
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

            _publicSportDataUnitOfWork.RugbyTournaments.Add(
                testTournament);

            Guid fixtureGuid = Guid.NewGuid();
            DateTime fixtureStartDate = DateTime.UtcNow - TimeSpan.FromHours(2);

            _publicSportDataUnitOfWork.RugbyFixtures.Add(
                new RugbyFixture()
                {
                    Id = fixtureGuid,
                    RugbyTournament = testTournament,
                    RugbyFixtureStatus = RugbyFixtureStatus.PreMatch,
                    StartDateTime = fixtureStartDate,
                });

            _systemSportDataUnitOfWork.SchedulerTrackingRugbyFixtures.Add(
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

            _publicSportDataUnitOfWork.RugbyTournaments.Add(
                testTournament);

            Guid fixtureGuid = Guid.NewGuid();
            DateTime fixtureStartDate = DateTime.UtcNow - TimeSpan.FromHours(4);

            _publicSportDataUnitOfWork.RugbyFixtures.Add(
                new RugbyFixture()
                {
                    Id = fixtureGuid,
                    RugbyTournament = testTournament,
                    RugbyFixtureStatus = RugbyFixtureStatus.PreMatch,
                    StartDateTime = fixtureStartDate,
                });

            _systemSportDataUnitOfWork.SchedulerTrackingRugbyFixtures.Add(
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
            _publicSportDataUnitOfWork.RugbyFixtures.Add(
                new RugbyFixture()
                {
                    Id = Guid.NewGuid(),
                    StartDateTime = DateTime.Today.Subtract(TimeSpan.FromDays(1))
                });
            Assert.AreEqual(1, (await _rugbyService.GetPastDaysFixtures(4)).Count());
        }

        [Test]
        public async Task GetPastDaysFixtures_Return_1_With_1_Fixture_TooFarInThePast()
        {
            _publicSportDataUnitOfWork.RugbyFixtures.AddRange(
                new List<RugbyFixture>()
                {
                    new RugbyFixture()
                    {
                        Id = Guid.NewGuid(),
                        StartDateTime = DateTime.Today.Subtract(TimeSpan.FromDays(1))
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
            _publicSportDataUnitOfWork.RugbyFixtures.AddRange(
                new List<RugbyFixture>()
                {
                    new RugbyFixture()
                    {
                        Id = Guid.NewGuid(),
                        StartDateTime = DateTime.Today.Subtract(TimeSpan.FromDays(1))
                    },
                    new RugbyFixture()
                    {
                        Id = Guid.NewGuid(),
                        StartDateTime = DateTime.Today.Subtract(TimeSpan.FromDays(2))
                    }
                });

            Assert.AreEqual(2, (await _rugbyService.GetPastDaysFixtures(4)).Count());
        }

        [Test]
        public async Task GetPastDaysFixtures_Return_0_With_2_Fixture_TooFarInThePast()
        {
            _publicSportDataUnitOfWork.RugbyFixtures.AddRange(
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
