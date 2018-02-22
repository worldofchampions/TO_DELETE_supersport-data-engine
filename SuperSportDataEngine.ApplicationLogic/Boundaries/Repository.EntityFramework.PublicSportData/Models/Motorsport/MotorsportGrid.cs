namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models
{
    using System;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Base;

    public class MotorsportGrid: BaseModel
    {
        /// <summary> A clustered-key record identifier. </summary>
        public Guid RaceId { get; set; }

        /// <summary> A clustered-key record identifier. </summary>
        public Guid DriverId { get; set; }

        public int Position { get; set; }

        public int QualifyingTimeHours { get; set; }

        public int QualifyingTimeMinutes { get; set; }

        public int QualifyingTimeSeconds { get; set; }

        public int QualifyingTimeMilliseconds { get; set; }
        
        public MotorsportDriver MotorsportDriver { get; set; }

        public MotorsportRace MotorsportRace { get; set; }
    }
}