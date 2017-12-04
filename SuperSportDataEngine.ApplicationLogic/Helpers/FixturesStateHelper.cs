using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.SystemSportData.Models.Enums;
using System;

namespace SuperSportDataEngine.ApplicationLogic.Helpers
{
    public static class FixturesStateHelper
    {
        public static SchedulerStateForRugbyFixturePolling GetSchedulerStateForFixture(DateTime now, RugbyFixtureStatus gameStatus, DateTime startDateTime)
        {
            if (gameStatus == RugbyFixtureStatus.PreMatch &&
                (now.AddMinutes(15)) < startDateTime)
                return SchedulerStateForRugbyFixturePolling.SchedulingNotYetStarted;

            if (gameStatus == RugbyFixtureStatus.PreMatch &&
                (now.AddMinutes(15)) > startDateTime &&
                (now.AddMinutes(15)) < (startDateTime + TimeSpan.FromHours(3)))
                return SchedulerStateForRugbyFixturePolling.PreLivePolling;

            if ((gameStatus == RugbyFixtureStatus.FirstHalf ||
                gameStatus == RugbyFixtureStatus.HalfTime ||
                gameStatus == RugbyFixtureStatus.SecondHalf ||
                gameStatus == RugbyFixtureStatus.FullTime) &&
                (now.AddMinutes(15)) > startDateTime &&
                (now.AddMinutes(15)) < (startDateTime + TimeSpan.FromHours(3)))
                return SchedulerStateForRugbyFixturePolling.LivePolling;

            if (gameStatus == RugbyFixtureStatus.Result &&
                now > (startDateTime.AddMinutes(110)) &&
                now < (startDateTime.AddMinutes(125)))
                return SchedulerStateForRugbyFixturePolling.PostLivePolling;

            if (gameStatus == RugbyFixtureStatus.Result &&
               now > (startDateTime.AddMinutes(125)) &&
               now < (startDateTime.AddMinutes(185)))
                return SchedulerStateForRugbyFixturePolling.ResultOnlyPolling;

            if (gameStatus == RugbyFixtureStatus.Result &&
               now > (startDateTime.AddMinutes(185)) &&
               now < (startDateTime.AddHours(6)))
                return SchedulerStateForRugbyFixturePolling.SchedulingCompleted;

            return SchedulerStateForRugbyFixturePolling.SchedulingNotYetStarted;
        }
    }
}
