using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
using System;

namespace SuperSportDataEngine.ApplicationLogic.Entities.SystemAPI.Motorsport
{
    public class MotorsportLeagueEntity
    {
        public Guid Id { get; set; }
        public int LegacyLeagueId { get; set; }
        public int ProviderLeagueId { get; set; }
        public string Name { get; set; }
        public string NameCmsOverride { get; set; }
        public string Slug { get; set; }
        public string ProviderSlug { get; set; }
        public bool IsEnabled { get; set; }
        public DataProvider DataProvider { get; set; }

        public MotorsportSportType MotorsportSportType { get; set; }
    }
}
