using System;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Base;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models
{
    public class MotorTeamStanding: BaseModel
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

        public virtual MotorLeague MotorLeague { get; set; }

        public virtual MotorSeason MotorSeason { get; set; }

        public virtual MotorTeam MotorTeam { get; set; }
        
    }
}