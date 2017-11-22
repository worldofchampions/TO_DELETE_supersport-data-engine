namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Helpers.Extensions
{
    using System.Collections.Generic;
    using SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Shared;

    public static class ResponseModelsExtensions
    {
        public static void AssignOrderingIds(this List<MatchEvent> matchEvents)
        {
            foreach (var item in matchEvents)
            {
                item.Id = matchEvents.IndexOf(item);
            }
        }
    }
}