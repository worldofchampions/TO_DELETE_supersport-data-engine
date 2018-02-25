namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models
{
    using System;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Base;

    public class MotorsportRaceCalendar : BaseModel
    {
        /// <summary> A clustered-key record identifier. </summary>
        public Guid MotorsportRaceId { get; set; }

        /// <summary> A clustered-key record identifier. </summary>
        public Guid MotorsportSeasonId { get; set; }

        public DateTimeOffset? StartDateTimeUtc { get; set; }

        //TODO: Figure out how to calculate this. Confirm if provider does not serve it.
        public DateTimeOffset? EndDateTimeUtc { get; set; }

        public string CountryName { get; set; }

        public string CountryAbbreviation { get; set; }

        public string CityName { get; set; }

        public string VenueName { get; set; }

        public virtual MotorsportRace MotorsportRace { get; set; }

        public virtual MotorsportSeason MotorsportSeason { get; set; }
    }
}
