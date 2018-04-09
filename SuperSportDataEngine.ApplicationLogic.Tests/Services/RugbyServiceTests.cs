using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models.Enums;
using SuperSportDataEngine.ApplicationLogic.Services;
using SuperSportDataEngine.Tests.Common.Repositories.Test;

namespace SuperSportDataEngine.ApplicationLogic.Tests.Services
{
    [Category("RugbyService")]
    public class RugbyServiceTests
    {
        private TestPublicSportDataUnitOfWork _publicSportDataUnitOfWork;
        private TestSystemSportDataUnitOfWork _systemSportDataUnitOfWork;
        private RugbyService _rugbyService;

        [SetUp]
        public void Setup()
        {
            _publicSportDataUnitOfWork = new TestPublicSportDataUnitOfWork();
            _systemSportDataUnitOfWork = new TestSystemSportDataUnitOfWork();

            _rugbyService = 
                new RugbyService(
                    _publicSportDataUnitOfWork,
                    _systemSportDataUnitOfWork);

        }

        [Test]
        public async Task GetTournamentByName_HasTournament()
        {
            var rugbyTournament = new RugbyTournament()
            {
                Slug = "TEST"
            };

            _publicSportDataUnitOfWork.RugbyTournaments.Add(
                rugbyTournament);

            Assert.IsNotNull(await _rugbyService.GetTournamentBySlug("TEST"));
        }

        [Test]
        public async Task GetTournamentByName_NoTournament()
        {
            var rugbyTournament = new RugbyTournament()
            {
                Slug = "TEST"
            };

            _publicSportDataUnitOfWork.RugbyTournaments.Add(
                rugbyTournament);

            Assert.IsNull(await _rugbyService.GetTournamentBySlug("INCORRECT"));
        }

        [Test]
        public async Task OneEndedTournament()
        {
            var tournamentId = Guid.NewGuid();
            var seasonId = Guid.NewGuid();

            var rugbyTournament = new RugbyTournament()
            {
                Id = tournamentId,
            };

            var season = new RugbySeason()
            {
                Id = seasonId
            };

            var schedule = new SchedulerTrackingRugbySeason()
            {
                TournamentId = tournamentId,
                SeasonId = seasonId,
                SchedulerStateForManagerJobPolling = SchedulerStateForManagerJobPolling.Running,
                RugbySeasonStatus = RugbySeasonStatus.Ended
            };

            _publicSportDataUnitOfWork.RugbyTournaments.Add(
                rugbyTournament);

            _publicSportDataUnitOfWork.RugbySeasons.Add(
                season);

            _systemSportDataUnitOfWork.SchedulerTrackingRugbySeasons.Add(
                schedule);

            var endedTournaments = await _rugbyService.GetEndedTournaments();

            Assert.AreEqual(1, endedTournaments.Count());
        }

        [Test]
        public async Task NoEndedTournaments()
        {
            var tournamentId = Guid.NewGuid();
            var seasonId = Guid.NewGuid();

            var rugbyTournament = new RugbyTournament()
            {
                Id = tournamentId,
            };

            var season = new RugbySeason()
            {
                Id = seasonId
            };

            var schedule = new SchedulerTrackingRugbySeason()
            {
                TournamentId = tournamentId,
                SeasonId = seasonId,
                SchedulerStateForManagerJobPolling = SchedulerStateForManagerJobPolling.NotRunning,
                RugbySeasonStatus = RugbySeasonStatus.InProgress
            };

            _publicSportDataUnitOfWork.RugbyTournaments.Add(
                rugbyTournament);

            _publicSportDataUnitOfWork.RugbySeasons.Add(
                season);

            _systemSportDataUnitOfWork.SchedulerTrackingRugbySeasons.Add(
                schedule);

            var endedTournaments = await _rugbyService.GetEndedTournaments();

            Assert.AreEqual(0, endedTournaments.Count());
        }
    }
}