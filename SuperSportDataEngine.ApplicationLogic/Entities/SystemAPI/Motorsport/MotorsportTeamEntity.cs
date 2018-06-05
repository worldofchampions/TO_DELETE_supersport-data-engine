using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
using System;

namespace SuperSportDataEngine.ApplicationLogic.Entities.SystemAPI.Motorsport
{
    public class MotorsportTeamEntity
    {
        public Guid Id { get; set; }
        public int LegacyTeamId { get; set; }
        public int ProviderTeamId { get; set; }
        public string Name { get; set; }
        public string NameCmsOverride { get; set; }

        public DataProvider DataProvider { get; set; }
    }
}
