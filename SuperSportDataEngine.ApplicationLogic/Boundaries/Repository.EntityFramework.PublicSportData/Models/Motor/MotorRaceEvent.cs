using System;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Base;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models
{
    public class MotorRaceEvent: BaseModel
    {
        public Guid Id { get; set; }
        public  Guid RaceId { get; set; }
        public  Guid SeasonId { get; set; }
        public MotorSpeed AverageSpeed { get; set; }
        public MotorTime Time { get; set; }
        public int SequenceNumber { get; set; }
        public DateTime StartDateTimeUtc { get; set; }
    }
}