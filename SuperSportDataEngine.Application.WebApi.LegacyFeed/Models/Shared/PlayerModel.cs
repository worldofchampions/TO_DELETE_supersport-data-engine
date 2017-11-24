namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Shared
{
    using System;

    [Serializable]
    public class Player : PersonModel
    {
        public int Number { get; set; }
        public string PositionName { get; set; }
        public string Jersey { get; set; }
        public bool IsSubstitute { get; set; }
        public bool IsUsedSubstitute { get; set; }
        public bool IsCaptain { get; set; }
        public int TeamId { get; set; }
    }
}