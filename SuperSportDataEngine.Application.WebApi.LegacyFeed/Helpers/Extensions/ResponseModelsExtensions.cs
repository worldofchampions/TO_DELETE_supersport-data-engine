using SuperSportDataEngine.Application.WebApi.LegacyFeed.Models;
using System.Collections.Generic;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Helpers
{
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