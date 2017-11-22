namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Shared
{
    using System.Collections.Generic;

    public class AlternateCommentsModel
    {
        public string Language { get; set; }
        public List<MatchEvent> Commentary { get; set; }
    }
}