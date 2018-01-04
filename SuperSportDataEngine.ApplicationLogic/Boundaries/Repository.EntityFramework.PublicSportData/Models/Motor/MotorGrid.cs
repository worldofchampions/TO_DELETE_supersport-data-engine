using System;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Base;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models
{
    public class MotorGrid: BaseModel
    {
        public Guid RaceId { get; set; }    

        public Guid DriverId { get; set; }

        public int Position { get; set; }

        public MotorTime QualifyingTime { get; set; }

        public MotorDriver MotorDriver { get; set; }

        public MotorRace MotorRace { get; set; }
    }
}