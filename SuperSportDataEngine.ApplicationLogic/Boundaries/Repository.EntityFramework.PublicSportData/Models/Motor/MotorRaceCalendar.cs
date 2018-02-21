namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models
{
    using System;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Base;

    public class MotorRaceCalendar: BaseModel
    {
        public Guid Id { get; set; }

        public DateTimeOffset? StartDateTimeUtc { get; set; }

        public DateTimeOffset? EndDateTimeUtc { get; set; }

        public virtual MotorRace MotorRace { get; set; }
    }
}