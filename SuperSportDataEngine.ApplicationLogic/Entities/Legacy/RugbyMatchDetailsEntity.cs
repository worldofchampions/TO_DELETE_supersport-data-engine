namespace SuperSportDataEngine.ApplicationLogic.Entities.Legacy
{
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
    using System.Collections.Generic;

    public class RugbyMatchDetailsEntity
    {
        public List<RugbyMatchEvent> MatchEvents { get; set; }

        public List<LegacyRugbyScorerEntity> TeamAScorers { get; set; }

        public List<LegacyRugbyScorerEntity> TeamBScorers { get; set; }

        public List<RugbyPlayerLineup> TeamALineup { get; set; }

        public List<RugbyPlayerLineup> TeamBLineup { get; set; }

        public List<RugbyPlayerLineup> TeamsLineups { get; set; }

        public RugbyMatchStatistics TeamAMatchStatistics { get; set; }

        public RugbyMatchStatistics TeamBMatchStatistics { get; set; }

        public RugbyFixture RugbyFixture { get; set; }
    }
}