using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.UnitOfWork;
using SuperSportDataEngine.ApplicationLogic.Services.LegacyFeed;

namespace SuperSportDataEngine.ApplicationLogic.Tests.Services
{
    public class MotorsportLegacyFeedServiceTests
    {
        [Test]
        public async Task WhenNoDriverStandingsInRepo_MustNotThrowExceptionAsync()
        {
            try
            {
                //Setup repo
                var publicSportDataUnitOfWork = new Mock<IPublicSportDataUnitOfWork>();
                publicSportDataUnitOfWork.Setup(mf => mf.MotorsportDriverStandings.All()).Returns(new List<MotorsportDriverStanding>());
                publicSportDataUnitOfWork.Setup(mf => mf.MotorsportSeasons.All()).Returns(new List<MotorsportSeason>());

                //Setup service
                var legacyFeedService = new MotorsportLegacyFeedService(publicSportDataUnitOfWork.Object);

                //Invoke method under test
                await legacyFeedService.GetDriverStandings("test-slug");
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }

        [Test]
        public async Task WhenNoTeamStandingsInRepo_MustNotThrowExceptionAsync()
        {
            try
            {
                //Setup repo
                var publicSportDataUnitOfWork = new Mock<IPublicSportDataUnitOfWork>();
                publicSportDataUnitOfWork.Setup(mf => mf.MotorsportTeamStandings.All()).Returns(new List<MotorsportTeamStanding>());
                publicSportDataUnitOfWork.Setup(mf => mf.MotorsportSeasons.All()).Returns(new List<MotorsportSeason>());

                //Setup service
                var legacyFeedService = new MotorsportLegacyFeedService(publicSportDataUnitOfWork.Object);

                //Invoke method under test
                await legacyFeedService.GetTeamStandings("test-slug");
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected no exception, but got: " + ex.Message);
            }
        }
    }
}