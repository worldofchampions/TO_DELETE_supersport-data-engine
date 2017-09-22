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

namespace SuperSportDataEngine.ApplicationLogic.Tests
{
    public class RugbyService_Test
    {
        RugbyService RugbyService;
        Mock<TestEntityFrameworkRepository<SchedulerTrackingRugbyFixture>> MockSchedulerTrackingFixtureRepository;
        Mock<TestEntityFrameworkRepository<SchedulerTrackingRugbySeason>> MockSchedulerTrackingSeasonRepository;
        Mock<TestEntityFrameworkRepository<RugbyFixture>> MockFixtureRepository;
        Mock<TestEntityFrameworkRepository<RugbySeason>> MockSeasonRepository;
        Mock<TestEntityFrameworkRepository<RugbyTournament>> MockTournamentRepository;

        [SetUp]
        public void SetUp()
        {
            MockSchedulerTrackingFixtureRepository =
                    new Mock<TestEntityFrameworkRepository<SchedulerTrackingRugbyFixture>>(new List<SchedulerTrackingRugbyFixture>());

            MockSchedulerTrackingSeasonRepository =
                    new Mock<TestEntityFrameworkRepository<SchedulerTrackingRugbySeason>>(new List<SchedulerTrackingRugbySeason>());

            MockFixtureRepository =
                    new Mock<TestEntityFrameworkRepository<RugbyFixture>>(new List<RugbyFixture>());

            MockTournamentRepository =
                    new Mock<TestEntityFrameworkRepository<RugbyTournament>>(new List<RugbyTournament>());

            MockSeasonRepository =
                    new Mock<TestEntityFrameworkRepository<RugbySeason>>(new List<RugbySeason>());

            RugbyService = new RugbyService(
                MockTournamentRepository.Object,
                MockSeasonRepository.Object,
                MockSchedulerTrackingSeasonRepository.Object,
                MockFixtureRepository.Object,
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

            await RugbyService.CleanupSchedulerTrackingRugbyFixturesTable();

            Assert.AreEqual(0, await MockSchedulerTrackingFixtureRepository.Object.CountAsync());
        }
    }
}
