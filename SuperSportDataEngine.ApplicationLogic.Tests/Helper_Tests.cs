using NUnit.Framework;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models.Enums;
using SuperSportDataEngine.ApplicationLogic.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSportDataEngine.ApplicationLogic.Tests
{
    public class Helper_Tests
    {
        [Test]
        public void GetFixtureState_PreGameFixture_ShouldNotNBeScheduled()
        {
            var now = new DateTime(2000, 01, 01, 12, 0, 0);
            var gameStartTime = new DateTime(2000, 01, 01, 13, 0, 0);

            var status = FixturesStateHelper.GetSchedulerStateForFixture(now, RugbyFixtureStatus.PreMatch, gameStartTime);
            Assert.AreEqual(SchedulerStateForRugbyFixturePolling.SchedulingNotYetStarted, status);
        }

        [Test]
        public void GetFixtureState_PreGameFixture_PreLivePolling()
        {
            var now = new DateTime(2000, 01, 01, 12, 0, 0);
            var gameStartTime = new DateTime(2000, 01, 01, 12, 14, 59);

            var status = FixturesStateHelper.GetSchedulerStateForFixture(now, RugbyFixtureStatus.PreMatch, gameStartTime);
            Assert.AreEqual(SchedulerStateForRugbyFixturePolling.PreLivePolling, status);
        }

        [Test]
        public void GetFixtureState_LiveFixture_LivePolling_FirstHalf()
        {
            var now = new DateTime(2000, 01, 01, 12, 0, 0);
            var gameStartTime = new DateTime(2000, 01, 01, 12, 14, 59);

            var status = FixturesStateHelper.GetSchedulerStateForFixture(now, RugbyFixtureStatus.FirstHalf, gameStartTime);
            Assert.AreEqual(SchedulerStateForRugbyFixturePolling.LivePolling, status);
        }

        [Test]
        public void GetFixtureState_LiveFixture_LivePolling_HalfTime()
        {
            var now = new DateTime(2000, 01, 01, 12, 0, 0);
            var gameStartTime = new DateTime(2000, 01, 01, 12, 14, 59);

            var status = FixturesStateHelper.GetSchedulerStateForFixture(now, RugbyFixtureStatus.HalfTime, gameStartTime);
            Assert.AreEqual(SchedulerStateForRugbyFixturePolling.LivePolling, status);
        }

        [Test]
        public void GetFixtureState_LiveFixture_LivePolling_SecondHalf()
        {
            var now = new DateTime(2000, 01, 01, 12, 0, 0);
            var gameStartTime = new DateTime(2000, 01, 01, 12, 14, 59);

            var status = FixturesStateHelper.GetSchedulerStateForFixture(now, RugbyFixtureStatus.SecondHalf, gameStartTime);
            Assert.AreEqual(SchedulerStateForRugbyFixturePolling.LivePolling, status);
        }

        [Test]
        public void GetFixtureState_LiveFixture_LivePolling_FullTime()
        {
            var now = new DateTime(2000, 01, 01, 12, 0, 0);
            var gameStartTime = new DateTime(2000, 01, 01, 12, 14, 59);

            var status = FixturesStateHelper.GetSchedulerStateForFixture(now, RugbyFixtureStatus.FullTime, gameStartTime);
            Assert.AreEqual(SchedulerStateForRugbyFixturePolling.LivePolling, status);
        }

        [Test]
        public void GetFixtureState_EndedFixture_PostMatch_GameEnd()
        {
            var now = new DateTime(2000, 01, 01, 13, 51, 0);
            var gameStartTime = new DateTime(2000, 01, 01, 12, 0, 0);

            var status = FixturesStateHelper.GetSchedulerStateForFixture(now, RugbyFixtureStatus.Result, gameStartTime);
            Assert.AreEqual(SchedulerStateForRugbyFixturePolling.PostLivePolling, status);
        }

        [Test]
        public void GetFixtureState_EndedFixture_ResultOnlyPolling_GameEnd()
        {
            var now = new DateTime(2000, 01, 01, 14, 15, 0);
            var gameStartTime = new DateTime(2000, 01, 01, 12, 0, 0);

            var status = FixturesStateHelper.GetSchedulerStateForFixture(now, RugbyFixtureStatus.Result, gameStartTime);
            Assert.AreEqual(SchedulerStateForRugbyFixturePolling.ResultOnlyPolling, status);
        }

        [Test]
        public void GetFixtureState_EndedFixture_SchedulingComplete_GameEnd()
        {
            var now = new DateTime(2000, 01, 01, 15, 15, 0);
            var gameStartTime = new DateTime(2000, 01, 01, 12, 0, 0);

            var status = FixturesStateHelper.GetSchedulerStateForFixture(now, RugbyFixtureStatus.Result, gameStartTime);
            Assert.AreEqual(SchedulerStateForRugbyFixturePolling.SchedulingCompleted, status);
        }
    }
}
