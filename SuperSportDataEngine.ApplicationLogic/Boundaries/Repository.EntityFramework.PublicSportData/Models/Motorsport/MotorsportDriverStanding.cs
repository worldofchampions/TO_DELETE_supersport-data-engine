namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models
{
    using System;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Base;

    public class MotorsportDriverStanding: BaseModel
    {
        /// <summary> A clustered-key record identifier. </summary>
        public Guid MotorLeagueId { get; set; }

        /// <summary> A clustered-key record identifier. </summary>
        public Guid MotorSeasonId { get; set; }

        /// <summary> A clustered-key record identifier. </summary>
        public Guid MotorTeamId { get; set; }

        /// <summary> A clustered-key record identifier. </summary>
        public Guid MotorDriverId { get; set; }

        public int Points { get; set; }

        public int Position { get; set; }

        public int Wins { get; set; }

        public virtual MotorsportLeague MotorsportLeague { get; set; }

        public virtual MotorsportSeason RugbySeason { get; set; }

        public virtual MotorsportTeam MotorsportTeam { get; set; }

        public virtual MotorsportDriver MotorsportDriver { get; set; }

    }
}