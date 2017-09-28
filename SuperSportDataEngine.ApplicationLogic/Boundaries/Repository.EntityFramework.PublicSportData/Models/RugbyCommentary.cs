namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models
{
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
    using System;

    public class RugbyCommentary
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

        /// <summary>
        /// The provider's event type identifier.
        ///
        /// This is persisted here solely in order to allow RugbyEventType to be retrospectively
        /// re-calculated if needed, as business wants to keep as much historic data as possible.
        /// Therefore, this field will allow new event types to be supported at a later stage without
        /// requiring a re-import of this data set.
        /// </summary>
        public int? ProviderEventTypeId { get; set; }

        public DataProvider DataProvider { get; set; }

        public virtual RugbyFixture RugbyFixture { get; set; }

        public virtual RugbyTeam RugbyTeam { get; set; }

        public virtual RugbyPlayer RugbyPlayer { get; set; }

        // TODO: [Davide] Add a link to the internal events table once those schema tables have been implemented.
        //public virtual RugbyEventType RugbyEventType { get; set; }
    }
}