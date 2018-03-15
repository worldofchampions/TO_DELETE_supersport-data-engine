namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models
{
    using System;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Base;

    public class MotorsportRaceEventGrid: BaseModel
    {
        /// <summary> A clustered-key record identifier. </summary>
        public Guid MotorsportRaceEventId { get; set; }

        /// <summary> A clustered-key record identifier. </summary>
        public Guid MotorsportDriverId { get; set; }
        
        /// <summary> A provider driven value. </summary>
        public int GridPosition { get; set; }

        /// <summary> A provider driven value. </summary>
        public int QualifyingTimeHours { get; set; }

        /// <summary> A provider driven value. </summary>
        public int QualifyingTimeMinutes { get; set; }

        /// <summary> A provider driven value. </summary>
        public int QualifyingTimeSeconds { get; set; }
        
        /// <summary> A provider driven value. </summary>
        public int QualifyingTimeMilliseconds { get; set; }
        
        public virtual MotorsportDriver MotorsportDriver { get; set; }

        public virtual MotorsportRaceEvent MotorsportRaceEvent { get; set; }

        public virtual MotorsportTeam MotorsportTeam { get; set; }
    }
}