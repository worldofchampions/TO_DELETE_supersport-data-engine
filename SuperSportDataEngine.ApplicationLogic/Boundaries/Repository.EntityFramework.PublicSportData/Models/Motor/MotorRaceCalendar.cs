namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models
{
    using System;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Base;

    public class MotorRaceCalendar: BaseModel
    {
        public Guid Id { get; set; }

        public int LegacyRaceId { get; set; }

        public int ProviderRaceId { get; set; }

        public int ProviderEventId { get; set; }

        public string RaceName { get; set; }

        public string RaceNameShort { get; set; }

        public string CountryName { get; set; }

        public string CountryAbbreviation { get; set; }

        public string City { get; set; }

        public string VenueName { get; set; }

        public DateTimeOffset? StartDateTimeUtc { get; set; }

        public DateTimeOffset? EndDateTimeUtc { get; set; }

        public  virtual  MotorLeague MotorLeague { get; set; }

        public virtual MotorRace MotorRace { get; set; }
    }
}