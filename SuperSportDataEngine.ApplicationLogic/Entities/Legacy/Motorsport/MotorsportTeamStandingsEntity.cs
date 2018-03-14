namespace SuperSportDataEngine.ApplicationLogic.Entities.Legacy.Motorsport
{
    using System.Collections.Generic;
    using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;

    public class MotorsportTeamStandingsEntity
    {
        public MotorsportLeague MotorsportLeague { get; set; }

        public List<MotorsportTeamStanding> MotorsportTeamStandings { get; set; }
    }
}
