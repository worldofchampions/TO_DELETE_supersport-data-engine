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

        public string CircuitName { get; set; }

        public int Position { get; set; }

        public int LapsCompleted { get; set; }

        public bool CompletedRace { get; set; }

        public string OutReason { get; set; }
        
        public int GridPosition { get; set; }
        
        public int FinishingTimeHours { get; set; }

        public int FinishingTimeMinutes { get; set; }

        public int FinishingTimeSeconds { get; set; }

        public int FinishingTimeMilliseconds { get; set; }

        public int DriverTotalPoints { get; set; }

        // TODO: Confirm if provider serves fastes lap info
        //public int FastesLap { get; set; }

        //public int FastestLapTimeHours { get; set; }

        //public int FastestLapTimeMinutes { get; set; }

        //public int FastestLapTimeSeconds { get; set; }

        //public int FastestLapTimeMilliseconds { get; set; }

        // TODO: Confirm if provider serves this value
        // Not a priority: Whishlist item from BRD
        //public int PitStops { get; set; }

        public virtual MotorsportDriver MotorsportDriver { get; set; }

        public virtual MotorsportTeam MotorsportTeam { get; set; }
    }
}