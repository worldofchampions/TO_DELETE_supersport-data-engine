using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
using System;
using System.Collections.Generic;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData
{
    public class RugbyMatchDetails
    {
        public List<RugbyCommentary> Commentary { get; set; }

        public List<RugbyPlayerLineup> TeamALineup { get; set; }

        public List<RugbyPlayerLineup> TeamBLineup { get; set; }

        public RugbyMatchStatistics TeamAMatchStatistics { get; set; }

        public RugbyMatchStatistics TeamBMatchStatistics { get; set; }
    }
}
