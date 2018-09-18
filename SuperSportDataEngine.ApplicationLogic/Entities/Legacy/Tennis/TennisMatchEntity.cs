using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models.Tennis;

namespace SuperSportDataEngine.ApplicationLogic.Entities.Legacy.Tennis
{
    public class TennisMatchEntity
    {
        public TennisMatch TennisMatch { get; set; }
        public List<TennisEventSeed> TennisSeeds { get; set; }
    }
}
