using NUnit.Framework;
using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces.LegacyFeed;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.UnitOfWork;
using SuperSportDataEngine.ApplicationLogic.Services.LegacyFeed;
using SuperSportDataEngine.Tests.Common.Repositories.Test;
using System;

namespace SuperSportDataEngine.ApplicationLogic.Tests.Services
{
    public class MotorsportLegacyFeedServiceTests
    {
        private IMotorsportLegacyFeedService _legacyFeedService;
        private IPublicSportDataUnitOfWork _publicSportDataUnitOfWork;

        [SetUp]
        public void Setup()
        {
            //Setup repo
            _publicSportDataUnitOfWork = new TestPublicSportDataUnitOfWork();

            //Setup service
            _legacyFeedService = new MotorsportLegacyFeedService(_publicSportDataUnitOfWork);
        }

        [Test]
        public void IsCategoryInvalid_ReturnsFalse_IfSlugIsInRepo()
        {
            //Setup service with expected data
            const string slugInRepo = "test-slug";
            var motorsportLeague = new MotorsportLeague { Slug = slugInRepo };

            _publicSportDataUnitOfWork = new TestPublicSportDataUnitOfWork();
            _publicSportDataUnitOfWork.MotorsportLeagues.Add(motorsportLeague);

            var service = new MotorsportLegacyFeedService(_publicSportDataUnitOfWork);

            // Assert
            Assert.False(service.IsCategoryInvalid(slugInRepo).Result);
        }

        [Test]
        public void IsCategoryInvalid_ReturnsTrue_IfSlugIsNotInRepo()
        {
            //Setup service with an empty repo
            _publicSportDataUnitOfWork = new TestPublicSportDataUnitOfWork();
            _legacyFeedService = new MotorsportLegacyFeedService(_publicSportDataUnitOfWork);

            //Setup test sample
            const string slugNotInRepo = "test-slug";

            // Assert
            Assert.True(_legacyFeedService.IsCategoryInvalid(slugNotInRepo).Result);
        }
       
        [Test]
        public void WhenRepoIsInitializedButEmpty_MethodsMustNotThrowExceptions()
        {
            //Setup service
            _publicSportDataUnitOfWork = new TestPublicSportDataUnitOfWork();

            _legacyFeedService = new MotorsportLegacyFeedService(_publicSportDataUnitOfWork);

            //Setup test samples
            const int testId = int.MinValue;
            const string testSlug = "test-slug";

            //Assert for all methodos under test
            Assert.DoesNotThrowAsync(() => _legacyFeedService.GetDriverStandings(testSlug));
            Assert.DoesNotThrowAsync(() => _legacyFeedService.GetLatestGrid(testSlug));
            Assert.DoesNotThrowAsync(() => _legacyFeedService.GetGridForRaceEventId(testSlug, testId));
            Assert.DoesNotThrowAsync(() => _legacyFeedService.GetLatestResult(testSlug));
            Assert.DoesNotThrowAsync(() => _legacyFeedService.GetResultsForRaceEventId(testSlug, testId));
            Assert.DoesNotThrowAsync(() => _legacyFeedService.GetSchedules(testSlug, true));
            Assert.DoesNotThrowAsync(() => _legacyFeedService.GetTeamStandings(testSlug));
            Assert.DoesNotThrowAsync(() => _legacyFeedService.IsCategoryInvalid(testSlug));
        }

        [Test]
        public void WhenRepoIsNotInitialized_MethodsMustThrowException()
        {
            //Setup service with an empty repo
            _legacyFeedService = new MotorsportLegacyFeedService(null);

            //Setup test samples
            const int testId = int.MinValue;
            const string testSlug = "test-slug";

            //Assert for all methodos under test
            Assert.ThrowsAsync<NullReferenceException>(() => _legacyFeedService.GetDriverStandings(testSlug));
            Assert.ThrowsAsync<NullReferenceException>(() => _legacyFeedService.GetTeamStandings(testSlug));
            Assert.ThrowsAsync<NullReferenceException>(() => _legacyFeedService.GetGridForRaceEventId(testSlug, testId));
            Assert.ThrowsAsync<NullReferenceException>(() => _legacyFeedService.GetLatestGrid(testSlug));
            Assert.ThrowsAsync<NullReferenceException>(() => _legacyFeedService.GetLatestResult(testSlug));
            Assert.ThrowsAsync<NullReferenceException>(() => _legacyFeedService.GetResultsForRaceEventId(testSlug, testId));
            Assert.ThrowsAsync<NullReferenceException>(() => _legacyFeedService.GetSchedules(testSlug, false));
            Assert.ThrowsAsync<NullReferenceException>(() => _legacyFeedService.IsCategoryInvalid(testSlug));
        }
    }
}