using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.News
{
    public class NewsModel
    {
        public class RootObject
        {
            public string Headline { get; set; }
            public int ID { get; set; }
            public string Blurb { get; set; }
            public string SmallImageName { get; set; }
            public string SmallImageAlt { get; set; }
            public string LargeImageName { get; set; }
            public string LargeImageAlt { get; set; }
            public string ExtraImageName { get; set; }
            public string ExtraImageAlt { get; set; }
            public string ImageUrlLocal { get; set; }
            public string ImageUrlRemote { get; set; }
            public string DateCreated { get; set; }
            public string Category { get; set; }
            public string CategoryDisplayName { get; set; }
            public int CategoryId { get; set; }
            public string CategoryShortName { get; set; }
            public int SiteId { get; set; }
            public string SiteName { get; set; }
            public string Author { get; set; }
            public object Credit { get; set; }
            public object CreditImageUrl { get; set; }
            public object CreditUrl { get; set; }
            public string UrlName { get; set; }
            public bool LiveChat { get; set; }
            public bool WebOnly { get; set; }
            public string UrlFriendlyHeadline { get; set; }
            public string UrlFriendlyDate { get; set; }
            public bool IsMainStory { get; set; }
            public string UpdatedDate { get; set; }
            public string Keywords { get; set; }
            public bool Active { get; set; }
            public string ValidFrom { get; set; }
            public string ValidTo { get; set; }
            public object Associations { get; set; }
            public object RelatedArticles { get; set; }
        }
    }
}