using System;

namespace SuperSportDataEngine.ApplicationLogic.Entities.SystemAPI
{
    public class RugbyPlayerEntity
    {
        public Guid Id { get; set; }
        public int LegacyPlayerId { get; set; }
        public int ProviderPlayerId { get; set; }
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayNameCmsOverride { get; set; }
    }
}
