namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models
{
    using System;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Base;

    public class MotorsportDriverStanding: BaseModel
    {
        /// <summary> A clustered-key record identifier. </summary>
        public Guid MotorsportLeagueId { get; set; }

        /// <summary> A clustered-key record identifier. </summary>
        public Guid MotorsportSeasonId { get; set; }

        /// <summary> A clustered-key record identifier. </summary>
        public Guid MotorsportTeamId { get; set; }

        /// <summary> A clustered-key record identifier. </summary>
        public Guid MotorsportDriverId { get; set; }

        /// <summary> A provider driven value. </summary>
        public int Position { get; set; }

        /// <summary> A provider driven value. </summary>
        public int Points { get; set; }

        /// <summary> A provider driven value. </summary>
        public int Wins { get; set; }

        public virtual MotorsportLeague MotorsportLeague { get; set; }

        public virtual MotorsportSeason MotorsportSeason { get; set; }

        public virtual MotorsportTeam MotorsportTeam { get; set; }

        public virtual MotorsportDriver MotorsportDriver { get; set; }

    }
}