namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.DeprecatedFeed.ResponseModels
{
    using System;

    public class HighlightVideosResponse
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
