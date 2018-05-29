namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Motorsport
{
    using System;

    [Serializable]
    public class CommentModel
    {
        public int order { get; set; }
        public string time { get; set; }
        public string text { get; set; }
    }
}
