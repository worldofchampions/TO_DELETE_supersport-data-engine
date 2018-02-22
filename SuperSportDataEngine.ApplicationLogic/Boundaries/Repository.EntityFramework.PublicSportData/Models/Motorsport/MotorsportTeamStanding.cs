namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models
{
    using System;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Base;

    public class MotorsportTeamStanding: BaseModel
    {
        /// <summary> A clustered-key record identifier. </summary>
        public Guid MotorLeagueId { get; set; }

        /// <summary> A clustered-key record identifier. </summary>
        public Guid MotorSeasonId { get; set; }

        /// <summary> A clustered-key record identifier. </summary>
        public Guid MotorTeamId { get; set; }

        public int Points { get; set; }

        public int Rank { get; set; }

        public int Wins { get; set; }

        public int FinishedSecond { get; set; }

        public int FinishedThird { get; set; }

        public int Top5Finishes { get; set; }

        public int Top10Finishes { get; set; }

        public int Top15Finishes { get; set; }

        public int Top20Finishes { get; set; }

        public int DidNotFinish { get; set; }

        public int Earnings { get; set; }

        public int Poles { get; set; }

        public int Starts { get; set; }

        public virtual MotorsportLeague MotorsportLeague { get; set; }

        public virtual MotorsportSeason MotorsportSeason { get; set; }

        public virtual MotorsportTeam MotorsportTeam { get; set; }
        
    }
}