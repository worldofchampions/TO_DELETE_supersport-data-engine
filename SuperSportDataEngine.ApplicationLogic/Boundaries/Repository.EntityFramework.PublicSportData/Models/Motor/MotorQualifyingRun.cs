using System;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Base;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models
{
    public class MotorQualifyingRun: BaseModel
    {
        public Guid Id { get; set; }
        public MotorSpeed AverageSpeed { get; set; }
        public MotorTime Time { get; set; }
        public int SequenceNumber { get; set; }
        public DateTime StartTimeUtc { get; set; }

    }

    public abstract class MotorSpeed
    {
        public double Value { get; set; }
        public string SpeedUnitId { get; set; }
        public string SpeedUnitAbbreviation { get; set; }
    }

    public abstract class MotorTime
    {
        public int Hours { get; set; }
        public int Minutes { get; set; }
        public int Seconds { get; set; }
        public int Milliseconds { get; set; }
    }
}
