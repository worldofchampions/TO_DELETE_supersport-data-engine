namespace SuperSportDataEngine.ApplicationLogic.Entities.Legacy.Motorsport
{
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
    using System.Collections.Generic;

    public class MotorsportDriverStandingsEntity
    {
        public MotorsportLeague MotorsportLeague { get; set; }

        public List<MotorsportDriverStanding> MotorsportDriverStandings { get; set; }
    }
}
