using System;

namespace SuperSportDataEngine.ApplicationLogic.Entities.SystemAPI
{
    public class RugbyTeamEntity
    {
        public Guid Id { get; set; }
        public int LegacyTeamId { get; set; }
        public int ProviderTeamId { get; set; }
        public string Name { get; set; }
        public string NameCmsOverride { get; set; } = null;
        public string Abbreviation { get; set; }
        public string LogoUrl { get; set; }
    }
}
