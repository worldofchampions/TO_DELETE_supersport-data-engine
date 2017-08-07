using System;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Cycling
{
    [Serializable]
    public class CyclingCommentModel
    {
        public int Order { get; set; }
        public DateTime Posted { get; set; }
        public string Text { get; set; }
        public string Title { get; set; }
    }
}