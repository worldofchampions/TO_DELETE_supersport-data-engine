namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Gateway.Http.DeprecatedFeed.ResponseModels.Base
{
    using System;

    public class BaseArticlesResponse
    {
        public int ID { get; set; }
        public string Headline { get; set; }
        public string Body { get; set; }
        public string Blurb { get; set; }
        public string UrlFriendlyHeadline { get; set; }
        public string SiteName { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime UpdatedDate { get; set; }
        public DateTime ArticleDate { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
    }
}
