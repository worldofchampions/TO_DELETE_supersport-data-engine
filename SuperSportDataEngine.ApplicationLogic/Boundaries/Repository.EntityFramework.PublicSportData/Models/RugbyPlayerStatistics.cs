using System;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Base;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models
{
    public class RugbyPlayerStatistics : BaseModel
    {
        /// <summary> A clustered-key record identifier. </summary>
        public Guid RugbyPlayerId { get; set; }
        
        /// <summary> A clustered-key record identifier. </summary>
        public Guid RugbyTeamId { get; set; }

        /// <summary> A clustered-key record identifier. </summary>
        public Guid RugbyTournamentId { get; set; }

        /// <summary> A clustered-key record identifier. </summary>
        public Guid RugbySeasonId { get; set; }
        
        public int Rank { get; set; }

        public int TriesScored { get; set; }

        public int PenaltiesScored { get; set; }

        public int ConversionsScored { get; set; }

        public int DropGoalsScored { get; set; }

        public virtual RugbyTournament RugbyTournament { get; set; }

        public virtual RugbyPlayer RugbyPlayer { get; set; }

        public virtual RugbySeason RugbySeason { get; set; }

        public virtual RugbyTeam RugbyTeam { get; set; }
    }
}