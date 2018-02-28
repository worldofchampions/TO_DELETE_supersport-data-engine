namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models
{
    using System;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Base;

    public class MotorsportRaceGrid: BaseModel
    {
        /// <summary> A clustered-key record identifier. </summary>
        public Guid MotorsportRaceId { get; set; }

        /// <summary> A clustered-key record identifier. </summary>
        public Guid MotorsportDriverId { get; set; }

        /// <summary> A clustered-key record identifier. </summary>
        public Guid MotorsportTeamId { get; set; }

        public int Position { get; set; }

        public int QualifyingTimeHours { get; set; }

        public int QualifyingTimeMinutes { get; set; }

        public int QualifyingTimeSeconds { get; set; }

        public int QualifyingTimeMilliseconds { get; set; }
        
        public virtual MotorsportDriver MotorsportDriver { get; set; }

        public virtual MotorsportRace MotorsportRace { get; set; }

        public virtual MotorsportTeam MotorsportTeam { get; set; }
    }
}