using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hangfire;
using Moq;
using NUnit.Framework;
using SuperSportDataEngine.Application.Service.SchedulerClient.ScheduledManager;
using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
using SuperSportDataEngine.ApplicationLogic.Services;
using SuperSportDataEngine.Common.Logging;
using SuperSportDataEngine.Tests.Common.Repositories.Test;

namespace SuperSportDataEngine.Application.Service.SchedulerClient.Tests.ScheduledManagerTests
{
    public class FixturesManagerJobTests
    {
        FixturesManagerJob _fixturesManagerJob;
        Mock<IRugbyService> _mockRugbyService;
        Mock<IRugbyIngestWorkerService> _mockRugbyIngestWorkerService;
        private TestSystemSportDataUnitOfWork _mockUnitOfWork;
        Mock<IRecurringJobManager> _mockRecurringJobManager;

        [SetUp]
        public void SetUp()
        {
            _mockRugbyService = new Mock<IRugbyService>();
            _mockRugbyIngestWorkerService = new Mock<IRugbyIngestWorkerService>();
            _mockRecurringJobManager = new Mock<IRecurringJobManager>();
            _mockUnitOfWork = new TestSystemSportDataUnitOfWork();

            _fixturesManagerJob =
                new FixturesManagerJob(
                    _mockRecurringJobManager.Object,
                    _mockUnitOfWork,
                    _mockRugbyService.Object,
                    _mockRugbyIngestWorkerService.Object
                    );
        }

        [Test]
        public async Task NoExceptionsWhenCallingDoWork()
        {
            _mockRugbyService.Setup(m => m.GetCurrentDayFixturesForActiveTournaments()).Returns(
                    Task.FromResult(
                        new List<RugbyFixture>())
                );

            try
            {
                await _fixturesManagerJob.DoWorkAsync();
            }
            catch (Exception e)
            {
                Assert.Fail();
            }

        }
    }
}
