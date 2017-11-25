using NUnit.Framework;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models.Enums;
using SuperSportDataEngine.ApplicationLogic.Helpers;
using System;

namespace SuperSportDataEngine.ApplicationLogic.Tests
{
    public class HelperTests
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
        public void GetFixtureState_PreGameFixture_NotScheduledYet()
        {
            var now = new DateTime(2000, 01, 01, 12, 0, 0);
            var gameStartTime = new DateTime(2000, 01, 01, 12, 15, 01);

            var status = FixturesStateHelper.GetSchedulerStateForFixture(now, RugbyFixtureStatus.PreMatch, gameStartTime);
            Assert.AreEqual(SchedulerStateForRugbyFixturePolling.SchedulingNotYetStarted, status);
        }

        [Test]
        public void GetFixtureState_PreGameFixture_NotScheduledYet_GameIsPostponed()
        {
            var now = new DateTime(2000, 01, 01, 18, 0, 0);
            var gameStartTime = new DateTime(2000, 01, 01, 12, 0, 0);

            var status = FixturesStateHelper.GetSchedulerStateForFixture(now, RugbyFixtureStatus.PreMatch, gameStartTime);
            Assert.AreEqual(SchedulerStateForRugbyFixturePolling.SchedulingNotYetStarted, status);
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
        public void TestAddMinutes()
        {
            var StartDatetime = DateTime.UtcNow.AddHours(3);

            Assert.IsFalse(StartDatetime < DateTime.UtcNow - TimeSpan.FromHours(6));
        }

        [Test]
        public void TestAddMinutes2()
        {
            var StartDatetime = DateTime.UtcNow.AddHours(-7);

            Assert.IsTrue(StartDatetime < DateTime.UtcNow - TimeSpan.FromHours(6));
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
