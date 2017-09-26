namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models
{
    using System;

    public class RugbyFlatLog
    {
        /// <summary> A clustered-key record identifier. </summary>
        public Guid RugbyTournamentId { get; set; }

        /// <summary> A clustered-key record identifier. </summary>
        public Guid RugbySeasonId { get; set; }

        /// <summary> A clustered-key record identifier. </summary>
        public int RoundNumber { get; set; }

        /// <summary> A clustered-key record identifier. </summary>
        public Guid RugbyTeamId { get; set; }

        public int LogPosition { get; set; }
        public int GamesPlayed { get; set; }
        public int GamesWon { get; set; }
        public int GamesDrawn { get; set; }
        public int GamesLost { get; set; }
        public int PointsFor { get; set; }
        public int PointsAgainst { get; set; }
        public int PointsDifference { get; set; }
        public int TournamentPoints { get; set; }
        public int BonusPoints { get; set; }
        public int TriesFor { get; set; }
        public int TriesAgainst { get; set; }

        public virtual RugbyTournament RugbyTournament { get; set; }
        public virtual RugbySeason RugbySeason { get; set; }
        public virtual RugbyTeam RugbyTeam { get; set; }
    }
}