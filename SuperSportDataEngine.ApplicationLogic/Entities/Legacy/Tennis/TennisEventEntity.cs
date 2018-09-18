using System.Collections.Generic;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models.Tennis;

namespace SuperSportDataEngine.ApplicationLogic.Entities.Legacy.Tennis
{
    public class TennisEventEntity
    {
        public TennisEvent TennisEvent { get; set; }
        public TennisEventTennisLeagues TennisEventTennisLeague { get; set; }
    }
}
