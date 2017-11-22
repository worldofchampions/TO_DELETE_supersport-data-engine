namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Shared
{
    using System;

    [Serializable]
    public class MatchVideoModel
    {
        public int Id { get; set; }
        public string CategoryUrl { get; set; }
        public string eventId { get; set; }
        public string Name { get; set; }
        public string Thumbnail { get; set; }
        public bool Mobile { get; set; }
        public string Region { get; set; }
        public DateTime Date { get; set; }
    }
}