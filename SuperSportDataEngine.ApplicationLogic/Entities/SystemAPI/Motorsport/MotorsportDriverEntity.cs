using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
using System;

namespace SuperSportDataEngine.ApplicationLogic.Entities.SystemAPI.Motorsport
{
    public class MotorsportDriverEntity
    {
        public Guid Id { get; set; }
        public int LegacyDriverId { get; set; }
        public int ProviderDriverId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullNameCmsOverride { get; set; }
        public string CountryName { get; set; }
        public int ProviderCarId { get; set; }
        public int? CarNumber { get; set; }

        public MotorsportTeam MotorsportTeam { get; set; }
    }
}
