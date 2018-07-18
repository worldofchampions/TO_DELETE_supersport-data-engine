using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
using System;

namespace SuperSportDataEngine.ApplicationLogic.Entities.SystemAPI.Motorsport
{
    public class MotorsportRaceEventEntity
    {
        public Guid Id { get; set; }
        public int LegacyRaceEventId { get; set; }
        public int ProviderRaceEventId { get; set; }
        public DateTimeOffset? StartDateTimeUtc { get; set; }
        public string CountryName { get; set; }
        public string CountryAbbreviation { get; set; }
        public string CityName { get; set; }
        public string CircuitName { get; set; }

        public MotorsportRaceEventStatus MotorsportRaceEventStatus { get; set; }

        public bool IsCurrent { get; set; }
        public bool IsLiveScored { get; set; }

        public virtual MotorsportDriver RaceEventWinner { get; set; }
    }
}
