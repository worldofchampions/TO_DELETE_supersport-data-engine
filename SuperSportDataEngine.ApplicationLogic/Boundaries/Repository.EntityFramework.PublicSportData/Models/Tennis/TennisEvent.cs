using System;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Base;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models.Tennis
{
    public class TennisEvent : BaseModel
    {
        /// <summary>
        /// A unique identifier for the TennisEvent.
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// A unique identifier for the TennisEvent for legacy purposes.
        /// </summary>
        public int LegacyEventId { get; set; }
        /// <summary>
        /// A provider driven value for this TennisEvent.
        /// </summary>
        public int ProviderEventId { get; set; }
        /// <summary>
        /// The name of this TennisEvent.
        /// </summary>
        public string EventName { get; set; }
        /// <summary>
        /// A CMS driven value for overriding the Event Name of the TennisEvent.
        /// </summary>
        public string EventNameCmsOverride { get; set; }
        /// <summary>
        /// A value indicating the Start Date of this TennisEvent.
        /// </summary>
        public DateTimeOffset StartDateUtc { get; set; }
        /// <summary>
        /// A value indicating the End Date of this TennisEvent.
        /// </summary>
        public DateTimeOffset EndDateUtc { get; set; }
        /// <summary>
        /// A value indicating the source of the data.
        /// </summary>
        public DataProvider DataProvider { get; set; }
        /// <summary>
        /// A navigation property for the TennisTournament for this TennisEvent.
        /// </summary>
        public virtual TennisTournament TennisTournament { get; set; }
        /// <summary>
        /// A navigation property for the TennisSeason for this TennisEvent.
        /// </summary>
        public virtual TennisSeason TennisSeason { get; set; }
        /// <summary>
        /// A navigation property for the TennisVenue for the TennisEvent.
        /// </summary>
        public virtual TennisVenue TennisVenue { get; set; }
        /// <summary>
        /// A provider driven value for the SurfaceType of the court.
        /// </summary>
        public virtual TennisSurfaceType TennisSurfaceType { get; set; }
    }
}
