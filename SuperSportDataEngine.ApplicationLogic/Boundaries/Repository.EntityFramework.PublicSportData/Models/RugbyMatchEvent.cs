namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models
{
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Base;
    using System;

    public class RugbyMatchEvent : BaseModel
    {
        /// <summary> The primary internal record identifier. </summary>
        public Guid Id { get; set; }

        public Guid RugbyFixtureId { get; set; }

        public Guid RugbyEventTypeId { get; set; }

        public Guid RugbyTeamId { get; set; }

        /// <summary> A provider driven value. </summary>
        public int GameTimeInSeconds { get; set; }

        /// <summary> This value is calculated on ingest, based on the game time in seconds. </summary>
        public int GameTimeInMinutes { get; set; }

        /// <summary> A provider driven value. </summary>
        public float EventValue { get; set; }

        public virtual RugbyFixture RugbyFixture { get; set; }

        public virtual RugbyEventType RugbyEventType { get; set; }

        public virtual RugbyTeam RugbyTeam { get; set; }

        public virtual RugbyPlayer RugbyPlayer1 { get; set; }

        public virtual RugbyPlayer RugbyPlayer2 { get; set; }
    }
}