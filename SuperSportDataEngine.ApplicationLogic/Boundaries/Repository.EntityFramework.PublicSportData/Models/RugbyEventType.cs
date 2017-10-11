namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models
{
    using System;

    public class RugbyEventType
    {
        /// <summary> The primary internal record identifier. </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// A unique int event code used by API clients.
        ///
        /// Note: Do not change any existing code values, as these values are being used by API
        ///       clients to perform application logic (for example, apps use this value to determine
        ///       what icon to display for a corresponding event type etc.).
        /// </summary>
        public int EventCode { get; set; }

        /// <summary> A CMS driven value. </summary>
        public string EventName { get; set; }
    }
}