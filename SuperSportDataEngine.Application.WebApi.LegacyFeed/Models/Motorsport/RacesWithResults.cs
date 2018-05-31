namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Models.Motorsport
{
    using System.Collections.Generic;

    public class RacesWithResults : Races
    {
        public List<ResultMotorsport> ResultMotorsport { get; set; }
    }
}
