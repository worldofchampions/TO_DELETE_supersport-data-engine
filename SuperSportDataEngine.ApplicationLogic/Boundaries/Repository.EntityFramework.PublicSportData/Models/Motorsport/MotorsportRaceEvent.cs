namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models
{
    using System;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Base;

    /// <summary> A calendar of race events. </summary>
    public class MotorsportRaceEvent : BaseModel
    {
        /// <summary> The primary internal record identifier. </summary>
        public Guid Id { get; set; }

        /// <summary> A unique int record identifier for legacy purposes. </summary>
        public int LegacyRaceEventId { get; set; }

        /// <summary> A provider driven value. </summary>
        public int ProviderRaceEventId { get; set; }

        /// <summary> A provider driven value. </summary>
        public DateTimeOffset? StartDateTimeUtc { get; set; }

        /// <summary> A provider driven value. </summary>
        public string CountryName { get; set; }

        /// <summary> A provider driven value. </summary>
        public string CountryAbbreviation { get; set; }

        /// <summary> A provider driven value. </summary>
        public string CityName { get; set; }

        /// <summary> A provider driven value. </summary>
        public string CircuitName { get; set; }

        /// <summary> A provider driven value. </summary>
        public MotorsportRaceEventStatus MotorsportRaceEventStatus { get; set; }

        /// <summary> A CMS driven value to set whether the race event is current. </summary>
        public bool IsCurrent { get; set; }
        
        /// <summary> A CMS driven value to indicate if a race event is live scored. </summary>
        public bool IsLiveScored { get; set; }

        public virtual MotorsportRace MotorsportRace { get; set; }

        public virtual MotorsportSeason MotorsportSeason { get; set; }
    }
}