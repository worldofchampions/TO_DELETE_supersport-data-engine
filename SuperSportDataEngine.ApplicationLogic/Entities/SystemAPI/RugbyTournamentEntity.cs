using System;

namespace SuperSportDataEngine.ApplicationLogic.Entities.SystemAPI
{
    public class RugbyTournamentEntity
    {
        public Guid Id { get; set; }
        public int LegacyTournamentId { get; set; }
        public int ProviderTournamentId { get; set; }
        public string Name { get; set; }
        public string NameCmsOverride { get; set; }
        public string Slug { get; set; }
        public string Abbreviation { get; set; }
        public string LogoUrl { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsLiveScored { get; set; }
        public bool HasLogs { get; set; }
    }
}
