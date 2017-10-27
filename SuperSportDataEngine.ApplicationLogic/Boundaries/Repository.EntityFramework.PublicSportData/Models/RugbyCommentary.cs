namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models
{
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Base;
    using System;

    public class RugbyCommentary : BaseModel
    {
        /// <summary> The primary internal record identifier. </summary>
        public Guid Id { get; set; }

        /// <summary> A provider driven value. </summary>
        public int GameTimeRawSeconds { get; set; }

        /// <summary> This value is calculated on ingest, based on the game time in seconds. </summary>
        public int GameTimeRawMinutes { get; set; }

        /// <summary> A provider driven value. </summary>
        public string GameTimeDisplayHoursMinutesSeconds { get; set; }

        /// <summary> A provider driven value. </summary>
        public string GameTimeDisplayMinutesSeconds { get; set; }

        /// <summary> A provider driven value. </summary>
        public string CommentaryText { get; set; }

        public virtual RugbyFixture RugbyFixture { get; set; }

        public virtual RugbyTeam RugbyTeam { get; set; }

        public virtual RugbyPlayer RugbyPlayer { get; set; }

        public virtual RugbyEventType RugbyEventType { get; set; }
    }
}