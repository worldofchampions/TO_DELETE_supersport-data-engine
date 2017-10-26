namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Base
{
    using System;

    public class BaseModel
    {
        public DateTimeOffset TimestampCreated { get; set; }

        public DateTimeOffset TimestampUpdated { get; set; }
    }
}