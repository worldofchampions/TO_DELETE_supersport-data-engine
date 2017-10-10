using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
using System;
using System.Collections.Generic;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData
{
    public class RugbyMatchDetails
    {
        public IEnumerable<RugbyCommentary> Commentary { get; set; }

        public IEnumerable<RugbyPlayerLineup> TeamALineup { get; set; }

        public IEnumerable<RugbyPlayerLineup> TeamBLineup { get; set; }

        public IEnumerable<RugbyMatchStatistics> TeamAMatchStatistics { get; set; }

        public IEnumerable<RugbyMatchStatistics> TeamBMatchStatistics { get; set; }
    }
}
