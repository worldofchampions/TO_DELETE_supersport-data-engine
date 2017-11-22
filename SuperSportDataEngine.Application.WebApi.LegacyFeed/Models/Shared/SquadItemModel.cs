namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Shared
{
    using System;

    [Serializable]
    public class SquadItemModel : PlayerProfileModel
    {
        public string Team { get; set; }
        public int TeamId { get; set; }
        public string Jersey { get; set; }
    }
}