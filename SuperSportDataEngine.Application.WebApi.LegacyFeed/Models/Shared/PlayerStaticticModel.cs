namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Shared
{
    using System;

    [Serializable]
    public class PersonStatistic : PersonModel
    {
        public PersonStaticticType StatisticType { get; set; }
        public int Rank { get; set; }
        public int Total { get; set; }
        public int TeamId { get; set; }
        public string TeamName { get; set; }

        public enum PersonStaticticType
        {
            YellowCard,
            RedCard,
            Tries,
            Dropgoals,
            Penalties,
            Points,
            Conversions
        }
    }
}