using System;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models
{
    [Serializable]
    public class CardsModel : PersonModel
    {
        public string CardType { get; set; }
        public int Time { get; set; }
        public int EventId { get; set; }
        public int Total { get; set; }
        public int TeamId { get; set; }
        public string TeamName { get; set; }
    }
}