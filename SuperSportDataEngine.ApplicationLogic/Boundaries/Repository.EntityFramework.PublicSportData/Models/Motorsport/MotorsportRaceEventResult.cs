namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models
{
    using System;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Base;

    public class MotorsportRaceEventResult: BaseModel
    {
        /// <summary> A clustered-key record identifier. </summary>
        public Guid MotorsportRaceEventId { get; set; }

        /// <summary> A clustered-key record identifier. </summary>
        public Guid MotorsportDriverId { get; set; }

        public int GridPosition { get; set; }

        /// <summary> 
        /// A provider driven value. 
        /// This is current position during a live event.
        /// This is final position after the event has ended.
        /// </summary>
        public int Position { get; set; }

        public int Points { get; set; }

        public int LapsCompleted { get; set; }

        public int LapsBehind { get; set; }
        
        public bool CompletedRace { get; set; }

        public string OutReason { get; set; }
        
        public int FinishingTimeHours { get; set; }

        public int FinishingTimeMinutes { get; set; }

        public int FinishingTimeSeconds { get; set; }

        public int FinishingTimeMilliseconds { get; set; }

        public int GapToLeaderTimeHours { get; set; }

        public int GapToLeaderTimeMinutes { get; set; }

        public int GapToLeaderTimeSeconds { get; set; }

        public int GapToLeaderTimeMilliseconds { get; set; }

        public int GapToCarInFrontTimeHours { get; set; }

        public int GapToCarInFrontTimeMinutes { get; set; }

        public int GapToCarInFrontTimeSeconds { get; set; }

        public int GapToCarInFrontTimeMilliseconds { get; set; }

        
        /// <summary> 
        /// Provides a mode to manually handle race data e.g. to cater for scenarios when 
        /// temporarily encountering a problem with provider data. Whilst the CMS override mode is 
        /// active, the following values are controlled manually (i.e. not set by ingest): 
        /// - TBC: Position 
        /// - TBC: FinishingTimeMinutes, etc
        /// </summary> 
        public bool CmsOverrideModeIsActive { get; set; }

        public virtual MotorsportDriver MotorsportDriver { get; set; }

        public virtual MotorsportTeam MotorsportTeam { get; set; }

        public virtual MotorsportRaceEvent MotorsportRaceEvent { get; set; }

    }
}