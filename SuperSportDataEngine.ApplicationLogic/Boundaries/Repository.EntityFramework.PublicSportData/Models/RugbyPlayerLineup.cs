namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models
{
    using System;

    public class RugbyPlayerLineup
    {
        /// <summary> A clustered-key record identifier. </summary>
        public Guid RugbyFixtureId { get; set; }

        /// <summary> A clustered-key record identifier. </summary>
        public Guid RugbyTeamId { get; set; }

        /// <summary> A clustered-key record identifier. </summary>
        public Guid RugbyPlayerId { get; set; }

        /// <summary> A provider driven value. </summary>
        public int ShirtNumber { get; set; }

        /// <summary> A provider driven value. </summary>
        public string PositionName { get; set; }

        /// <summary> A provider driven value. </summary>
        public bool IsCaptain { get; set; }

        //TODO: Waiting to see if STATS Prozone can supply this value directly. In the mean time, calculate this value on ingest (set to True if playerPositionId="16" or PositionName="Interchange").
        /// <summary> A calculated value, (until this value can be supplied directly by the data provider). </summary>
        public bool IsSubstitute { get; set; }

        public virtual RugbyFixture RugbyFixture { get; set; }

        public virtual RugbyTeam RugbyTeam { get; set; }

        public virtual RugbyPlayer RugbyPlayer { get; set; }
    }
}