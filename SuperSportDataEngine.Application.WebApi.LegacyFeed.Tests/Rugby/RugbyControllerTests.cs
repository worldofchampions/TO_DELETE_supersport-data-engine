namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Tests.Rugby
{
    using Moq;
    using NUnit.Framework;
    using SuperSportDataEngine.Application.WebApi.LegacyFeed.Controllers;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces.DeprecatedFeed;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
    using SuperSportDataEngine.ApplicationLogic.Services;
    using SuperSportDataEngine.Common.Interfaces;
    using SuperSportDataEngine.Common.Logging;
    using SuperSportDataEngine.Tests.Common.Repositories.Test;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Http.Results;
    using Match = SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Shared.Match;

    [Category("LegacyFeed")]
    public class RugbyControllerTests
    {
        private TestPublicSportDataUnitOfWork _publicSportDataUnitOfWork;
        private TestSystemSportDataUnitOfWork _systemSportDataUnitOfWork;
        private ICache _cache;
        private RugbyController _controller;
        private RugbyController _controllerWithMockRugbyService;
        private Mock<IRugbyService> _mockRugbyService;
        private Mock<ILoggingService> _mockLogger;

        public RugbyControllerTests()
        {
            AutoMapperConfig.InitializeMappings();
        }

        [SetUp]
        public void Setup()
        {
            _publicSportDataUnitOfWork = new TestPublicSportDataUnitOfWork();
            _systemSportDataUnitOfWork = new TestSystemSportDataUnitOfWork();
            _mockLogger = new Mock<ILoggingService>();

            _cache = new TestCache();
            var rugbyService = new RugbyService(
                _publicSportDataUnitOfWork,
                _systemSportDataUnitOfWork,
                _mockLogger.Object);

            _mockRugbyService = new Mock<IRugbyService>();
            _controllerWithMockRugbyService = new RugbyController(
                _mockRugbyService.Object,
                new Mock<IDeprecatedFeedIntegrationServiceRugby>().Object,
                _cache,
                new Mock<ILoggingService>().Object);

            _controller = new RugbyController(
                rugbyService,
                new Mock<IDeprecatedFeedIntegrationServiceRugby>().Object,
                _cache,
                new Mock<ILoggingService>().Object);
        }

        [Test]
        // When there are no fixtures today,
        // there should be no fixtures being served out on the feed.
        public async Task NoFixturesToday()
        {
            var result = await _controller.GetTodayFixtures();
            if (result is OkNegotiatedContentResult<List<Match>> content)
            {
                var liveFixtures = content.Content;

                if (liveFixtures != null)
                    Assert.AreEqual(0, liveFixtures.Count);
                else
                {
                    Assert.Fail();
                }
            }
        }

        [Test]
        // When there is one fixture today,
        // that fixtures should be served out.
        public async Task OneFixtureToday()
        {
            var rugbyTournament = new RugbyTournament()
            {
                IsEnabled = true
            };

            _publicSportDataUnitOfWork.RugbyTournaments.Add(
                rugbyTournament);

            _publicSportDataUnitOfWork.RugbyFixtures.Add(
                new RugbyFixture()
                {
                    RugbyTournament = rugbyTournament,
                    StartDateTime = DateTime.UtcNow,
                });

            var result = await _controller.GetTodayFixtures();
            if (result is OkNegotiatedContentResult<List<Match>> content)
            {
                var liveFixtures = content.Content;

                if (liveFixtures != null)
                    Assert.AreEqual(1, liveFixtures.Count);
                else
                {
                    Assert.Fail();
                }
            }
        }

        [Test]
        // When the live endpoint is being called twice,
        // The service should only be called once.
        // This checks if the cache is queried and returns the object from cache.
        public async Task EndpointCalledTwice_ServiceCalledOnce()
        {
            var rugbyTournament = new RugbyTournament()
            {
                IsEnabled = true
            };

            _publicSportDataUnitOfWork.RugbyTournaments.Add(
                rugbyTournament);

            _mockRugbyService.Setup(m => m.GetCurrentDayFixturesForActiveTournaments()).Returns(
                Task.FromResult(new List<RugbyFixture>()));

            await _controllerWithMockRugbyService.GetTodayFixtures();
            await _controllerWithMockRugbyService.GetTodayFixtures();

            _mockRugbyService.Verify(m => m.GetCurrentDayFixturesForActiveTournaments(), Times.Once());
        }

        [Test]
        // When there are two fixtures,
        // One is today, and one in the past
        // Only one fixture is returned.
        // Also check if the fixture returned is the one that is today.
        public async Task OneFixturePast_OneFixtureToday()
        {
            var rugbyTournament = new RugbyTournament()
            {
                IsEnabled = true
            };

            _publicSportDataUnitOfWork.RugbyTournaments.Add(
                rugbyTournament);

            _publicSportDataUnitOfWork.RugbyFixtures.Add(
                new RugbyFixture()
                {
                    RugbyTournament = rugbyTournament,
                    StartDateTime = DateTime.UtcNow,
                    LegacyFixtureId = 10
                });

            _publicSportDataUnitOfWork.RugbyFixtures.Add(
                new RugbyFixture()
                {
                    RugbyTournament = rugbyTournament,
                    StartDateTime = DateTime.UtcNow - TimeSpan.FromHours(24),
                    LegacyFixtureId = 0
                });

            var result = await _controller.GetTodayFixtures();
            if (result is OkNegotiatedContentResult<List<Match>> content)
            {
                var liveFixtures = content.Content;

                if (liveFixtures != null)
                {
                    Assert.AreEqual(1, liveFixtures.Count);
                    Assert.AreEqual(10, liveFixtures[0].MatchID);
                }
                else
                {
                    Assert.Fail();
                }
            }
        }

        [Test]
        // When there is a fixture that has started just before 12am UTC and the
        // fixture is in progress, also show this fixture on the live endpoint.
        public async Task OneInProgressFixtureStartedEndOfPreviousDay()
        {
            var rugbyTournament = new RugbyTournament()
            {
                IsEnabled = true
            };

            _publicSportDataUnitOfWork.RugbyTournaments.Add(
                rugbyTournament);

            _publicSportDataUnitOfWork.RugbyFixtures.Add(
                new RugbyFixture()
                {
                    RugbyTournament = rugbyTournament,
                    StartDateTime = DateTime.UtcNow,
                });

            _publicSportDataUnitOfWork.RugbyFixtures.Add(
                new RugbyFixture()
                {
                    RugbyTournament = rugbyTournament,
                    StartDateTime = DateTime.UtcNow.Date - TimeSpan.FromMinutes(1),
                    RugbyFixtureStatus = RugbyFixtureStatus.FirstHalf
                });

            var result = await _controller.GetTodayFixtures();
            if (result is OkNegotiatedContentResult<List<Match>> content)
            {
                var liveFixtures = content.Content;

                if (liveFixtures != null)
                    Assert.AreEqual(2, liveFixtures.Count);
                else
                {
                    Assert.Fail();
                }
            }
        }

        [Test]
        // When there is a fixture that has started long before 12am UTC and the
        // fixture is in progress, dont show this fixture on the feed.
        public async Task OneFixtureStartedLongAgoPreviousDay()
        {
            var rugbyTournament = new RugbyTournament()
            {
                IsEnabled = true
            };

            _publicSportDataUnitOfWork.RugbyTournaments.Add(
                rugbyTournament);

            _publicSportDataUnitOfWork.RugbyFixtures.Add(
                new RugbyFixture()
                {
                    RugbyTournament = rugbyTournament,
                    StartDateTime = DateTime.UtcNow,
                });

            _publicSportDataUnitOfWork.RugbyFixtures.Add(
                new RugbyFixture()
                {
                    RugbyTournament = rugbyTournament,
                    StartDateTime = DateTime.UtcNow.Date - TimeSpan.FromMinutes(120),
                    RugbyFixtureStatus = RugbyFixtureStatus.FirstHalf
                });

            var result = await _controller.GetTodayFixtures();
            if (result is OkNegotiatedContentResult<List<Match>> content)
            {
                var liveFixtures = content.Content;

                if (liveFixtures != null)
                    Assert.AreEqual(1, liveFixtures.Count);
                else
                {
                    Assert.Fail();
                }
            }
        }
    }

    internal class TestCache : ICache
    {
        private readonly Dictionary<string, object> _database;

        public TestCache()
        {
            _database = new Dictionary<string, object>();
        }

        public void Add<T>(string key, T cacheObject, TimeSpan? ttl = null, string parentKey = null) where T : class
        {
            if (!_database.ContainsKey(key))
                _database.Add(key, cacheObject);
        }

        public void Remove(string key)
        {
            _database.Remove(key);
        }

        public void SetParentExpiry(string parentKey, TimeSpan ttl)
        {
        }

        public Task<T> GetAsync<T>(string key) where T : class
        {
            return _database.ContainsKey(key) ? Task.FromResult((T)_database[key]) : null;
        }
    }
}
