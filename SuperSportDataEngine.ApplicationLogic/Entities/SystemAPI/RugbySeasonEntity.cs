using System;

namespace SuperSportDataEngine.ApplicationLogic.Entities.SystemAPI
{
    public class RugbySeasonEntity
    {
        public Guid Id { get; set; }
        public int ProviderSeasonId { get; set; }
        public string Name { get; set; }
        public bool IsCurrent { get; set; }
        public DateTimeOffset StartDateTime { get; set; }
        public DateTimeOffset EndDateTime { get; set; }
        public int CurrentRoundNumber { get; set; }
    }
}
