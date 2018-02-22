namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models
{
    using System;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Base;

    public class MotorsportRaceResult: BaseModel
    {
        /// <summary> A clustered-key record identifier. </summary>
        public Guid MotorsportRaceId { get; set; }

        /// <summary> A clustered-key record identifier. </summary>
        public Guid MotorsportDriverId { get; set; }

        public int LapsCompleted { get; set; }

        public int LapsLed { get; set; }

        public int LapsBehind { get; set; }

        public int Position { get; set; }

        public int StartingPosition { get; set; }

        public bool IsFastest { get; set; }

        public int FinishingTimeHours { get; set; }

        public int FinishingTimeMinutes { get; set; }

        public int FinishingTimeSeconds { get; set; }

        public int FinishingTimeMilliseconds { get; set; }

        public int DriverTotalPoints { get; set; }

        public int DriverBonusPoints { get; set; }

        public int DriverPenaltyPoints { get; set; }

        public int OwnerTotalPoints { get; set; }

        public int OwnerBonusPoints { get; set; }

        public int OwnerPenaltyPoints { get; set; }

        public MotorsportDriver MotorsportDriver { get; set; }

    }
}