using System;
using System.Collections.Generic;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Motorsport
{
    [Serializable]
    public class StageModel
    {
        public enum StageStatus
        {
            Dormant,
            Racing,
            OnGrid,
            SafetyCarOut,
            RedFlagOut,
            RaceFinished,
            RaceStopped,
            RaceRestart,
            RaceFalseStart,
            Completed
        }

        public enum StageName
        {
            FriPractice1,
            FriPractice2,
            SatPractice1,
            SatPractice2,
            SatQualifying,
            SunQualifying,
            Race
        }

        public int Id { get; set; }
        public int Order { get; set; }
        public DateTime Date { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public StageName Name { get; set; }
        public StageStatus Status { get; set; }
        public List<ResultMotorsportModel> StageResult { get; set; }
        public List<GridModel> StageGrid { get; set; }
        public List<DriversList> StageDriversList { get; set; }
        public List<LiveModel> StageLive { get; set; }
        public bool IsLive { get; set; }
        public string Weather { get; set; }
        public string StatusComments { get; set; }
        public int Laps { get; set; }
    }
}