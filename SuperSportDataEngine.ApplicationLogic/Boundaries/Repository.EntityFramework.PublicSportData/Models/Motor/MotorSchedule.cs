using System;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Base;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models
{
    public class MotorSchedule: BaseModel
    {
        public Guid Id { get; set; }

        public int ProviderRaceId { get; set; }

        public Guid CurrentChampionId { get; set; }

        public Guid PreviousChampionId { get; set; }

        public string Name { get; set; }

        public string City { get; set; }

        public string VenueName { get; set; }

        public string CountryNameFull { get; set; }

        public string CountryAbbreviation { get; set; }

        public DateTime? StartDateTimeUtc { get; set; }

        public MotorDriver CurrentChampion { get; set; }

        public MotorDriver PreviousChampion { get; set; }
    }
}