using System;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Base;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models
{
    public class MotorRaceResult: BaseModel
    {
        public Guid Id { get; set; }

        public Guid DriverId { get; set; }

        public int LapsCompleted { get; set; }

        public int LapsLed { get; set; }

        public int LapsBehind { get; set; }

        public int Position { get; set; }

        public int StartingPosition { get; set; }

        public bool IsFastest { get; set; }

        public MotorTime FinishingTime { get; set; }

        public int DriverTotalPoints { get; set; }

        public int DriverBonusPoints { get; set; }

        public int DriverPenaltyPoints { get; set; }

        public int OwnerTotalPoints { get; set; }

        public int OwnerBonusPoints { get; set; }

        public int OwnerPenaltyPoints { get; set; }

        public MotorDriver MotorDriver { get; set; }

    }
}