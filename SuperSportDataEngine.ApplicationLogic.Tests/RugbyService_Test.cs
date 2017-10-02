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

            RugbyService = new RugbyService(
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
                RugbyFixtureStatus = RugbyFixtureStatus.Final
            });

            await MockSchedulerTrackingFixtureRepository.Object.SaveAsync();

            await RugbyService.CleanupSchedulerTrackingRugbyFixturesTable();

            Assert.AreEqual(0, await MockSchedulerTrackingFixtureRepository.Object.CountAsync());
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
    }
}
